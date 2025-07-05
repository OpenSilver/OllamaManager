using LocalLLMServer.Endpoint.Dto;
using LocalLLMServer.Model.Entity;
using LocalLLMServer.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text.Json;

namespace LocalLLMServer.Endpoint;

public static class EndPointExtension
{
    public static WebApplication AddChatEndPoints(this WebApplication app)
    {
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

        return app;

    }

    public static WebApplication AddModelEndPoints(this WebApplication app)
    {
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

        

        return app;
    }




    private static async Task<bool> CheckModelRunning(string modelName)
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

    private static List<OllamaModel> ParseOllamaModels(string listOutput, string psOutput)
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

}
