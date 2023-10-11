namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public enum Direction
    {
        horizontal,
        vertical
    }
    public class ShipPosition
    {
        public Guid Id { get; set; }

        public Guid GamePlayerId { get; set; }

        public Guid ShipId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Direction Direction { get; set; }

        public Ship Ship { get; set; }

        public GamePlayer GamePlayer { get; set; }
    }
}
