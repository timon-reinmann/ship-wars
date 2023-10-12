using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api
{
    public class BoardStateDto
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Direction Direction { get; set; }

        public string Name { get; set; }

    }
}
