using Microsoft.AspNetCore.SignalR;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api;

public sealed class ChatHub : Hub
{
    private readonly ApplicationDbContext _applicationDbContext;
    public ChatHub(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync($"{Context.ConnectionId} has joined");
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync($"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(Guid user, string message)
    {
        var gamePlayer = (from gp in _applicationDbContext.GamePlayer
                          where gp.Id == user
                          select gp).SingleOrDefault();
        var player = (from p in _applicationDbContext.Player
                      where p.Id == gamePlayer.PlayerId
                      select p).SingleOrDefault();
        await Clients.Group(gamePlayer.GameId.ToString()).SendAsync("ReceiveMessage", player.Name, message);
        var messageDB = new Message() { 
            Id = Guid.NewGuid(),
            Text = message,
            Date = DateTime.Now,
            GamePlayers = gamePlayer
        };
        _applicationDbContext.Message.Add(messageDB);
        await _applicationDbContext.SaveChangesAsync();
    }
}

