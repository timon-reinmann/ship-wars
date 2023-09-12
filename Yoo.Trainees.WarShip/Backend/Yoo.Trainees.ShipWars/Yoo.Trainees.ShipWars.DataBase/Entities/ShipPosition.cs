using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class ShipPosition
    {
        public Guid Id { get; set; }
        
        public Guid GamePlayerId { get; set; }

        public Guid ShipId { get; set; }

        public int Life { get; set; }       

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Boolean Direction { get; set; }

        public Ship Ship { get; set; }

        public GamePlayer GamePlayer { get; set; }
    }
}
