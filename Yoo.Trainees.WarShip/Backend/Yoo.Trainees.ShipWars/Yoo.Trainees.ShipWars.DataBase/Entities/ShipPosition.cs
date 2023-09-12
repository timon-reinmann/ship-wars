using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    internal class ShipPosition
    {
        public Guid Id { get; set; }

        public Ship ShipId { get; set; }

        public int Life { get; set; }       

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Boolean Direction { get; set; }
    }
}
