using System.ComponentModel.DataAnnotations.Schema;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api
{
    public class MessageDto
    {
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public String User { get; set; }
    }
}
