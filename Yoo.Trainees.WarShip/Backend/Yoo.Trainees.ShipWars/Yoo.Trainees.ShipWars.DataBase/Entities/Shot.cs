namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Shot
    {
        public Guid Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public GamePlayer Player { get; set; }
    }
}
