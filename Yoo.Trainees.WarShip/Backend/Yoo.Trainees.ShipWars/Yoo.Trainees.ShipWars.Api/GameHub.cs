using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api;

public sealed class ChatHub : Hub
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IGameLogic _gameLogic;
    public ChatHub(ApplicationDbContext applicationDbContext, IGameLogic gameLogic)
    {
        _applicationDbContext = applicationDbContext;
        _gameLogic = gameLogic;
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
        var messageDB = new Message()
        {
            Id = Guid.NewGuid(),
            Text = message,
            Date = DateTime.Now,
            GamePlayers = gamePlayer
        };

        // send Message with ReceiveMessage Method
        await Clients.Group(GetGameId(user).ToString()).SendAsync("ReceiveMessage", player.Name, messageDB.Text, messageDB.Date);

        // add Message and save changes
        _applicationDbContext.Message.Add(messageDB);
        await _applicationDbContext.SaveChangesAsync();
    }

    // To visualize the own Fields which got hit.
    [HttpGet("{gamePlayerId}/LoadShotsFromOpponent")]
    public async Task LoadShotsFromOpponent(Guid gamePlayerId)
    {
        var gameId = await GetGameId(gamePlayerId);
        var opponent = await GetOpponent(gamePlayerId, gameId);
        var shots = _gameLogic.GetAllShotsOfOpponent(gamePlayerId);

        await Clients.Groups(opponent.Id.ToString()).SendAsync("LoadShotsFromOpponent", shots);
    }

    // Count ALL shots for the counter and also give information for the nextplayer and game state (Ongoing, Lost, Won, Prep, Complete).
    [HttpGet("{gamePlayerId}/CountShots")]
    public async Task CountShots(Guid gamePlayerId)
    {
        ShotInfoDto countAndNextPlayer = _gameLogic.CountShotsInDB(gamePlayerId);
        var gameStateDB = _gameLogic.GetGameState(gamePlayerId);
        var gameId = await GetGameId(gamePlayerId);
        var opponent = await GetOpponent(gamePlayerId, gameId);

        await Clients.Group(opponent.ToString()).SendAsync("CountShots", new { shots = countAndNextPlayer.ShotCount, nextPlayer = countAndNextPlayer.IsNextPlayer, gameState = gameStateDB });
    }

    private async Task<GamePlayer?> GetOpponent(Guid gamePlayerId, Guid gameId)
    {
        return await (from gp in _applicationDbContext.GamePlayer where gp.GameId == gameId && gp.Id != gamePlayerId select gp).SingleOrDefaultAsync();
    }

    private async Task<Guid> GetGameId(Guid gamePlayerId)
    {
        return await (from gp in _applicationDbContext.GamePlayer where gp.Id.Equals(gamePlayerId) select gp.GameId).SingleOrDefaultAsync();
    }
}