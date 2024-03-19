using Yoo.Trainees.ShipWars.Common.Enums;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{

    public class ShipPosition
    {
        public Guid Id { get; set; }

        public Guid GamePlayerId { get; set; }

        public Guid ShipId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public Direction Direction { get; set; }

        public int Life { get; set; }

        public Ship Ship { get; set; }

        public GamePlayer GamePlayer { get; set; }

        public bool IsHuman { get; set; }
    }
}
