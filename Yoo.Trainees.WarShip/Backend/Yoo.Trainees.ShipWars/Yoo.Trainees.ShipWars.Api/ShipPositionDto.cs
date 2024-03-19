using Yoo.Trainees.ShipWars.Common.Enums;
using Yoo.Trainees.ShipWars.DataBase.Entities;
namespace Yoo.Trainees.ShipWars.Api
{
    public class ShipPositionDto
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Direction Direction { get; set; }

        public string Name { get; set; }

    }
}
