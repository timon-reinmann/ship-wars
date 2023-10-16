﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
﻿using Microsoft.EntityFrameworkCore.Query.Internal;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public class GameLogic : IGameLogic
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly VerificationLogic verificationLogic = new VerificationLogic();
        private Game Game;

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

            this.Game = new Game
            {
                Id = gameId,
                Name = name,
                GameStatus = "TODO",
                GamePlayers = gamePlayers,
                Date = DateTime.Now
            };


            applicationDbContext.Player.Add(player1);
            applicationDbContext.Player.Add(player2);
            applicationDbContext.Game.Add(this.Game);
            applicationDbContext.SaveChanges();

            return this.Game;
        }
        public void CreateBoard(SaveShipsDto SwaggerData)
        {
            var game = applicationDbContext.Game.Find(SwaggerData.GameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }
            Guid id = new Guid();
            for (var i = 0; i < SwaggerData.Ships.Length; i++)
            {
                var Ship = SwaggerData.Ships[i];
                var shipType = applicationDbContext.Ship.Where(ship => ship.Name == Ship.ShipType).SingleOrDefault();
                var shipPositio = new ShipPosition
                {
                    Id = Guid.NewGuid(),
                    GamePlayerId = Guid.Parse(SwaggerData.GamePlayerId.ToString()),
                    ShipId = Guid.Parse(shipType.Id.ToString()),                                      // Weirde Fehler aber me hets müesse Caste well es die gliche enums sie müesse
                    X = Ship.X,                                                                      // Und well beides in anderne Files gmacht worde isch es genau gseh nid sgliche :(    
                    Y = Ship.Y,
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)Ship.Direction
                };
                id = shipPositio.GamePlayerId;

                applicationDbContext.ShipPosition.Add(shipPositio);
            }
            applicationDbContext.SaveChanges();
        }
        public bool IsReady(Guid gameId)
        {
            // var game = applicationDbContext.Game.Where(Game => Game.Id == gameId).SingleOrDefault();
            // var players = game.GamePlayers.Count();

            var gamePlayers = from s in applicationDbContext.GamePlayer
                      where s.GameId == gameId
                      select s;
            var gamePlayer1 = from s in applicationDbContext.ShipPosition
                                   where s.GamePlayerId == gamePlayers.First().Id
                                   select s;
            var gamePlayer2 = from s in applicationDbContext.ShipPosition
                              where s.GamePlayerId == gamePlayers.ToArray()[1].Id
                              select s;

            var count = gamePlayer1.Count() + gamePlayer2.Count();
            
            return count == 20;
        }
        public BoardStateDto[] IsComplete(Guid gamePlayerId)
        {
            var gamePlayer = from sp in applicationDbContext.ShipPosition
                             join g in applicationDbContext.Ship on sp.ShipId equals g.Id
                             where sp.GamePlayerId.Equals(gamePlayerId)
                             select new BoardStateDto { X = sp.X, Y = sp.Y, Direction = (Yoo.Trainees.ShipWars.Api.Direction)sp.Direction, Name = g.Name };
            if (gamePlayer.Count() == 10)
                return gamePlayer.ToArray();
            return null;
        }
        public bool CheckShots(Guid gameId, Guid gamePlayerId)
        {
            var game = (from sp in applicationDbContext.Game
                        where sp.Id.Equals(gameId)
                        select sp.NextPlayer);
            Guid? nextPlayerId = game.SingleOrDefault();
            //var player1 = (from s in applicationDbContext.Shot
            //               join gp in applicationDbContext.GamePlayer on s.Player.Id equals gp.Id
            //               where s.Player.Id.Equals(gamePlayerId) && gp.GameId.Equals(gameId)
            //               select s.);
            //var player2 = (from s in applicationDbContext.Shot
            //               join gp in applicationDbContext.GamePlayer on s.Player.Id equals gp.Id
            //               where s.Player.Id != gamePlayerId && gp.GameId.Equals(gameId)
            //              select s).ToList();
            //var player1Count = player1.Count;
            //var player2Count = player2.Count;
            return nextPlayerId == gamePlayerId;
        }

        public void VerifyAndExecuteShotOrThrow(SaveShotsDto xy, Guid gamePlayerId)
        {
            SaveShotsDto shot = new SaveShotsDto { X = xy.X, Y = xy.Y};
            var shots = (from gp in applicationDbContext.GamePlayer
                        join s in applicationDbContext.Shot on gp.PlayerId equals s.Player.Id
                        where gp.Id == gamePlayerId
                        select new SaveShotsDto { X = s.X, Y = s.Y })
                        .ToList();

            if (shots == null || !verificationLogic.VerifyShot(shots, shot))
            {
                throw new InvalidOperationException("Ungültiger Schuss");
            }
        }
        public void SaveShot(SaveShotsDto shot, Guid gamePlayerId)
        {
            var player = applicationDbContext.GamePlayer
                .FirstOrDefault(x => x.Id == gamePlayerId)?.Player;

            var gamePlayer = (from gp in applicationDbContext.GamePlayer
                           where gp.Id == gamePlayerId
                           select gp).FirstOrDefault();
            var shotToSave = new Shot
            {
                Id = Guid.NewGuid(),
                X = shot.X,
                Y = shot.Y,
                Player = gamePlayer
            };
            applicationDbContext.Shot.Add(shotToSave);
            applicationDbContext.SaveChanges();
        }
        public void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            var gamePlayer = applicationDbContext.GamePlayer.First(x => x.Id == gamePlayerId);
            
            gamePlayer.ScissorsRockPaperBet = scissorsRockPaperBet;
            applicationDbContext.GamePlayer.Update(gamePlayer);

            applicationDbContext.SaveChanges();
        }
        public bool GetResultOfTheSRP(Guid gamePlayerId)
        {
            var gameId = (from gp in applicationDbContext.GamePlayer
                          where gp.Id.Equals(gamePlayerId)
                          select gp.GameId).SingleOrDefault();
            var player1 = (from gp in applicationDbContext.GamePlayer
                           where gp.Id == gamePlayerId
                           select gp.ScissorsRockPaperBet).SingleOrDefault();
            var player2 = (from gp in applicationDbContext.GamePlayer
                           where gp.GameId != gamePlayerId && gp.GameId.Equals(gameId)
                           select gp.ScissorsRockPaperBet).SingleOrDefault();

            if(player2 == null || player1 == null) return false;

            var game = (from g in applicationDbContext.Game
                        where g.Id.Equals(gameId)
                        select g).SingleOrDefault();

            // Kann ich es hier mit ()||()||() Verbinden oder ist das zu unleserlich?
            if((player1 == ScissorsRockPaper.Scissors && player2 == ScissorsRockPaper.Rock) ||
               (player1 == ScissorsRockPaper.Rock && player2 == ScissorsRockPaper.Paper)    ||
               (player1 == ScissorsRockPaper.Paper && player2 == ScissorsRockPaper.Scissors))
            {
                
                return false;
            }

            return true;
        }
    }
}