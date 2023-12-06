using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
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

        public void SaveShipPositions(SaveShipsDto SwaggerData)
        {
            var gamePlayerId = SwaggerData.GamePlayerId;
            var game = _applicationDbContext.Game.Find(SwaggerData.GameId);
            if (game == null)
            {
                throw new Exception("game not found");
            }
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

            var saveShipDtos = GetBotShipPositions(gamePlayerId);
        }
        public SaveShipDto[] GetBotShipPositions(Guid gamePlayerId)
        {
            bool verifyResult = true;
            string[] badRequest = { "-1", "-1" };
            var saveShipDtos = new SaveShipDto[10];
            var shipCount = 10;

            Random rnd = new Random();
            var game = GetGame(gamePlayerId);
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
                    GamePlayerId = gamePlayerId,
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


                return saveShipDtos;

        }

        private Game? GetGame(Guid gamePlayerId)
        {
            return (from g in _applicationDbContext.GamePlayer
                    where g.Id.Equals(gamePlayerId)
                    select g.Game).SingleOrDefault();
        }
    }
}
