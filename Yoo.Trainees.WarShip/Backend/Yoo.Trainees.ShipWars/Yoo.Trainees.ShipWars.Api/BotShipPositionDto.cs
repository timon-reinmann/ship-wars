namespace Yoo.Trainees.ShipWars.Api
{
    public class BotShipPositionDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }
        public string ShipType { get; set; }
        public Guid Id { get; set; }

    }
}   

