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
        // get gamePlayer from the DB with the same ID like the ${user}
        var gamePlayer = (from gp in _applicationDbContext.GamePlayer
                          where gp.Id == user
                          select gp).SingleOrDefault();

        // get player from gamePlayer
        var player = (from p in _applicationDbContext.Player
                      where p.Id == gamePlayer.PlayerId
                      select p).SingleOrDefault();

        // create new Message in DB
        var messageDB = new Message() { 
            Id = Guid.NewGuid(),
            Text = message,
            Date = DateTime.Now,
            GamePlayers = gamePlayer
        };

        // send Message with ReceiveMessage Method
        await Clients.Group(gamePlayer.GameId.ToString()).SendAsync("ReceiveMessage", player.Name, messageDB.Text, messageDB.Date);

        // add Message and save changes
        _applicationDbContext.Message.Add(messageDB);
        await _applicationDbContext.SaveChangesAsync();
    }
}

