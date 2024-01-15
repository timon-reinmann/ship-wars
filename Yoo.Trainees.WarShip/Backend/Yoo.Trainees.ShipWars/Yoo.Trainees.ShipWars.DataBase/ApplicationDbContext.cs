using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.DataBase.Entities;
using static System.Net.WebRequestMethods;

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
        public virtual DbSet<StartShotHumanLogic> StartShotHumanLogic { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ship>().HasData(
            new Ship
            {
                Id = new Guid("45652d4e-10be-4b26-a8a8-c6de9a4e49af"),
                Length = 1,
                Name = "Submarine"
            }, new Ship
            {
                Id = new Guid("a735b8ec-c868-45fa-81bc-942eda165a8f"),
                Length = 2,
                Name = "Destroyer"
            }, new Ship
            {
                Id = new Guid("985497b3-353c-49d3-97a0-79232a270da0"),
                Length = 3,
                Name = "Cruiser"
            }, new Ship
            {
                Id = new Guid("eee6167b-6588-42d9-b657-7698a2f5ca40"),
                Length = 4,
                Name = "Warship"
            });
        }
    }
}
