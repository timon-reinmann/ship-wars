using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Game_Status { get; set; }
        //smalldatetime => 1900-01-01 00:00:00 bis 2079-06-06 23:59:59
        [Column(TypeName = "smalldatetime")]
        public DateTime Date { get; set; }

        public Game(Guid id, string name, string game_Status)
        {
            Id = id;
            Name = name;
            Game_Status = game_Status;
            Date = DateTime.Now;
        }
    }
}