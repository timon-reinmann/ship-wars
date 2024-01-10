using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Linq;
using Yoo.Trainees.ShipWars.Api.Controllers;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{

    public class BotLogic : IBotLogic
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private IVerificationLogic _verificationLogic;
        private static String[] ships = {   "warship",
                                            "cruiser",
                                            "cruiser",
                                            "destroyer",
                                            "destroyer",
                                            "destroyer",
                                            "submarine",
                                            "submarine",
                                            "submarine",
                                            "submarine" };

        public BotLogic(ApplicationDbContext applicationDbContext, IVerificationLogic verificationLogic, IConfiguration configuration)
        {
            this._applicationDbContext = applicationDbContext;
            this._verificationLogic = verificationLogic;
            this._configuration = configuration;
        }

        public void SaveShipPositionsInBotGame(SaveShipsDto SwaggerData)
        {
            var gamePlayerId = SwaggerData.GamePlayerId;
            var game = GetGame(gamePlayerId);
            
            if (game == null)
            {
                throw new Exception("game not found");
            }

            var botGamePlayerId = GetBotGamePlayerId(gamePlayerId);

            Guid id = new Guid();
            for (var i = 0; i < SwaggerData.Ships.Length; i++)
            {
                var Ship = SwaggerData.Ships[i];
                var shipType = _applicationDbContext.Ship.Where(ship => ship.Name == Ship.ShipType).SingleOrDefault();
                var shipPositio = new ShipPosition
                {
                    Id = Guid.NewGuid(),
                    GamePlayerId = Guid.Parse(gamePlayerId.ToString()),
                    ShipId = Guid.Parse(shipType.Id.ToString()),
                    X = Ship.X,
                    Y = Ship.Y,
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)Ship.Direction,
                    Life = shipType.Length,
                    IsHuman = true
                };
                id = shipPositio.GamePlayerId;

                _applicationDbContext.ShipPosition.Add(shipPositio);
            }
            _applicationDbContext.SaveChanges();

            CreateAndSaveBotShipPositions(gamePlayerId);
        }
        public void CreateAndSaveBotShipPositions(Guid gamePlayerId)
        {
            bool verifyResult = true;
            string[] badRequest = { "-1", "-1" };
            var saveShipDtos = new SaveShipDto[10];
            var shipCount = 10;
            var game = GetGame(gamePlayerId);
            var botGamePLayerId = GetBotGamePlayerId(gamePlayerId);


            Random rnd = new Random();

            if (game == null)
            {
                throw new Exception("game not found");
            }
            Guid id = new Guid();
            for (var i = 0; i < shipCount; i++)
            {
                saveShipDtos[i] = new SaveShipDto
                {
                    Id = Guid.NewGuid(),
                    X = rnd.Next(0, 10),
                    Y = rnd.Next(0, 10),
                    ShipType = ships[i],
                    Direction = (Direction)rnd.Next(0, 2)
                };

                i = _verificationLogic.VerifyShipPositionBot(saveShipDtos) ? i : i - 1;

            }
            
            foreach (var ship in saveShipDtos)
            {
                var shipPositio = new ShipPosition
                {
                    Id = ship.Id,

                    GamePlayerId = botGamePLayerId,
                    ShipId = (from s in _applicationDbContext.Ship
                              where s.Name.Equals(ship.ShipType)
                              select s.Id).SingleOrDefault(),
                    X = ship.X,
                    Y = ship.Y,
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)ship.Direction,
                    Life = (from s in _applicationDbContext.Ship
                            where s.Name.Equals(ship.ShipType)
                            select s.Length).SingleOrDefault(),
                    IsHuman = false
                };
                _applicationDbContext.ShipPosition.Add(shipPositio);
            }
            _applicationDbContext.SaveChanges();

        }

        public SaveBotShotsDto BotShotPosition(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            Random rnd = new Random();
            SaveBotShotsDto botShots;

            bool shotVerified;
            do
            {
                botShots = new SaveBotShotsDto
                {
                    X = rnd.Next(0, 10),
                    Y = rnd.Next(0, 10)
                };

                shotVerified = _verificationLogic.VerifyBotShot(allBotShots, botShots);

                if (shotVerified)
                {
                    var shot = new Shot
                    {
                        Id = Guid.NewGuid(),
                        X = botShots.X,
                        Y = botShots.Y,
                        Player = botGamePlayer
                    };
                    _applicationDbContext.Shot.Add(shot);
                    _applicationDbContext.SaveChanges();
                }
            } while (!shotVerified);

            return botShots;
        }

        public ShipHit CheckIfBotHit(SaveShotsDto xy, Guid gamePlayerId)
        {

            this._verificationLogic = new VerificationLogic();
            var ships = (from sp in _applicationDbContext.ShipPosition
                         where sp.GamePlayer.Id.Equals(gamePlayerId)
                         select new SaveShipDto
                         {
                             Direction = (Yoo.Trainees.ShipWars.Api.Direction)sp.Direction,
                             Id = sp.Id,
                             ShipType = sp.Ship.Name,
                             X = sp.X,
                             Y = sp.Y
                         }).ToList();

            var ship = _verificationLogic.VerifyShipHit(ships, xy);
            if (ship == null)
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

        public bool IsBotLobby(Guid gameId)
        {
            return (from g in _applicationDbContext.Game
                    where g.Id.Equals(gameId)
                    select g.IsBotGame).SingleOrDefault();
        }

        public Game? GetGame(Guid gamePlayerId)
        {
            return (from g in _applicationDbContext.GamePlayer
                    where g.Id.Equals(gamePlayerId)
                    select g.Game).SingleOrDefault();
        }

        private Guid GetBotPlayerId(Guid botGamePlayerId)
        {
            return (from gp in _applicationDbContext.GamePlayer
                    where gp.Id.Equals(botGamePlayerId)
                    select gp.PlayerId).SingleOrDefault();
        }

        private GamePlayer? GetBotGamePlayer(Guid gamePLayerId, Guid gameId)
        {
            return (from gp in _applicationDbContext.GamePlayer
                    where !gp.Id.Equals(gamePLayerId) &&
                    gp.GameId.Equals(gameId)
                    select gp).SingleOrDefault();
        }

        public Guid GetBotGamePlayerId(Guid gamePlayerId)
        {
            var gameId = GetGame(gamePlayerId).Id;
            return (from gp in _applicationDbContext.GamePlayer
                    where !gp.Id.Equals(gamePlayerId) &&
                    gp.GameId.Equals(gameId)
                    select gp.Id).SingleOrDefault();
        }
        private int GetShotCount(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            var shot = (from s in _applicationDbContext.Shot
                        where s.Player.Equals(botGamePlayer)
                        select s).Count();
            return shot;
        }
        public List<SaveBotShotsDto> GetAllBotShots(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (from s in _applicationDbContext.Shot
                    where s.Player.Equals(botGamePlayer)
                    select new SaveBotShotsDto { X = s.X, Y = s.Y }).ToList();
        }
    }
}
