using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IBotLogic
    {
        void CreateAndSaveBotShipPositions(Guid gamePlayerId);
        void SaveShipPositionsInBotGame(SaveShipsDto SwaggerData);
        bool IsBotLobby(Guid gameId);
        SaveBotShotsDto BotShotPosition(Guid gamePlayerId);
        ShipHit CheckIfBotHit(SaveShotsDto xy, Guid gamePlayerId);
        List<SaveBotShotsDto> GetAllBotShots(Guid gamePlayerId, Guid gameId);
        Game? GetGame(Guid gamePlayerId);
        Guid GetBotGamePlayerId(Guid gamePlayerId);
    }
}
