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

// API를 사용하여 모델 목록 가져오기
app.MapGet("/api/models", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Ollama");

    try
    {
        // 설치된 모델 목록 가져오기
        var tagsResponse = await client.GetAsync("api/tags");
        var runningResponse = await client.GetAsync("api/ps");

        if (!tagsResponse.IsSuccessStatusCode)
        {
            return Results.Problem("설치된 모델 목록을 가져올 수 없습니다.");
        }

        var tagsContent = await tagsResponse.Content.ReadAsStringAsync();
        var runningContent = runningResponse.IsSuccessStatusCode
            ? await runningResponse.Content.ReadAsStringAsync()
            : "{}";

        var tagsData = JsonSerializer.Deserialize<TagsResponse>(tagsContent);
        var runningData = JsonSerializer.Deserialize<RunningModelsResponse>(runningContent);

        var runningModelNames = runningData?.models?.Select(m => m.name).ToHashSet() ?? new HashSet<string>();

        var models = tagsData?.models?.Select(model => new OllamaModel
        {
            Name = model.name,
            Size = FormatBytes(model.size),
            LastUsed = model.modified_at?.ToString("yyyy-MM-dd HH:mm") ?? "알 수 없음",
            Status = runningModelNames.Contains(model.name) ? "Running" : "Stopped"
        }).ToList() ?? new List<OllamaModel>();

        return Results.Ok(new { models, count = models.Count });
    }
    catch (Exception ex)
    {
        return Results.Problem($"모델 목록을 가져오는 중 오류가 발생했습니다: {ex.Message}");
    }
});

app.MapPost("/api/models/{modelName}/start", async (string modelName, IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Ollama");

    try
    {
        // 모델을 메모리에 로드 (빈 프롬프트로 generate 호출)
        var content = new StringContent(
            JsonSerializer.Serialize(new { model = modelName, prompt = "", keep_alive = "5m" }),
            System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/generate", content);

        await Task.Delay(2000); // 로딩 대기

        var status = await CheckModelRunningApi(client, modelName) ? "Running" : "Error";
        await hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status);

        return Results.Ok(new { success = true });
    }
    catch (Exception ex)
    {
        await hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, "Error");
        return Results.Problem($"모델 시작 중 오류: {ex.Message}");
    }
});

app.MapPost("/api/models/{modelName}/stop", async (string modelName, IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Ollama");

    try
    {
        // 모델 언로드 (keep_alive: 0으로 설정)
        var content = new StringContent(
            JsonSerializer.Serialize(new { model = modelName, prompt = "", keep_alive = 0 }),
            System.Text.Encoding.UTF8, "application/json");

        await client.PostAsync("api/generate", content);
        await Task.Delay(1000);

        var status = await CheckModelRunningApi(client, modelName) ? "Running" : "Stopped";
        await hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status);

        return Results.Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return Results.Problem($"모델 중지 중 오류: {ex.Message}");
    }
});

app.MapPost("/api/chat", async (ChatRequest request, IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("Ollama");

    try
    {
        if (!await CheckModelRunningApi(client, request.Model))
        {
            await hubContext.Clients.All.SendAsync("ChatMessageReceived", $"모델 {request.Model}을 시작하는 중입니다...");

            // 모델 시작
            var startContent = new StringContent(
                JsonSerializer.Serialize(new { model = request.Model, prompt = "", keep_alive = "5m" }),
                System.Text.Encoding.UTF8, "application/json");

            await client.PostAsync("api/generate", startContent);
            await Task.Delay(3000);

            if (!await CheckModelRunningApi(client, request.Model))
            {
                await hubContext.Clients.All.SendAsync("ChatMessageReceived", $"모델 {request.Model}을 시작할 수 없습니다.");
                return Results.BadRequest(new { success = false });
            }
        }

        // 채팅 요청
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

        var errorMessage = $"API 오류: {response.StatusCode}";
        await hubContext.Clients.All.SendAsync("ChatMessageReceived", errorMessage);
        return Results.BadRequest(new { success = false, error = errorMessage });
    }
    catch (Exception ex)
    {
        var errorMessage = $"채팅 중 오류: {ex.Message}";
        await hubContext.Clients.All.SendAsync("ChatMessageReceived", errorMessage);
        return Results.Problem(errorMessage);
    }
});

