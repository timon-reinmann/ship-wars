using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string GameStatus { get; set; }

        public Guid? NextPlayer {  get; set; }

        public bool IsBotGame { get; set; }

        //smalldatetime => 1900-01-01 00:00:00 bis 2079-06-06 23:59:59
        [Column(TypeName = "smalldatetime")]
        public DateTime Date { get; set; }

        public ICollection<GamePlayer> GamePlayers { get; set; }

    }
}