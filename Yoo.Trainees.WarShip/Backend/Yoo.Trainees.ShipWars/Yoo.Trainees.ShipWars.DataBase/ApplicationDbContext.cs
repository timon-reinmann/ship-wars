using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<GamePlayer> GamePlayer { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Ship> Ship { get; set; }
        public virtual DbSet<ShipPosition> ShipPosition { get; set; }
        public virtual DbSet<Shot> Shot { get; set; }

    }
}
