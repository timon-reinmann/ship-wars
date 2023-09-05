﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Game
    { 
        public Guid Id { get; set; }
        public Guid Player2Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
