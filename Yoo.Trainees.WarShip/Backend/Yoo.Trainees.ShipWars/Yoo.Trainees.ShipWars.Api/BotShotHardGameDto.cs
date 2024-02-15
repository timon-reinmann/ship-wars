using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api
{
    public class BotShotHardGameDto
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ShipHit Hit { get; set; }
        public Step Step { get; set; }
        public bool MainShot { get; set; }
        public int hitShotsCounter { get; set; }
    }
}
