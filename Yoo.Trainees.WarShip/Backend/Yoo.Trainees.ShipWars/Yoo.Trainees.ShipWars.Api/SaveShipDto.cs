namespace Yoo.Trainees.ShipWars.Api
{
    public class SaveShipDto
    {
        public int X { get; set; }

        public int Y { get; set; }

        public bool Direction { get; set; }

        public Guid ShipId { get; set; }
    }
}
