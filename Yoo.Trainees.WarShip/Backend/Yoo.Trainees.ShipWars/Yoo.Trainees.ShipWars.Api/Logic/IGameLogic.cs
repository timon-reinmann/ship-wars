﻿using Microsoft.AspNetCore.Mvc;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IGameLogic
    {
        Game CreateGame(string name);
        void CreateBoard(SaveShipsDto Ships);
        bool IsReady(Guid gameId);
        ShipPositionDto[] GetCompleteShipPositionsForGamePlayer(Guid gamePlayerId);
        bool UpdateAndCheckNextPlayer(Guid gameId, Guid playerId);
        void VerifyAndExecuteShotOrThrow(SaveShotsDto xy, Guid gamePlayerId);
        void SaveShot(SaveShotsDto shot, Guid gamePlayerId);
        List<SaveShotsDto> ShotsAll(Guid gamePlayerId);
        List<SaveShotsDto> GetAllShotsOfOpponent(Guid gamePlayerId);
        void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId);
        RockPaperScissorsState GetResultOfTheSRP(Guid gamePlayerId);
        bool CheckIfPlayer1IsLoser(GamePlayer player1, GamePlayer player2);
        ShipHit CheckIfShipHit(SaveShotsDto xy, Guid gamePlayerId);
        public ShotInfoDto CountShotsInDB(Guid gamePlayerId);
        GameState GetGameState(Guid gamePlayerId);
        bool CheckIfShipsThere(List<ShipPosition> ships);
    }
}
