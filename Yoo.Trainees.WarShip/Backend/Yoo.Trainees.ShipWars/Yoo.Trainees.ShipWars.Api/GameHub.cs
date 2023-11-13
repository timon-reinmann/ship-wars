using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api;

public sealed class GameHub : Hub
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IGameLogic _gameLogic;
    public GameHub(ApplicationDbContext applicationDbContext, IGameLogic gameLogic)
    {
        _applicationDbContext = applicationDbContext;
        _gameLogic = gameLogic;
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync($"{Context.ConnectionId} has joined " + groupName);
    }

    public async Task EgoGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync($"{Context.ConnectionId} has joined " + groupName);
    }

    // To visualize the own Fields which got hit.
    public async Task LoadShotsFromOpponent(Guid gamePlayerId)
    {
        var gameId = await GetGameId(gamePlayerId);
        var opponent = await GetOpponent(gamePlayerId, gameId);
        var shots = _gameLogic.GetAllShotsOfOpponent(opponent.Id);

        await Clients.Group(opponent.Id.ToString()).SendAsync("LoadShotsFromOpponent", shots);
    }

    // Count ALL shots for the counter and also give information for the nextplayer and game state (Ongoing, Lost, Won, Prep, Complete).
    public async Task CountShots(Guid gamePlayerId)
    {
        var gameStateDB = _gameLogic.GetGameState(gamePlayerId);
        var gameId = await GetGameId(gamePlayerId);
        ShotInfoDto countAndNextPlayer = _gameLogic.CountShotsInDB(gamePlayerId);

        await Clients.Group(gameId.ToString()).SendAsync("CountShots", countAndNextPlayer.ShotCount, countAndNextPlayer.IsNextPlayer, gameStateDB);
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