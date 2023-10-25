using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;


namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public enum RockPaperScissorsState
    {
        Waiting,
        Lost,
        Won,
        Draw,
        Redo
    }
    public enum ShipHit
    {
        Missed,
        Hit
    }

    public enum GameState
    {
        Ongoing,
        Lost,
        Won,
        Prep,
        Complete
    }
    public class GameLogic : IGameLogic
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private IVerificationLogic _verificationLogic;
        private Game _Game;

        public GameLogic(ApplicationDbContext applicationDbContext, IVerificationLogic verificationLogic)
        {
            this._applicationDbContext = applicationDbContext;
            this._verificationLogic = verificationLogic;
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

            this._Game = new Game
            {
                Id = gameId,
                Name = name,
                GameStatus = GameState.Prep.ToString(),
                GamePlayers = gamePlayers,
                Date = DateTime.Now
            };


            _applicationDbContext.Player.Add(player1);
            _applicationDbContext.Player.Add(player2);
            _applicationDbContext.Game.Add(this._Game);
            _applicationDbContext.SaveChanges();

            return this._Game;
        }
        public void CreateBoard(SaveShipsDto SwaggerData)
        {
            var game = _applicationDbContext.Game.Find(SwaggerData.GameId);
            if (game == null)
            {
                throw new Exception("_Game not found");
            }
            Guid id = new Guid();
            for (var i = 0; i < SwaggerData.Ships.Length; i++)
            {
                var Ship = SwaggerData.Ships[i];
                var shipType = _applicationDbContext.Ship.Where(ship => ship.Name == Ship.ShipType).SingleOrDefault();
                var shipPositio = new ShipPosition
                {
                    Id = Guid.NewGuid(),
                    GamePlayerId = Guid.Parse(SwaggerData.GamePlayerId.ToString()),
                    ShipId = Guid.Parse(shipType.Id.ToString()),                                      
                    X = Ship.X,                                                                  
                    Y = Ship.Y,
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)Ship.Direction,
                    Life = shipType.Length
                };
                id = shipPositio.GamePlayerId;

                _applicationDbContext.ShipPosition.Add(shipPositio);
            }
            _applicationDbContext.SaveChanges();
        }
        public bool IsReady(Guid gameId)
        { 
            var gamePlayers = from s in _applicationDbContext.GamePlayer
                      where s.GameId == gameId
                      select s;
            var gamePlayer1 = from s in _applicationDbContext.ShipPosition
                                   where s.GamePlayerId == gamePlayers.First().Id
                                   select s;
            var gamePlayer2 = from s in _applicationDbContext.ShipPosition
                              where s.GamePlayerId == gamePlayers.ToArray()[1].Id
                              select s;

            var count = gamePlayer1.Count() + gamePlayer2.Count();
            
            return count == int.Parse(_configuration["Ships:MaxShips"]);
        }
        public ShipPositionDto[] GetCompleteShipPositionsForGamePlayer(Guid gamePlayerId)
        {
            var gamePlayer = from sp in _applicationDbContext.ShipPosition
                             join g in _applicationDbContext.Ship on sp.ShipId equals g.Id
                             where sp.GamePlayerId.Equals(gamePlayerId)
                             select new ShipPositionDto { X = sp.X, Y = sp.Y, Direction = (Yoo.Trainees.ShipWars.Api.Direction)sp.Direction, Name = g.Name };
            if (gamePlayer.Count() == 10)
                return gamePlayer.ToArray();
            return null;
        }
        public bool UpdateAndCheckNextPlayer(Guid gameId, Guid gamePlayerId)
        {
            var game = (from g in _applicationDbContext.Game
                        where g.Id.Equals(gameId)
                        select g).SingleOrDefault();
            Guid? nextPlayerId = game.NextPlayer;

            if(nextPlayerId == gamePlayerId)
            {
                var nextPlayer = (from gp in _applicationDbContext.GamePlayer
                                  where gp.Id != nextPlayerId && gp.GameId.Equals(gameId)
                                  select gp.Id).SingleOrDefault();
                game.NextPlayer = nextPlayer;
                _applicationDbContext.Game.Update(game);
                _applicationDbContext.SaveChanges();
            }
            return nextPlayerId == gamePlayerId;
        }

        public void VerifyAndSaveShot(SaveShotsDto xy, Guid gamePlayerId)
        {
            var game = (from gp in _applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.Game).FirstOrDefault();
            SaveShotsDto shot = new SaveShotsDto { X = xy.X, Y = xy.Y};
            var shots = (from s in _applicationDbContext.Shot
                        where s.Player.Id == gamePlayerId
                        select new SaveShotsDto { X = s.X, Y = s.Y })
                        .ToList();

            if (!_verificationLogic.VerifyShot(shots, shot))
            {
                game.NextPlayer = gamePlayerId;
                _applicationDbContext.Game.Update(game);
                _applicationDbContext.SaveChanges();
                throw new InvalidOperationException("Shot not valid");
            }
        }
        public void SaveShot(SaveShotsDto shot, Guid gamePlayerId)
        {
            var player = _applicationDbContext.GamePlayer
                .FirstOrDefault(x => x.Id == gamePlayerId)?.Player;

            var gamePlayer = (from gp in _applicationDbContext.GamePlayer
                           where gp.Id == gamePlayerId
                           select gp).FirstOrDefault();
            var shotToSave = new Shot
            {
                Id = Guid.NewGuid(),
                X = shot.X,
                Y = shot.Y,
                Player = gamePlayer
            };
            _applicationDbContext.Shot.Add(shotToSave);
            _applicationDbContext.SaveChanges();
        }
        public List<SaveShotsDto> GetAllShotsOfOpponent(Guid gamePlayerId)
        {
            var game = (from gp in _applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.GameId).SingleOrDefault();
            var player2 = (from gp in _applicationDbContext.GamePlayer
                           where gp.GameId.Equals(game) && gp.Id != gamePlayerId
                           select gp).SingleOrDefault(); 
            var shots = (from s in _applicationDbContext.Shot
                         where s.Player.Id.Equals(player2.Id) 
                         select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
            return shots;
        }

        public List<SaveShotsDto> ShotsAll(Guid gamePlayerId)
        {
            var shots = (from s in _applicationDbContext.Shot
                         where s.Player.Id.Equals(gamePlayerId)
                         select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
            return shots;
        }
        public void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            var gamePlayer = _applicationDbContext.GamePlayer.FirstOrDefault(x => x.Id == gamePlayerId);

            if (gamePlayer != null) {
                gamePlayer.ScissorsRockPaperBet = scissorsRockPaperBet;
                _applicationDbContext.GamePlayer.Update(gamePlayer);
                _applicationDbContext.SaveChanges();
            }
        }
        public RockPaperScissorsState GetResultOfTheSRP(Guid gamePlayerId)
        {
            var gameId = (from gp in _applicationDbContext.GamePlayer
                          where gp.Id.Equals(gamePlayerId)
                          select gp.GameId).FirstOrDefault();
            var player1 = (from gp in _applicationDbContext.GamePlayer
                           where gp.Id == gamePlayerId
                           select gp).SingleOrDefault();
            var player2 = (from gp in _applicationDbContext.GamePlayer
                           where gp.Id != gamePlayerId && gp.GameId.Equals(gameId)
                           select gp).FirstOrDefault();

            var game = (from g in _applicationDbContext.Game
                        where g.Id.Equals(gameId)
                        select g).SingleOrDefault();

            if (player1.ScissorsRockPaperBet == null) return RockPaperScissorsState.Redo;
            if (player2.ScissorsRockPaperBet == null || game == null) return RockPaperScissorsState.Waiting;
            if (player1.ScissorsRockPaperBet == player2.ScissorsRockPaperBet) 
            {
                player1.ScissorsRockPaperBet = null;
                player2.ScissorsRockPaperBet = null;
                _applicationDbContext.GamePlayer.Update(player1);
                _applicationDbContext.GamePlayer.Update(player2);
                _applicationDbContext.SaveChanges();
                return RockPaperScissorsState.Draw; 
            }

            bool isPlayer1Loser = CheckIfPlayer1IsLoser(player1, player2);

            game.NextPlayer = isPlayer1Loser ? player2.Id : player1.Id;

            _applicationDbContext.Game.Update(game);
            _applicationDbContext.SaveChanges();

            return isPlayer1Loser ? RockPaperScissorsState.Lost : RockPaperScissorsState.Won;
        }
        private static bool CheckIfPlayer1IsLoser(GamePlayer player1, GamePlayer player2)
        {
            return (player1.ScissorsRockPaperBet == ScissorsRockPaper.Scissors && player2.ScissorsRockPaperBet == ScissorsRockPaper.Rock) ||
                   (player1.ScissorsRockPaperBet == ScissorsRockPaper.Rock && player2.ScissorsRockPaperBet == ScissorsRockPaper.Paper) ||
                   (player1.ScissorsRockPaperBet == ScissorsRockPaper.Paper && player2.ScissorsRockPaperBet == ScissorsRockPaper.Scissors);
        }
        public ShipHit CheckIfShipHit(SaveShotsDto xy, Guid gamePlayerId)
        {
            var game = (from gp in _applicationDbContext.GamePlayer
                        where gp.Id == gamePlayerId
                        select gp.Game).FirstOrDefault();

            var player2 = (from gp in _applicationDbContext.GamePlayer
                           where gp.Id != gamePlayerId && gp.GameId.Equals(game.Id)
                           select gp).FirstOrDefault();

            this._verificationLogic = new VerificationLogic();
            var ships = (from sp in _applicationDbContext.ShipPosition
                         where sp.GamePlayer.Id.Equals(player2.Id)
                         select new SaveShipDto { Direction = (Yoo.Trainees.ShipWars.Api.Direction)sp.Direction, Id = sp.Id, ShipType = sp.Ship.Name ,
                                                    X = sp.X, Y = sp.Y}).ToList();

            var ship = _verificationLogic.VerifyShipHit(ships, xy);
            if(ship == null)
            {
                return ShipHit.Missed;
            }
            var shipDB = (from sp in _applicationDbContext.ShipPosition
                          where sp.Id.Equals(ship.Id)
                          select sp).FirstOrDefault();
            shipDB.Life--;
            _applicationDbContext.ShipPosition.Update(shipDB);
            _applicationDbContext.SaveChanges();
            return ShipHit.Hit;
        }
        
        public ShotInfoDto CountShotsInDB(Guid gamePlayerId)
        {
            var game = (from gp in _applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.Game).SingleOrDefault();
            var count = (from gp in _applicationDbContext.GamePlayer
                         join s in _applicationDbContext.Shot on gp equals s.Player
                         where gp.GameId == game.Id
                         select s).Count();
            
            // Return number of Shots and if you are the next player
            return new ShotInfoDto { ShotCount = count, IsNextPlayer = game.NextPlayer == gamePlayerId ? 1 : 0 };
        }

        public GameState GetGameState(Guid gamePlayerId)
        {
            var gameState = GameState.Ongoing;
            var game = (from g in _applicationDbContext.GamePlayer
                        where  g.Id.Equals(gamePlayerId)
                        select g.Game).SingleOrDefault();
            var shipsPlayer1  = (from gp in _applicationDbContext.GamePlayer
                                 join sp in _applicationDbContext.ShipPosition on gp equals sp.GamePlayer
                                 where gp.Game.Equals(game) && gp.Id.Equals(gamePlayerId)
                                 select sp).ToList();
            var shipsPlayer2 = (from gp in _applicationDbContext.GamePlayer
                                join sp in _applicationDbContext.ShipPosition on gp equals sp.GamePlayer
                                where gp.Game.Equals(game) && gp.Id != gamePlayerId
                                select sp).ToList();
            if( !IsAnyShipAlive(shipsPlayer1) || !IsAnyShipAlive(shipsPlayer2))
            {
                game.GameStatus = GameState.Complete.ToString();
                _applicationDbContext.Game.Update(game);
                _applicationDbContext.SaveChanges();
            }
            if (!IsAnyShipAlive(shipsPlayer2))
            {
                return gameState = GameState.Lost;
            }
            if (!IsAnyShipAlive(shipsPlayer1))
            {
                return gameState = GameState.Won;
            }

            if (gameState.ToString() != game.GameStatus)
            {
                game.GameStatus = gameState.ToString();
                _applicationDbContext.Game.Update(game);
                _applicationDbContext.SaveChanges();
            }

            return gameState;
        }
        private bool IsAnyShipAlive(List<ShipPosition> ships)
        {
            return ships.Any(x => x.Life > 0);
        }
    }
}