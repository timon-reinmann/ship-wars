using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    internal class Message
    {
        public Guid Id { get; set; }

        public string text { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Date { get; set; }

        public GamePlayer GamePlayers { get; set; }

    }
}
