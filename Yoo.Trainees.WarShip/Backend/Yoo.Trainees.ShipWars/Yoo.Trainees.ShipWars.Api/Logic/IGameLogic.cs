﻿using Microsoft.AspNetCore.Mvc;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IGameLogic
    {
        Game CreateGame(string name);
        void CreateBoard(SaveShipsDto Ships);
        bool IsReady(Guid gameId);
        BoardStateDto[] IsComplete(Guid gamePlayerId);
        bool CheckShots(Guid gameId, Guid playerId);
        void VerifyAndExecuteShotOrThrow(SaveShotsDto xy, Guid gamePlayerId);
        void SaveShot(SaveShotsDto shot, Guid gamePlayerId);
        List<SaveShotsDto> ShotsAll(Guid gamePlayerId);
        List<SaveShotsDto> ShotsAllOpponent(Guid gamePlayerId);
        void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId);
        SRPStatus GetResultOfTheSRP(Guid gamePlayerId);
        bool CheckIfPlayer1IsLoser(GamePlayer player1, GamePlayer player2);
    }
}
