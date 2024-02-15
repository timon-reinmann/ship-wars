
using System.Diagnostics;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public enum Step
    {
        Random,
        ShootAround,
        ShootInThisDirection,
        ShootInTheOtherDirection
    }

    public enum Navigation
    {
        top,
        bottom,
        left,
        right,
        none
    }

    public enum ShipHit
    {
        Missed,
        Hit
    }
    public class HardGameShot
    {
        public Guid Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ShipHit Hit { get; set; }
        public Step Step { get; set; }
        public bool MainShot { get; set; }
        public Navigation Direction { get; set; }
        public GamePlayer Player { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HitShotsCounter { get; set; }
    }
}
