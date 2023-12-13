using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IBotLogic
    {
        SaveShipDto[] GetBotShipPositions(Guid gamePlayerId);
        void SaveShipPositions(SaveShipsDto SwaggerData);
        bool IsBotLobby(Guid gameId);
    }
}
