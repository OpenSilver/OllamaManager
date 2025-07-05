using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
namespace LocalLLMServer.SignalRHub;

public class ModelHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "ModelUpdates");
        await base.OnConnectedAsync();
    }
}
