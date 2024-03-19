using Yoo.Trainees.ShipWars.DataBase.Entities;
using Yoo.Trainees.ShipWars.Common.Enums;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IGameLogic
    {
        Game CreateGame(string name, bool bot, bool easyGame);
        void CreateBoard(SaveShipsDto Ships);
        bool IsReady(Guid gameId);
        ShipPositionDto[] GetCompleteShipPositionsForGamePlayer(Guid gamePlayerId);
        bool UpdateAndCheckNextPlayer(Guid gameId, Guid playerId);
        void VerifyAndSaveShot(SaveShotsDto xy, Guid gamePlayerId);
        void SaveShot(SaveShotsDto shot, Guid gamePlayerId);
        List<SaveShotsDto> ShotsAll(Guid gamePlayerId);
        List<SaveShotsDto> GetAllShotsOfOpponent(Guid gamePlayerId);
        void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId);
        RockPaperScissorsState GetResultOfTheSRP(Guid gamePlayerId);
        ShipHit CheckIfShipHit(SaveShotsDto xy, Guid gamePlayerId);
        public ShotInfoDto CountShotsInDB(Guid gamePlayerId);
        GameState GetGameState(Guid gamePlayerId);
        List<SaveShotsDto> GetAllHitShipFields(Guid gamePlayerId);
        List<MessageDto> GetAllMessages(Guid gameId);
        String GetCurrentUser(Guid gamePlayerId);
    }
}
