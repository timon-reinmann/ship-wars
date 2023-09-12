using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public player(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}