app.Run();

// API를 사용하여 모델 실행 상태 확인
static async Task<bool> CheckModelRunningApi(HttpClient client, string modelName)
{
    try
    {
        var response = await client.GetAsync("api/ps");
        if (!response.IsSuccessStatusCode) return false;

        var content = await response.Content.ReadAsStringAsync();
        var runningData = JsonSerializer.Deserialize<RunningModelsResponse>(content);

        return runningData?.models?.Any(m => m.name == modelName) ?? false;
    }
    catch
    {
        return false;
    }
}

// 바이트를 읽기 쉬운 형태로 변환
static string FormatBytes(long bytes)
{
    string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
    int counter = 0;
    decimal number = bytes;
    while (Math.Round(number / 1024) >= 1)
    {
        number /= 1024;
        counter++;
    }
    return $"{number:n1} {suffixes[counter]}";
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Dictionary<string, string> _lastStatus = new();

    public ModelMonitorService(IHubContext<ModelHub> hubContext, IHttpClientFactory httpClientFactory)
    {
        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var currentStatus = await GetModelStatusApi();

                foreach (var (modelName, status) in currentStatus)
                {
                    if (!_lastStatus.TryGetValue(modelName, out var prevStatus) || prevStatus != status)
                    {
                        _lastStatus[modelName] = status;
                        await _hubContext.Clients.Group("ModelUpdates").SendAsync("ModelStatusChanged", modelName, status, stoppingToken);
                    }
                }

                // 제거된 모델들 정리
                foreach (var removed in _lastStatus.Keys.Except(currentStatus.Keys).ToList())
                    _lastStatus.Remove(removed);
            }
            catch (Exception ex)
            {
                // 로깅 가능
                Console.WriteLine($"모델 상태 모니터링 오류: {ex.Message}");
            }

            await Task.Delay(3000, stoppingToken);
        }
    }

    private async Task<Dictionary<string, string>> GetModelStatusApi()
    {
        var result = new Dictionary<string, string>();
        var client = _httpClientFactory.CreateClient("Ollama");

        try
        {
            // 실행 중인 모델들 가져오기
            var runningResponse = await client.GetAsync("api/ps");
            var runningModelNames = new HashSet<string>();

            if (runningResponse.IsSuccessStatusCode)
            {
                var runningContent = await runningResponse.Content.ReadAsStringAsync();
                var runningData = JsonSerializer.Deserialize<RunningModelsResponse>(runningContent);
                runningModelNames = runningData?.models?.Select(m => m.name).ToHashSet() ?? new HashSet<string>();
            }

            // 모든 설치된 모델들 가져오기
            var tagsResponse = await client.GetAsync("api/tags");
            if (tagsResponse.IsSuccessStatusCode)
            {
                var tagsContent = await tagsResponse.Content.ReadAsStringAsync();
                var tagsData = JsonSerializer.Deserialize<TagsResponse>(tagsContent);

                if (tagsData?.models != null)
                {
                    foreach (var model in tagsData.models)
                    {
                        result[model.name] = runningModelNames.Contains(model.name) ? "Running" : "Stopped";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API를 통한 모델 상태 확인 실패: {ex.Message}");
        }

        return result;
    }
}

// API 응답 모델들
public class TagsResponse
{
    public List<ModelInfo>? models { get; set; }
}

public class RunningModelsResponse
{
    public List<RunningModelInfo>? models { get; set; }
}

public class ModelInfo
{
    public string name { get; set; } = "";
    public DateTime? modified_at { get; set; }
    public long size { get; set; }
    public string digest { get; set; } = "";
}

public class RunningModelInfo
{
    public string name { get; set; } = "";
    public string model { get; set; } = "";
    public long size { get; set; }
    public DateTime? expires_at { get; set; }
    public long size_vram { get; set; }
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