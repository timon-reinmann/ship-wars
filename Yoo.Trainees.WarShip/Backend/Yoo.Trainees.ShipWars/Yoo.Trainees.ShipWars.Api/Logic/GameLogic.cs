using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
﻿using Microsoft.EntityFrameworkCore.Query.Internal;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;
using System.Collections.Generic;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public enum SRPStatus
    {
        WAITING,
        LOST,
        WON,
        DRAW,
        REDO
    }
    public enum ShipHit
    {
        MISSED,
        HIT
    }
    public class GameLogic : IGameLogic
    {
        private readonly ApplicationDbContext applicationDbContext;
        private VerificationLogic verificationLogic = new VerificationLogic();
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
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)Ship.Direction,
                    Life = shipType.Length
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
            var game = (from g in applicationDbContext.Game
                        where g.Id.Equals(gameId)
                        select g).SingleOrDefault();
            Guid? nextPlayerId = game.NextPlayer;

            if(nextPlayerId == gamePlayerId)
            {
                var nextPlayer = (from gp in applicationDbContext.GamePlayer
                                  where gp.Id != nextPlayerId && gp.GameId.Equals(gameId)
                                  select gp.Id).SingleOrDefault();
                game.NextPlayer = nextPlayer;
                applicationDbContext.Game.Update(game);
                applicationDbContext.SaveChanges();
            }
            return nextPlayerId == gamePlayerId;
        }

        public void VerifyAndExecuteShotOrThrow(SaveShotsDto xy, Guid gamePlayerId)
        {
            var game = (from gp in applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.Game).FirstOrDefault();
            SaveShotsDto shot = new SaveShotsDto { X = xy.X, Y = xy.Y};
            var shots = (from s in applicationDbContext.Shot
                        where s.Player.Id == gamePlayerId
                        select new SaveShotsDto { X = s.X, Y = s.Y })
                        .ToList();

            if (!verificationLogic.VerifyShot(shots, shot))
            {
                game.NextPlayer = gamePlayerId;
                applicationDbContext.Game.Update(game);
                applicationDbContext.SaveChanges();
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
        public List<SaveShotsDto> ShotsAllOpponent(Guid gamePlayerId)
        {
            var game = (from gp in applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.GameId).SingleOrDefault();
            var player2 = (from gp in applicationDbContext.GamePlayer
                           where gp.GameId.Equals(game) && gp.Id != gamePlayerId
                           select gp).SingleOrDefault(); 
            var shots = (from s in applicationDbContext.Shot
                         where s.Player.Id.Equals(player2.Id) 
                         select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
            return shots;
        }

        public List<SaveShotsDto> ShotsAll(Guid gamePlayerId)
        {
            var shots = (from s in applicationDbContext.Shot
                         where s.Player.Id.Equals(gamePlayerId)
                         select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
            return shots;
        }
        public void SaveChoiceIntoDB(ScissorsRockPaper scissorsRockPaperBet, Guid gamePlayerId)
        {
            var gamePlayer = applicationDbContext.GamePlayer.First(x => x.Id == gamePlayerId);

            if (gamePlayer != null) {
                gamePlayer.ScissorsRockPaperBet = scissorsRockPaperBet;
                applicationDbContext.GamePlayer.Update(gamePlayer);
                applicationDbContext.SaveChanges();
            }
        }
        public SRPStatus GetResultOfTheSRP(Guid gamePlayerId)
        {
            var gameId = (from gp in applicationDbContext.GamePlayer
                          where gp.Id.Equals(gamePlayerId)
                          select gp.GameId).FirstOrDefault();
            var player1 = (from gp in applicationDbContext.GamePlayer
                           where gp.Id == gamePlayerId
                           select gp).SingleOrDefault();
            var player2 = (from gp in applicationDbContext.GamePlayer
                           where gp.Id != gamePlayerId && gp.GameId.Equals(gameId)
                           select gp).FirstOrDefault();

            var game = (from g in applicationDbContext.Game
                        where g.Id.Equals(gameId)
                        select g).SingleOrDefault();


            if (player1.ScissorsRockPaperBet == null) return SRPStatus.REDO;
            if (player2.ScissorsRockPaperBet == null || game == null) return SRPStatus.WAITING;
            if (player1.ScissorsRockPaperBet == player2.ScissorsRockPaperBet) 
            {
                player1.ScissorsRockPaperBet = null;
                player2.ScissorsRockPaperBet = null;
                applicationDbContext.GamePlayer.Update(player1);
                applicationDbContext.GamePlayer.Update(player2);
                applicationDbContext.SaveChanges();
                return SRPStatus.DRAW; 
            }

            bool isPlayer1Loser = CheckIfPlayer1IsLoser(player1, player2);

            game.NextPlayer = isPlayer1Loser ? player2.Id : player1.Id;

            applicationDbContext.Game.Update(game);
            applicationDbContext.SaveChanges();

            return isPlayer1Loser ? SRPStatus.LOST : SRPStatus.WON;
        }
        public bool CheckIfPlayer1IsLoser(GamePlayer player1, GamePlayer player2)
        {
            return (player1.ScissorsRockPaperBet == ScissorsRockPaper.Scissors && player2.ScissorsRockPaperBet == ScissorsRockPaper.Rock) ||
                   (player1.ScissorsRockPaperBet == ScissorsRockPaper.Rock && player2.ScissorsRockPaperBet == ScissorsRockPaper.Paper) ||
                   (player1.ScissorsRockPaperBet == ScissorsRockPaper.Paper && player2.ScissorsRockPaperBet == ScissorsRockPaper.Scissors);
        }
        public ShipHit CheckIfShipHit(SaveShotsDto xy, Guid gamePlayerId, List<Ship> shipType)
        {
            var game = (from gp in applicationDbContext.GamePlayer
                        where gp.Id == gamePlayerId
                        select gp.Game).FirstOrDefault();

            var player2 = (from gp in applicationDbContext.GamePlayer
                           where gp.Id != gamePlayerId && gp.GameId.Equals(game.Id)
                           select gp).FirstOrDefault();

            this.verificationLogic = new VerificationLogic(shipType);
            var ships = (from sp in applicationDbContext.ShipPosition
                         where sp.GamePlayer.Id.Equals(player2.Id)
                         select new SaveShipDto { Direction = (Yoo.Trainees.ShipWars.Api.Direction)sp.Direction, Id = sp.Id, ShipType = sp.Ship.Name ,
                                                    X = sp.X, Y = sp.Y}).ToList();

            var ship = verificationLogic.VerifyShipHit(ships, xy);
            if(ship == null)
            {
                return ShipHit.MISSED;
            }
            var shipDB = (from sp in applicationDbContext.ShipPosition
                          where sp.Id.Equals(ship.Id)
                          select sp).FirstOrDefault();
            shipDB.Life--;
            applicationDbContext.ShipPosition.Update(shipDB);
            applicationDbContext.SaveChanges();
            return ShipHit.HIT;
        }
        
        public int[] CountShotsInDB(Guid gamePlayerId)
        {
            var game = (from gp in applicationDbContext.GamePlayer
                        where gp.Id.Equals(gamePlayerId)
                        select gp.Game).SingleOrDefault();
            var count = (from gp in applicationDbContext.GamePlayer
                         join s in applicationDbContext.Shot on gp equals s.Player
                         where gp.GameId == game.Id
                         select s).Count();
            
            int[] countAndNextPlayer = { count, game.NextPlayer == gamePlayerId ? 1 : 0};
            return countAndNextPlayer;
        }
    }
}