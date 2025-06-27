![image](https://github.com/user-attachments/assets/916aca90-c548-4142-976f-238f84ee5809)

![image](https://github.com/user-attachments/assets/52dbf475-b23b-4b06-ae7d-0bfa7d85ed3d)

# 이재웅
- https://github.com/jamesnet214
- https://jamesnet.dev
- https://github.com/jamesnetgroup

# Ollama API Management System

## 📑 목차

- [1. Server (Minimal API)](#1-server-minimal-api)
  - [Project 생성](#project-생성)
  - [간단한 API 소스코드](#간단한-api-소스코드)
  - [Ollama 관리 API 전체 소스코드](#ollama-관리-api-전체-소스코드)
  - [API 엔드포인트별 통신 방식](#api-엔드포인트별-통신-방식)
  - [주요 모델](#주요-모델)
  - [Api Client 전체 소스코드](#api-client-전체-소스코드)
  - [API Client 정의](#api-client-정의)
  - [SignalR Client 정의](#signalr-client-정의)
  - [API 주소 목록](#api-주소-목록)

---

## 1. Server (Minimal API)

#### Project 생성
```
dotnet new web -n MyOllamaAPI
```

#### 간단한 API 소스코드
```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/users", () =>
{
    return new[] { "James", "Vicky" };
});

app.Run();
```

#### Ollama 관리 API 전체 소스코드
```csharp
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSignalR();
builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddHostedService<ModelMonitorService>();

var app = builder.Build();

app.UseCors();
app.MapHub<ModelHub>("/modelhub");

app.MapGet("/api/models", async () =>
{
    var listProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = "list",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    listProcess.Start();
    var listOutput = await listProcess.StandardOutput.ReadToEndAsync();
    await listProcess.WaitForExitAsync();

    var psProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = "ps",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    psProcess.Start();
    var psOutput = await psProcess.StandardOutput.ReadToEndAsync();
    await psProcess.WaitForExitAsync();

    var models = ParseOllamaModels(listOutput, psOutput);
    return Results.Ok(new { models, count = models.Count });
});

app.MapPost("/api/models/{modelName}/start", async (string modelName, IHubContext<ModelHub> hubContext) =>
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = $"run {modelName}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    await process.WaitForExitAsync();

    await Task.Delay(2000);
    var status = await CheckModelRunning(modelName) ? "Running" : "Error";
    await hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status);

    return Results.Ok(new { success = true });
});

app.MapPost("/api/models/{modelName}/stop", async (string modelName, IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Ollama");
    var content = new StringContent(JsonSerializer.Serialize(new { model = modelName, keep_alive = 0 }), System.Text.Encoding.UTF8, "application/json");

    await client.PostAsync("api/generate", content);
    await Task.Delay(1000);

    var status = await CheckModelRunning(modelName) ? "Running" : "Stopped";
    await hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status);

    return Results.Ok(new { success = true });
});

app.MapPost("/api/chat", async (ChatRequest request, IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory) =>
{
    if (!await CheckModelRunning(request.Model))
    {
        await hubContext.Clients.All.SendAsync("ChatMessageReceived", $"모델 {request.Model}을 시작하는 중입니다...");

        var startProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = $"run {request.Model}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        startProcess.Start();
        await startProcess.WaitForExitAsync();

        await Task.Delay(3000);

        if (!await CheckModelRunning(request.Model))
        {
            await hubContext.Clients.All.SendAsync("ChatMessageReceived", $"모델 {request.Model}을 시작할 수 없습니다.");
            return Results.BadRequest(new { success = false });
        }
    }

    var client = httpClientFactory.CreateClient("Ollama");
    var content = new StringContent(
        JsonSerializer.Serialize(new { model = request.Model, prompt = request.Message, stream = false }),
        System.Text.Encoding.UTF8, "application/json");

    var response = await client.PostAsync("api/generate", content);
    var responseContent = await response.Content.ReadAsStringAsync();

    if (response.IsSuccessStatusCode)
    {
        var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent);
        var message = ollamaResponse?.response ?? "응답을 생성할 수 없습니다.";
        await hubContext.Clients.All.SendAsync("ChatMessageReceived", message);
        return Results.Ok(new { success = true, response = message });
    }

    var testResponse = $"테스트 응답: '{request.Message}'에 대한 답변입니다.";
    await hubContext.Clients.All.SendAsync("ChatMessageReceived", testResponse);
    return Results.Ok(new { success = true, response = testResponse });
});

app.Run();

static List<OllamaModel> ParseOllamaModels(string listOutput, string psOutput)
{
    var runningModels = new HashSet<string>();

    foreach (var line in psOutput.Split('\n').Skip(1))
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0) runningModels.Add(parts[0]);
    }

    var models = new List<OllamaModel>();
    foreach (var line in listOutput.Split('\n').Skip(1))
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 4)
        {
            models.Add(new OllamaModel
            {
                Name = parts[0],
                Size = $"{parts[2]} {parts[3]}",
                LastUsed = string.Join(" ", parts.Skip(4)),
                Status = runningModels.Contains(parts[0]) ? "Running" : "Stopped"
            });
        }
    }
    return models;
}

static async Task<bool> CheckModelRunning(string modelName)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = "ps",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    var output = await process.StandardOutput.ReadToEndAsync();
    await process.WaitForExitAsync();
    return output.Contains(modelName);
}

public class ModelHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "ModelUpdates");
        await base.OnConnectedAsync();
    }
}

public class ModelMonitorService : BackgroundService
{
    private readonly IHubContext<ModelHub> _hubContext;
    private readonly Dictionary<string, string> _lastStatus = new();

    public ModelMonitorService(IHubContext<ModelHub> hubContext) => _hubContext = hubContext;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var currentStatus = await GetModelStatus();

            foreach (var (modelName, status) in currentStatus)
            {
                if (!_lastStatus.TryGetValue(modelName, out var prevStatus) || prevStatus != status)
                {
                    _lastStatus[modelName] = status;
                    await _hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status);
                }
            }

            foreach (var removed in _lastStatus.Keys.Except(currentStatus.Keys).ToList())
                _lastStatus.Remove(removed);

            await Task.Delay(3000, stoppingToken);
        }
    }

    private async Task<Dictionary<string, string>> GetModelStatus()
    {
        var result = new Dictionary<string, string>();
        var running = new HashSet<string>();

        var psProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "ps",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        psProcess.Start();
        var psOutput = await psProcess.StandardOutput.ReadToEndAsync();
        await psProcess.WaitForExitAsync();

        foreach (var line in psOutput.Split('\n').Skip(1))
        {
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) running.Add(parts[0]);
        }

        var listProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "list",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        listProcess.Start();
        var listOutput = await listProcess.StandardOutput.ReadToEndAsync();
        await listProcess.WaitForExitAsync();

        foreach (var line in listOutput.Split('\n').Skip(1))
        {
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
            {
                var modelName = parts[0];
                result[modelName] = running.Contains(modelName) ? "Running" : "Stopped";
            }
        }

        return result;
    }
}

public class OllamaModel
{
    public string Name { get; set; } = "";
    public string Size { get; set; } = "";
    public string LastUsed { get; set; } = "";
    public string Status { get; set; } = "";
}

public class ChatRequest
{
    public string Message { get; set; } = "";
    public string Model { get; set; } = "";
}

public class OllamaResponse
{
    public string response { get; set; } = "";
}
```

### API 엔드포인트별 통신 방식

### 1. `GET /api/models`
- **Ollama 통신**: CMD 명령어 2개 실행
  - `ollama list` (전체 모델 목록)
  - `ollama ps` (실행중인 모델 상태)
- **SignalR**: 사용 안함 (단순 조회)

### 2. `POST /api/models/{modelName}/start`
- **Ollama 통신**: CMD 명령어 `ollama run {modelName}` 실행
- **SignalR**: 모델 시작 후 상태를 모든 클라이언트에게 실시간 브로드캐스트

### 3. `POST /api/models/{modelName}/stop`
- **Ollama 통신**: HTTP API 호출 (`http://localhost:11434/api/generate`)
  - JSON으로 `keep_alive: 0` 전송하여 모델 종료
- **SignalR**: 모델 중지 후 상태를 모든 클라이언트에게 실시간 브로드캐스트

### 4. `POST /api/chat`
- **Ollama 통신**: 
  - 모델 상태 확인: CMD `ollama ps`
  - 모델 시작 (필요시): CMD `ollama run {modelName}`
  - 채팅 요청: HTTP API (`/api/generate`)
- **SignalR**: AI 응답을 모든 클라이언트에게 실시간 전송

**핵심**: CMD 명령어는 모델 관리용, HTTP API는 실제 AI 통신용으로 구분해서 사용

#### 주요 모델
```csharp
public partial class ModelItem : ObservableObject
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _size;
    [ObservableProperty]
    private string _lastUsed;
    [ObservableProperty]
    private string _status;
}

public class UserMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

public class AIMessage
{
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string ModelName { get; set; }
    public bool IsCodeBlock { get; set; }
    public string CodeLanguage { get; set; }
}
```

#### Api Client 전체 소스코드
```
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OllamaHub.Support.Local.Services;

public class ApiClient<T>
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        _http = new();
        _http.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<List<T>> GetAsync(string url)
    {
        var json = await _http.GetStringAsync(url);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<ApiResponse<List<T>>>(json, options);
        return response?.Models ?? new List<T>();
    }

    public async Task<string> PostAsync(string url)
    {
        var response = await _http.PostAsync(url, null);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string url, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _http.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _http?.Dispose();
    }
}

public class ApiResponse<T>
{
    public string Message { get; set; } = "";
    public int Count { get; set; }
    public T Models { get; set; } = default!;
    public bool Success { get; set; } = true;
}
```

#### API Client 정의
```csharp
private readonly ApiClient<ModelItem> _apiClient;
public MainViewModel()
{ 
    _apiClient = new();
}
```

#### SignalR Client 정의
```csharp
  _hubConnection = new HubConnectionBuilder()
      .WithUrl("https://localhost:7171/modelhub")
      .WithAutomaticReconnect()
      .Build();

  _hubConnection.On<string, string>("ModelStatusChanged", (modelName, status) =>
  {
      var model = Models.FirstOrDefault(m => m.Name == modelName);
      if (model != null)
      {
          // 로컬 LLM 모델 상태 Received 구현
      }
  });

  _hubConnection.On<string>("ChatMessageReceived", (message) =>
  {
      // 결과 메시지 Received 구현
  });

  _hubConnection.Closed += async (error) =>
  {
      await Task.Delay(new Random().Next(0, 5) * 1000);
      await _hubConnection.StartAsync();
  };

  _hubConnection.Reconnecting += (error) => Task.CompletedTask;
  _hubConnection.Reconnected += (connectionId) => Task.CompletedTask;
```

#### API 주소 목록
- LLM 로컬 모델 목록: https://localhost:7171/api/models
- LLM 로컬 모델 시작(Run): https://localhost:7171/api/models/{model.Name}/start
- LLM 로컬 모델 중지(Stop): https://localhost:7171/api/models/{model.Name}/stop
- LLM 채팅(질문): https://localhost:7171/api/chat { message = Message, model = CurrentModel.Name }

------------



# OpenSilver 설치 방법
wasm-tools 워크로드 설치
```
dotnet workload install wasm-tools
```

오픈실버 홈페이지
```
[http://opensilver.com](https://opensilver.net/)
```


