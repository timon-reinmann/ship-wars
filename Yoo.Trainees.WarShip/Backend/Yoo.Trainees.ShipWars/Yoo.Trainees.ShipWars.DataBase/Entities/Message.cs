using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }

        public GamePlayer GamePlayers { get; set; }

    }
}
