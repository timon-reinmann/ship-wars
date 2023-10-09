using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
