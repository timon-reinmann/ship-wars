using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class GamePlayer
    {
        [Key]
        public Guid Id { get; set; } 

        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }

        // Navigation properties
        public Game Game { get; set; }
        public Player Player { get; set; }

    }
}
