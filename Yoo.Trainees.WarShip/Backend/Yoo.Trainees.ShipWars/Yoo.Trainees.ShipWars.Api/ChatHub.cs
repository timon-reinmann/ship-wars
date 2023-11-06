using Microsoft.AspNetCore.SignalR;

namespace Yoo.Trainees.ShipWars.Api;

public sealed class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
         await Clients.All.SendAsync($"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

