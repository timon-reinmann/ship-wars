using Yoo.Trainees.ShipWars.Common.Enums;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;
using Yoo.Trainees.ShipWars.DataBase.Migrations;

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
                    Direction = (Direction)ship.Direction,
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
                    Direction = (Direction)ship.Direction,
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
            var isBotGame = IsBotLobby(gamePlayerId);
            var gameMode = GetGameMode(game.Id);
            var allBotShots = (gameMode == GameMode.hard) ? GetAllHardGameBotShots(gamePlayerId, game.Id) : GetAllBotShots(gamePlayerId, game.Id);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            Random rnd = new Random();
            SaveShotsDto botShots;

            BotResponse shotVerified;
            do
            {
                botShots = new SaveShotsDto
                {
                    X = rnd.Next(0, 10),
                    Y = rnd.Next(0, 10)
                };

                shotVerified = _verificationLogic.VerifyBotShot(allBotShots, botShots, gamePlayerId);

            } while (shotVerified == BotResponse.Outside || shotVerified == BotResponse.AlredyExist || shotVerified == BotResponse.NotValidArea);

            if (isBotGame)
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
            return botShots;
        }

        public ShipHit CheckIfBotHit(SaveShotsDto xy, Guid gamePlayerId)
        {
            var ships = (from sp in _applicationDbContext.ShipPosition
                         where sp.GamePlayer.Id.Equals(gamePlayerId)
                         select new SaveShipDto
                         {
                             Direction = (Direction)sp.Direction,
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
            Console.WriteLine(gamePlayerId);
            SaveShotsDto testBotShot;
            HardGameShot botShot;
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var isFistShot = IsFirstShot(gamePlayerId);
            if (isFistShot)
            {
                testBotShot = BotRandomShotPosition(gamePlayerId);
                var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
                var largestShip = GetAliveLargestShipLength(gamePlayerId);
                botShot = new HardGameShot()
                {
                    Id = Guid.NewGuid(),
                    X = testBotShot.X,
                    Y = testBotShot.Y,
                    Hit = (ShipHit)isHit,
                    Step = (largestShip == 1 && isHit == ShipHit.Hit) ? Step.Random : ((isHit == ShipHit.Hit) ? Step.ShootAround : Step.Random),
                    MainShot = (isHit == ShipHit.Hit) ? true : false,
                    Direction = Navigation.none,
                    Player = botGamePlayer,
                    CreatedAt = DateTime.Now,
                    HitShotsCounter = (isHit == ShipHit.Hit) ? 1 : 0
                };
                _applicationDbContext.HardGameShot.Add(botShot);
                _applicationDbContext.SaveChanges();

                SaveBotShotInDto(testBotShot, gamePlayerId);

                return testBotShot;
            }

            var mainShot = GetMainShot(gamePlayerId);
            var lastHardGameShot = GetLastBotShotHardGame(gamePlayerId);
            switch (lastHardGameShot.Step)
            {
                case Step.Random:
                default:
                    testBotShot = RandomShot(gamePlayerId);
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
            BotResponse verifyResult;
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            var allBotShots = GetAllHardGameBotShots(gamePlayerId, game.Id);
            var rnd = new Random();
            var possibleDirections = new List<int>
            {
               0,1,2,3
            };

            Navigation tempNavigation;

            do
            {
                var index = rnd.Next(0, possibleDirections.Count);

                switch (possibleDirections[index])
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
                possibleDirections.Remove(possibleDirections[index]);

                verifyResult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot, gamePlayerId);


                if (possibleDirections.Count == 0 && verifyResult != BotResponse.Approved)
                {
                    return RandomShot(gamePlayerId);
                }

            } while (verifyResult == BotResponse.Outside || verifyResult == BotResponse.AlredyExist);


            var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
            var largestShip = GetAliveLargestShipLength(gamePlayerId);
            var isShotAroundFromAHitShip = (verifyResult == BotResponse.NotValidArea) ? true : false;


            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (ShipHit)isHit,
                Step = (largestShip <= 2 && isHit == ShipHit.Hit || isShotAroundFromAHitShip) ? Step.Random : ((isHit == ShipHit.Hit) ? Step.ShootInThisDirection : Step.ShootAround),
                MainShot = false,
                Direction = (isHit == ShipHit.Hit) ? tempNavigation : Navigation.none,
                Player = botGamePlayer,
                CreatedAt = DateTime.Now,
                HitShotsCounter = (isHit == ShipHit.Hit) ? 2 : 1
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
            var allBotShots = GetAllHardGameBotShots(gamePlayerId, game.Id);
            var lastShot = GetLastBotShotHardGame(gamePlayerId);
            var tempNavigation = GetNavigation(gamePlayerId);
            var reverceNavigation = Navigation.none;
            var nextStep = false;
            SaveShotsDto testBotShot;
            BotResponse verifyResult;
            var counter = 0;
            do
            {
                counter++;
                switch (tempNavigation)
                {
                    case Navigation.top:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y - 1,
                        };
                        reverceNavigation = Navigation.bottom;

                        if (lastShot.Y - 1 == 0)
                        {
                            nextStep = true;
                        }
                        break;
                    case Navigation.bottom:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X,
                            Y = lastShot.Y + 1,
                        };
                        reverceNavigation = Navigation.top;

                        if (lastShot.Y + 1 == 9)
                        {
                            nextStep = true;
                        }
                        break;
                    case Navigation.left:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X - 1,
                            Y = lastShot.Y,
                        };
                        reverceNavigation = Navigation.right;

                        if (lastShot.X - 1 == 0)
                        {
                            nextStep = true;
                        }
                        break;
                    case Navigation.right:
                    default:
                        testBotShot = new SaveShotsDto()
                        {
                            X = lastShot.X + 1,
                            Y = lastShot.Y,
                        };
                        reverceNavigation = Navigation.left;

                        if (lastShot.X + 1 == 9)
                        {
                            nextStep = true;
                        }
                        break;
                }

                verifyResult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot, gamePlayerId);

                if (counter > 5)
                {
                    return ShootInTheOtherDirection(gamePlayerId);
                }

            } while (verifyResult == BotResponse.AlredyExist);

            ShipHit isHit = CheckIfBotHit(testBotShot, gamePlayerId);
            var largestShip = GetAliveLargestShipLength(gamePlayerId);
            var isShotAroundFromAHitShip = (verifyResult == BotResponse.NotValidArea) ? true : false;
            var finalyStep = (largestShip <= lastShot.hitShotsCounter + 1 && isHit == ShipHit.Hit && verifyResult != BotResponse.Outside || isShotAroundFromAHitShip) ? Step.Random : (nextStep ? Step.ShootInTheOtherDirection : ((isHit == ShipHit.Hit) ? Step.ShootInThisDirection : Step.ShootInTheOtherDirection));

            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (ShipHit)isHit,
                Step = finalyStep,
                MainShot = false,
                Direction = (isHit == ShipHit.Hit || verifyResult != BotResponse.Outside && finalyStep == Step.ShootInThisDirection) ? tempNavigation : reverceNavigation,
                Player = botGamePlayer,
                CreatedAt = DateTime.Now,
                HitShotsCounter = (isHit == ShipHit.Hit) ? lastShot.hitShotsCounter + 1 : lastShot.hitShotsCounter + 0
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
            var allBotShots = GetAllHardGameBotShots(gamePlayerId, game.Id);
            var lastShot = GetLastBotShotHardGame(gamePlayerId);
            var tempNavigation = GetNavigation(gamePlayerId);
            var mainShot = GetMainShot(gamePlayerId);
            var reverceNavigation = Navigation.none;
            SaveShotsDto testBotShot;
            BotResponse verifyResult;
            var counter = 0;

            do
            {
                counter++;
                switch (tempNavigation)
                {
                    case Navigation.top:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X,
                            Y = mainShot.Y - 1,
                        };
                        break;
                    case Navigation.bottom:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X,
                            Y = mainShot.Y + 1,
                        };
                        break;
                    case Navigation.left:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X - 1,
                            Y = mainShot.Y,
                        };
                        break;
                    case Navigation.right:
                    default:
                        testBotShot = new SaveShotsDto()
                        {
                            X = mainShot.X + 1,
                            Y = mainShot.Y,
                        };
                        break;
                }

                verifyResult = _verificationLogic.VerifyBotShot(allBotShots, testBotShot, gamePlayerId);
                if (counter == 2)
                {
                    mainShot = lastShot;
                }
                else if (counter > 3 || verifyResult == BotResponse.Outside)
                {
                    return RandomShot(gamePlayerId);
                }


            } while (verifyResult == BotResponse.AlredyExist || verifyResult == BotResponse.NotValidArea);

            var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
            var largestShip = GetAliveLargestShipLength(gamePlayerId);
            var isShotAroundFromAHitShip = (verifyResult == BotResponse.NotValidArea) ? true : false;

            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (ShipHit)isHit,
                Step = (largestShip <= lastShot.hitShotsCounter + 2 && isHit == ShipHit.Hit || isShotAroundFromAHitShip) ? Step.Random : ((isHit == ShipHit.Hit) ? Step.ShootInTheOtherDirection : Step.Random),
                MainShot = false,
                Direction = tempNavigation,
                Player = botGamePlayer,
                CreatedAt = DateTime.Now,
                HitShotsCounter = (isHit == ShipHit.Hit) ? lastShot.hitShotsCounter + 1 : 0
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
                Y = xy.Y
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
                    select g.Game).FirstOrDefault();
        }

        public GameMode GetGameMode(Guid gameId)
        {
            return (GameMode)(from g in _applicationDbContext.Game
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
                    orderby hgs.CreatedAt descending
                    select new BotShotHardGameDto { X = hgs.X, Y = hgs.Y, hitShotsCounter = hgs.HitShotsCounter, Hit = hgs.Hit, MainShot = hgs.MainShot, Step = hgs.Step }).FirstOrDefault();
        }

        private Navigation GetNavigation(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, game.Id);
            return (from hgs in _applicationDbContext.HardGameShot
                    where hgs.Player.Equals(botGamePlayer) && hgs.Direction != Navigation.none
                    orderby hgs.CreatedAt descending
                    select hgs.Direction).FirstOrDefault();
        }
        public List<SaveShotsDto> GetAllHardGameBotShots(Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (from s in _applicationDbContext.HardGameShot
                    where s.Player.Equals(botGamePlayer)
                    orderby s.CreatedAt descending
                    select new SaveShotsDto { X = s.X, Y = s.Y }).ToList();
        }

        public ShipHit GetShipHit(SaveShotsDto xy, Guid gamePlayerId, Guid gameId)
        {
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            return (ShipHit)(from hgs in _applicationDbContext.HardGameShot
                             where hgs.Player.Equals(botGamePlayer) && hgs.X == xy.X && hgs.Y == xy.Y
                             orderby hgs.CreatedAt descending
                             select hgs.Hit).FirstOrDefault();
        }
        private BotShotHardGameDto GetLastBotShotHardGame(Guid gamePlayerId)
        {
            var game = GetGame(gamePlayerId);
            var botGamePayer = GetBotGamePlayer(gamePlayerId, game.Id);
            return (from hgs in _applicationDbContext.HardGameShot
                    where hgs.Player.Equals(botGamePayer)
                    orderby hgs.CreatedAt descending
                    select new BotShotHardGameDto { X = hgs.X, Y = hgs.Y, Step = hgs.Step, hitShotsCounter = hgs.HitShotsCounter }).FirstOrDefault();
        }
        private SaveShotsDto RandomShot(Guid gamePlayerId)
        {
            var gameId = GetGame(gamePlayerId).Id;
            var allBotShots = GetAllBotShots(gamePlayerId, gameId);
            var botGamePlayer = GetBotGamePlayer(gamePlayerId, gameId);
            var testBotShot = BotRandomShotPosition(gamePlayerId);
            var isHit = CheckIfBotHit(testBotShot, gamePlayerId);
            var isShotAroundFromAHitShip = (_verificationLogic.VerifyBotShot(allBotShots, testBotShot, gamePlayerId) == BotResponse.NotValidArea) ? true : false;
            var largestShip = GetAliveLargestShipLength(gamePlayerId);
            if (isShotAroundFromAHitShip)
            {
                RandomShot(gamePlayerId);
            }
            var botShot = new HardGameShot()
            {
                Id = Guid.NewGuid(),
                X = testBotShot.X,
                Y = testBotShot.Y,
                Hit = (ShipHit)isHit,
                Step = (largestShip == 1 && isHit == ShipHit.Hit) ? Step.Random : ((isHit == ShipHit.Hit) ? Step.ShootAround : Step.Random),
                MainShot = (isHit == ShipHit.Hit) ? true : false,
                Direction = Navigation.none,
                Player = botGamePlayer,
                CreatedAt = DateTime.Now,
                HitShotsCounter = (isHit == ShipHit.Hit) ? 1 : 0
            };
            _applicationDbContext.HardGameShot.Add(botShot);
            _applicationDbContext.SaveChanges();

            SaveBotShotInDto(testBotShot, gamePlayerId);
            return testBotShot;
        }
        private int? GetAliveLargestShipLength(Guid gamePlayerId)
        {
            return (from shp in _applicationDbContext.ShipPosition
                    join sh in _applicationDbContext.Ship on shp.ShipId equals sh.Id
                    where shp.Life != 0 && shp.GamePlayerId.Equals(gamePlayerId)
                    orderby sh.Length descending
                    select sh.Length).FirstOrDefault();
        }
    }
}
