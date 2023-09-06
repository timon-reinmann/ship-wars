using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public class GameLogic : IGameLogic
    {
        private readonly ApplicationDbContext applicationDbContext;

        public GameLogic(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public Game CreateGame(string name)
        {
            var Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();

            var game = new Game(Id, player2Id, name);

            applicationDbContext.Games.Add(game);
            applicationDbContext.SaveChanges();

            return game;
        }
    }
}
