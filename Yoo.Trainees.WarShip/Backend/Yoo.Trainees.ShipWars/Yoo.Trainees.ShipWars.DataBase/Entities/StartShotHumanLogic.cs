namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class StartShotHumanLogic
    {
        public Guid Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Hit { get; set; }
        public bool IsFirstShot { get; set; }
    }
}
