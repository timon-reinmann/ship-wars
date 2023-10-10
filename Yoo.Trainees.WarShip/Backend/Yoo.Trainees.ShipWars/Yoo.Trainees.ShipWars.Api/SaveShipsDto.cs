namespace Yoo.Trainees.ShipWars.Api
{
    public class SaveShipsDto
    {
        public Guid GamePlayerId { get; set; }

        public SaveShipDto[] Ships { get; set; }
    } 
}
