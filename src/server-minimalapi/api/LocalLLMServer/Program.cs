using LocalLLMServer.Endpoint;
using LocalLLMServer.Service.Background;
using LocalLLMServer.SignalRHub;


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

app.AddModelEndPoints();
app.AddChatEndPoints();

app.Run();