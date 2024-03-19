using Yoo.Trainees.ShipWars.Common.Enums;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IBotLogic
    {
        void CreateAndSaveBotShipPositions(Guid gamePlayerId);
        void SaveShipPositionsInBotGame(SaveShipsDto SwaggerData);
        bool IsBotLobby(Guid gameId);
        SaveShotsDto BotRandomShotPosition(Guid gamePlayerId);
        ShipHit CheckIfBotHit(SaveShotsDto xy, Guid gamePlayerId);
        List<SaveShotsDto> GetAllBotShots(Guid gamePlayerId, Guid gameId);
        Game? GetGame(Guid gamePlayerId);
        Guid GetBotGamePlayerId(Guid gamePlayerId);
        GameMode GetGameMode(Guid gameId);
        SaveShotsDto HardGameMode(Guid gamePlayerId);
        ShipHit GetShipHit(SaveShotsDto xy, Guid gamePlayerId, Guid gameId);
    }
}
