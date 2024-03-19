using Yoo.Trainees.ShipWars.Common.Enums;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IVerificationLogic
    {
        bool VerifyEverything(SaveShipDto[] shipDtos, bool isBotLobby);
        bool VerifyShipLocations(SaveShipDto[] shipDtos, bool isBotLobby);
        bool TestVerifyeToManyShipsFromSameType(SaveShipDto[] shipDtos);
        bool VerifyShot(List<SaveShotsDto> shotsDto, SaveShotsDto shot);
        SaveShipDto VerifyShipHit(List<SaveShipDto> shipsDB, SaveShotsDto shot);
        BotResponse VerifyBotShot(IList<SaveShotsDto> shots, SaveShotsDto lastShot, Guid gamePlayerId);
    }
}
