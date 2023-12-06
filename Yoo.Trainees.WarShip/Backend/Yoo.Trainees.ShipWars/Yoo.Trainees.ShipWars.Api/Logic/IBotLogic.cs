namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IBotLogic
    {
        SaveShipDto[] GetBotShipPositions(Guid gamePlayerId);
        SaveShipDto[] SaveShipPositions(SaveShipsDto SwaggerData);
    }
}
