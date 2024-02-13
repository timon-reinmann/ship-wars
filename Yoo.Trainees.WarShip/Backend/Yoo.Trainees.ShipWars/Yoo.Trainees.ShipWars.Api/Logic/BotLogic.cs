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
        readonly static String[] ships = {   "warship",
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

            for (var i = 0; i < SwaggerData.Ships.Length; i++)
            {
                var ship = SwaggerData.Ships[i];
                var shipType = _applicationDbContext.Ship.Where(x => x.Name == ship.ShipType).SingleOrDefault();
                var shipPositio = new ShipPosition
                {
                    Id = Guid.NewGuid(),
                    GamePlayerId = Guid.Parse(gamePlayerId.ToString()),
                    ShipId = Guid.Parse(shipType.Id.ToString()),
                    X = ship.X,
                    Y = ship.Y,
                    Direction = (Yoo.Trainees.ShipWars.DataBase.Entities.Direction)ship.Direction,
                    Life = shipType.Length,
                    IsHuman = true
                };

                _applicationDbContext.ShipPosition.Add(shipPositio);
            }
            _applicationDbContext.SaveChanges();

            CreateAndSaveBotShipPositions(gamePlayerId);
        }

        public void CreateAndSaveBotShipPositions(Guid gamePlayerId)
        {
            var saveShipDtos = new SaveShipDto[10];
            var shipCount = 10;
            var game = GetGame(gamePlayerId);
            var botGamePlayerId = GetBotGamePlayerId(gamePlayerId);

            var rnd = new Random();

            if (game == null)
            {
                throw new Exception("game not found");
            }

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

                i = _verificationLogic.VerifyShipLocations(saveShipDtos, true) ? i : i - 1;
            }

            foreach (var ship in saveShipDtos)
            {
                var shipPositio = new ShipPosition
                {
                    Id = ship.Id,

                    GamePlayerId = botGamePlayerId,
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

        public SaveShotsDto BotRandomShotPosition(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            Random rnd = new Random();
            SaveShotsDto botShots;

            bool shotVerified;
            do
            {
                botShots = new SaveShotsDto
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

        public SaveShotsDto HardGameMode(Guid gamePlayerId)
        {
            SaveShotsDto testBotShot;
            HardGameShot botShot;
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var isFistShot = IsFirstShot(gamePlayerId);
            if (isFistShot)
            {
                testBotShot = BotRandomShotPosition(gamePlayerId);
                var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
                botShot = new HardGameShot()
                {
                    Id = Guid.NewGuid(),
                    X = testBotShot.X,
                    Y = testBotShot.Y,
                    Hit = (Yoo.Trainees.ShipWars.DataBase.Entities.ShipHit)isHit,
                    Step = (isHit == ShipHit.Hit) ? Step.ShootAround : Step.Random,
                    MainShot = (isHit == ShipHit.Hit) ? true : false,
                    Direction = Navigation.none,
                    Player = botGamePlayer
                };
                _applicationDbContext.HardGameShot.Add(botShot);
                _applicationDbContext.SaveChanges();

                SaveBotShotInDto(testBotShot, gamePlayerId);

                return testBotShot;
            }


            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            var allHardGameBotShots = GetAllHardGameBotShots(gamePlayerId, game.Id);
            var mainShot = GetMainShot(gamePlayerId);
            var lastHardGameShot = allHardGameBotShots[allHardGameBotShots.Count - 1];
            switch (lastHardGameShot.Step)
            {
                case Step.Random:
                default:
                    testBotShot = BotRandomShotPosition(gamePlayerId);
                    var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
                    botShot = new HardGameShot()
                    {
                        Id = Guid.NewGuid(),
                        X = testBotShot.X,
                        Y = testBotShot.Y,
                        Hit = (Yoo.Trainees.ShipWars.DataBase.Entities.ShipHit)isHit,
                        Step = (isHit == ShipHit.Hit) ? Step.ShootAround : Step.Random,
                        MainShot = (isHit == ShipHit.Hit) ? true : false,
                        Direction = Navigation.none,
                        Player = botGamePlayer
                    };
                    _applicationDbContext.HardGameShot.Add(botShot);
                    _applicationDbContext.SaveChanges();

                    SaveBotShotInDto(testBotShot, gamePlayerId);

                    break;
                case Step.ShootAround:
                    testBotShot = ShootAround(mainShot, gamePlayerId);

                    break;
                case Step.ShootInThisDirection:
                    testBotShot = ShootInThisDirection(gamePlayerId);

                    break;
                case Step.ShootInTheOtherDirection:
                    testBotShot = ShootInTheOtherDirection(gamePlayerId);

                    break;
            }
            return testBotShot;
        }
        private SaveShotsDto ShootAround(BotShotHardGameDto mainShot, Guid gamePlayerId)
        {
            SaveShotsDto testBotShot;
            bool verifyresult = false;
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var lastShot = GetAllBotShots(gamePlayerId, game.Id).Last();
            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            var rnd = new Random();
            Navigation tempNavigation;

            do
            {
                var index = rnd.Next(0, 4);

                switch (index)
                {
                    case 0:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X + 1,
                            Y = mainShot.Y,
                        };
                        tempNavigation = Navigation.right;
                        break;
                    case 1:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X - 1,
                            Y = mainShot.Y,
                        };
                        tempNavigation = Navigation.left;
                        break;
                    case 2:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X,
                            Y = mainShot.Y + 1,
                        };
                        tempNavigation = Navigation.bottom;
                        break;
                    case 3:
                    default:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X,
                            Y = mainShot.Y - 1,
                        };
                        tempNavigation = Navigation.top;
                        break;
                }

                verifyresult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot);

            } while (!verifyresult);


            var isHit = CheckIfBotHit(testBotShot, gamePlayerId);

            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (Yoo.Trainees.ShipWars.DataBase.Entities.ShipHit)isHit,
                Step = (isHit == ShipHit.Hit) ? Step.ShootInThisDirection : Step.ShootAround,
                MainShot = false,
                Direction = (isHit == ShipHit.Hit) ? tempNavigation : Navigation.none,
                Player = botGamePlayer
            };

            _applicationDbContext.HardGameShot.Add(botShot);
            _applicationDbContext.SaveChanges();

            SaveBotShotInDto(testBotShot, gamePlayerId);

            return testBotShot;
        }
        private SaveShotsDto ShootInThisDirection(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            var lastShot = allBotShots[allBotShots.Count - 1];
            var tempNavigation = GetNavigation(gamePlayerId);
            var reverceNavigation = Navigation.none;
            SaveShotsDto testBotShot;
            var verifyResult = false;
            do
            {
                switch (tempNavigation)
                {
                    case Navigation.top:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y - 1,
                        };
                        reverceNavigation = Navigation.bottom;
                        break;
                    case Navigation.bottom:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y + 1,
                        };
                        reverceNavigation = Navigation.top;
                        break;
                    case Navigation.left:           
                        testBotShot = new SaveShotsDto()
                        {                           
                            X = lastShot.X - 1,     
                            Y = lastShot.Y,         
                        };                          
                        reverceNavigation = Navigation.right;
                        break;                      
                    case Navigation.right:          
                    default:                        
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X + 1,
                            Y = lastShot.Y,
                        };
                        reverceNavigation = Navigation.left;
                        break;
                }

                verifyResult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot);

            } while (!verifyResult);

            ShipHit isHit = CheckIfBotHit(testBotShot, gamePlayerId);

            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (Yoo.Trainees.ShipWars.DataBase.Entities.ShipHit)isHit,
                Step = (isHit == ShipHit.Hit) ? Step.ShootInThisDirection : Step.ShootInTheOtherDirection,
                MainShot = false,
                Direction = (isHit == ShipHit.Hit) ? tempNavigation : reverceNavigation,
                Player = botGamePlayer
            };

            _applicationDbContext.HardGameShot.Add(botShot);
            _applicationDbContext.SaveChanges();

            SaveBotShotInDto(testBotShot, gamePlayerId);

            return testBotShot;

        }
        private SaveShotsDto ShootInTheOtherDirection(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var allBotShots = GetAllBotShots(gamePlayerId, game.Id);
            var lastShot = allBotShots[allBotShots.Count - 1];
            var tempNavigation = GetNavigation(gamePlayerId);
            var reverceNavigation = Navigation.none;
            SaveShotsDto testBotShot;
            var verifyResult = false;
            do
            {
                switch (tempNavigation)
                {
                    case Navigation.top:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y - 1,
                        };
                        break;
                    case Navigation.bottom:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y + 1,
                        };
                        break;
                    case Navigation.left:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X - 1,
                            Y = lastShot.Y,
                        };
                        break;
                    case Navigation.right:
                    default:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X + 1,
                            Y = lastShot.Y,
                        };
                        break;
                }

                verifyResult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot);

            } while (!verifyResult);

            var isHit = CheckIfBotHit(testBotShot, gamePlayerId);

            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (Yoo.Trainees.ShipWars.DataBase.Entities.ShipHit)isHit,
                Step = (isHit == ShipHit.Hit) ? Step.ShootInTheOtherDirection : Step.Random,
                MainShot = false,
                Direction = tempNavigation,
                Player = botGamePlayer
            };

            _applicationDbContext.HardGameShot.Add(botShot);
            _applicationDbContext.SaveChanges();

            SaveBotShotInDto(testBotShot, gamePlayerId);

            return testBotShot;
        }
        public void SaveBotShotInDto(SaveShotsDto xy, Guid gamePlayerId)
        {
            var gameId = GetGame(gamePlayerId).Id;
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            var botShot = new Shot()
            {
                Id = Guid.NewGuid(),
                Player = botGamePlayer,
                X = xy.X,
                Y= xy.Y
            };

            _applicationDbContext.Shot.Add(botShot);
            _applicationDbContext.SaveChanges();
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

        public GameMode GetGameMode(Guid gameId)
        {
            return (Yoo.Trainees.ShipWars.Api.Logic.GameMode)(from g in _applicationDbContext.Game
                                                              where g.Id.Equals(gameId)
                                                              select g.GameMode).SingleOrDefault();
        }

        public List<SaveShotsDto> GetAllBotShots(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (from s in _applicationDbContext.Shot
                    where s.Player.Equals(botGamePlayer)
                    select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
        }
        public Guid GetBotGamePlayerId(Guid gamePlayerId)
        {
            var gameId = GetGame(gamePlayerId).Id;
            return (from gp in _applicationDbContext.GamePlayer
                    where !gp.Id.Equals(gamePlayerId) &&
                    gp.GameId.Equals(gameId)
                    select gp.Id).SingleOrDefault();
        }
        private GamePlayer? GetBotGamePlayer(Guid gamePLayerId, Guid gameId)
        {
            return (from gp in _applicationDbContext.GamePlayer
                    where !gp.Id.Equals(gamePLayerId) &&
                    gp.GameId.Equals(gameId)
                    select gp).SingleOrDefault();
        }


        private int GetShotCount(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            var shot = (from s in _applicationDbContext.Shot
                        where s.Player.Equals(botGamePlayer)
                        select s).Count();
            return shot;
        }

        private bool IsFirstShot(Guid gamePlayerId)
        {
            var gameId = GetGame(gamePlayerId).Id;
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return ((from s in _applicationDbContext.HardGameShot
                     where s.Player.Equals(botGamePlayer)
                     select s).Count() == 0) ? true : false;
        }

        private BotShotHardGameDto GetMainShot(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePayer = GetBotGamePlayer(gamePlayerId, game.Id);
            return (from hgs in _applicationDbContext.HardGameShot
                    where hgs.Player.Equals(botGamePayer) &&
                    hgs.MainShot.Equals(true)
                    select new BotShotHardGameDto { X = hgs.X, Y = hgs.Y, Step = hgs.Step }).SingleOrDefault();
        }

        private Navigation GetNavigation(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            return (from hgs in _applicationDbContext.HardGameShot
                    where hgs.Player.Equals(botGamePlayer) && hgs.Direction != Navigation.none
                    select hgs.Direction).SingleOrDefault();
        }
        public List<BotShotHardGameDto> GetAllHardGameBotShots(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (from s in _applicationDbContext.HardGameShot
                    where s.Player.Equals(botGamePlayer)
                    select new BotShotHardGameDto { X = s.X, Y = s.Y, Step = s.Step }).ToList();
        }

        public ShipHit GetShipHit(SaveShotsDto xy, Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (Yoo.Trainees.ShipWars.Api.Logic.ShipHit)(from hgs in _applicationDbContext.HardGameShot
                    where hgs.Player.Equals(botGamePlayer) && hgs.X == xy.X && hgs.Y == xy.Y
                    select hgs.Hit).FirstOrDefault();
        }
    }
}
