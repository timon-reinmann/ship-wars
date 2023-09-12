
namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    internal class Shot
    {
        public Guid Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Result { get; set; }

        public GamePlayer Player { get; set; }
    }
}
