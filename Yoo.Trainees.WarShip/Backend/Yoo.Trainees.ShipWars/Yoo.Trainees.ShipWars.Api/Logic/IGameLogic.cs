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
        void VerifyAndExecuteShotOrThrow(String[] xy, Guid gamePlayerId);
        void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId);
        bool GetResultOfTheSRP(Guid gamePlayerId);
    }
}
