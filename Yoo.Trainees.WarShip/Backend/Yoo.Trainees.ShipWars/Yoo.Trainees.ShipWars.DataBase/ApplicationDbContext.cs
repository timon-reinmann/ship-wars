﻿using Microsoft.EntityFrameworkCore;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<GamePlayer> GamePlayers { get; set; }
        public virtual DbSet<Player> Players { get; set; }
    }
}
