namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Ship
    {
        public Guid Id { get; set; }

        public int Length { get; set; }

        public string Name { get; set; }

        public ICollection<ShipPosition> Positions { get; set; }
    }
}
