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
            var gamePlayers = new List<GamePlayer>();
            var player1 = new Player
            {
                Id = Guid.NewGuid(), 
                Name = "Hans"
            };
            var player2 = new Player
            {
                Id = Guid.NewGuid(),
                Name = "Peter"
            };
            var gameId = Guid.NewGuid();

            gamePlayers.Add(new GamePlayer
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                PlayerId = player1.Id
            });
            gamePlayers.Add(new GamePlayer
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                PlayerId = player2.Id
            });

            var game = new Game
            {
                Id = gameId,
                Name = name,
                GameStatus = "TODO",
                GamePlayers = gamePlayers,
                Date = DateTime.Now
            };


            applicationDbContext.Player.Add(player1);
            applicationDbContext.Player.Add(player2);
            applicationDbContext.Game.Add(game);
            applicationDbContext.SaveChanges();

            return game;
        }
        public async void CreateBoard(SaveShipsDto SwaggerData)
        {
            for(var i = 0;i < SwaggerData.Ships.Length; i++)
            {
                var Ship = SwaggerData.Ships[i];
                var shipType = applicationDbContext.Ship.Where(ship => ship.Name == Ship.ShipType).SingleOrDefault();
                var shipPositio = new ShipPosition
                {
                    Id = Guid.NewGuid(),
                    GamePlayerId = Guid.Parse(SwaggerData.GamePlayerId.ToString()),
                    ShipId = Guid.Parse(shipType.Id.ToString()),
                    _direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)Ship._direction, // Weirde Fehler aber me hets müesse Caste well es die gliche enums sie müesse
                    X = Ship.X,                                                                      // Und well beides in anderne Files gmacht worde isch es genau gseh nid sgliche :(    
                    Y = Ship.Y                  
                };

                 Console.WriteLine(shipPositio._direction);
                
             applicationDbContext.ShipPosition.Add(shipPositio);
            }
            applicationDbContext.SaveChanges();
        }
    }
}
