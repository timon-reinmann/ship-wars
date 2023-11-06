using System.ComponentModel.DataAnnotations;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{

    public enum ScissorsRockPaper
    {
        Scissors,
        Rock,
        Paper
    }
    public class GamePlayer
    {
        [Key]
        public Guid Id { get; set; } 

        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }

        // Navigation properties
        public Game Game { get; set; }
        public Player Player { get; set; }
        public ScissorsRockPaper? ScissorsRockPaperBet { get; set; }
    }
}
