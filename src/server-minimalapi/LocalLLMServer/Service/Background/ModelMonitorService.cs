using LocalLLMServer.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace LocalLLMServer.Service.Background;
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
