namespace Yoo.Trainees.ShipWars.Api
{
    public class BotShipPositionsDto
    {
        public Guid GameId { get; set; }
        public Guid GamePlayerId { get; set; }

        public BotShipPositionDto[] Ships { get; set; }
    }
}
