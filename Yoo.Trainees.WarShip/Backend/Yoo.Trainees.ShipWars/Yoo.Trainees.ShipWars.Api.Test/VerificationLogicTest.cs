using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Test
{
    public class VerificationLogicTest
    {
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

        [Test]
        public void TestVerifyEmptyList_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[] { };

            Assert.False(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyCorrectFilledList_ShouldReturnTrue()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[]
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical , X = 1, Y = 0, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 1, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 6, Y = 4, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 3, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 3, Y = 6, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 1, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 4, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 7, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 9, ShipType = "Submarine"}
            };

            Assert.True(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyOverlappingShips_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[]
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 0, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 0, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 2, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 5, Y = 1, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 4, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 4, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 4, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 6, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 3, Y = 6, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 6, ShipType = "Submarine"}
            };

            Assert.False(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyeOutOfBound_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[]
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 2, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 3, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 6, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 0, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 4, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 8, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 0, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 5, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 12, ShipType = "Submarine"}
            };

            Assert.False(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyeUnkownShips_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[]
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 2, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 3, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 6, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 0, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 4, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 8, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 0, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 5, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 9, ShipType = "UnkownShip"}
            };

            Assert.False(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyeToManyShipsFromSameType_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new SaveShipDto[]
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 2, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 3, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 6, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 0, Y = 0, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 4, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 0, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 1, Y = 5, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 7, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 9, ShipType = "Submarine"}
            };

            Assert.False(verificationLogic.VerifyEverything(shipDtos, false));
        }

        [Test]
        public void TestVerifyHaveShotsSamePosition_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shotsDto = new List<SaveShotsDto>()
            {
                new SaveShotsDto{X = 1, Y = 2},
                new SaveShotsDto{X = 2, Y = 2},
                new SaveShotsDto{X = 3, Y = 8},
                new SaveShotsDto{X = 5, Y = 1},
                new SaveShotsDto{X = 1, Y = 5},
                new SaveShotsDto{X = 3, Y = 2}
            };
            var shot = new SaveShotsDto { X = 3, Y = 2 };

            Assert.False(verificationLogic.VerifyShot(shotsDto, shot));
        }

        [Test]
        public void TestVerifyShotsOutOfBoard_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shotsDto = new List<SaveShotsDto>()
            {
                new SaveShotsDto{X = 1, Y = 2},
                new SaveShotsDto{X = 2, Y = 2},
                new SaveShotsDto{X = 4, Y = 3},
                new SaveShotsDto{X = 1, Y = 4},
                new SaveShotsDto{X = 8, Y = 4},
                new SaveShotsDto{X = 2, Y = 5}
            };
            var shot = new SaveShotsDto { X = 3, Y = -7 };

            Assert.False(verificationLogic.VerifyShot(shotsDto, shot));
        }

        [Test]
        public void TestVerifyShotRight_ShouldReturnTrue()
        {
            var verificationLogic = new VerificationLogic();

            var shotsDto = new List<SaveShotsDto>()
            {
                new SaveShotsDto{X = 1, Y = 8},
                new SaveShotsDto{X = 2, Y = 1},
                new SaveShotsDto{X = 4, Y = 5},
                new SaveShotsDto{X = 7, Y = 5},
                new SaveShotsDto{X = 4, Y = 8},
                new SaveShotsDto{X = 2, Y = 8}
            };
            var shot = new SaveShotsDto { X = 3, Y = 7 };

            Assert.True(verificationLogic.VerifyShot(shotsDto, shot));
        }
        [Test]
        public void TestVerifyShipNotHit_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new List<SaveShipDto>
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical , X = 1, Y = 0, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 1, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 6, Y = 4, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 3, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 3, Y = 6, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 1, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 4, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 7, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 9, ShipType = "Submarine"}
            };

            var shot = new SaveShotsDto { X = 3, Y = 7 };

            Assert.AreEqual(null, verificationLogic.VerifyShipHit(shipDtos, shot));
        }

        [Test]
        public void TestVerifyShipHit_ShouldReturnTrue()
        {
            var verificationLogic = new VerificationLogic();

            var shipDtos = new List<SaveShipDto>
            {
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical , X = 1, Y = 0, ShipType = "Warship"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 4, Y = 1, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 6, Y = 4, ShipType = "Cruiser"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 3, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 3, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 3, Y = 7, ShipType = "Destroyer"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 1, Y = 8, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.vertical, X = 4, Y = 9, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 8, Y = 7, ShipType = "Submarine"},
                new SaveShipDto{ Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 6, Y = 9, ShipType = "Submarine"}
            };

            var shot = new SaveShotsDto { X = 4, Y = 7 };

            var expected = new SaveShipDto { Direction = (Yoo.Trainees.ShipWars.Api.Direction)Direction.horizontal, X = 3, Y = 7, ShipType = "Destroyer" };
            var actual = verificationLogic.VerifyShipHit(shipDtos, shot);

            Assert.AreEqual(expected.Direction, actual.Direction);
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
            Assert.AreEqual(expected.ShipType, actual.ShipType);
        }

        [Test]
        public void TestVerifyShipPositionsBot_ShouldReturnTrue()
        {
            var verificationLogic = new VerificationLogic();
            var saveShipDtos = new SaveShipDto[10];
            var rnd = new Random();

            for (var i = 0; i < 9; i++)
            {
                saveShipDtos[i] = new SaveShipDto
                {
                    Id = Guid.NewGuid(),
                    X = rnd.Next(0, 10),
                    Y = rnd.Next(0, 10),
                    ShipType = ships[i],
                    Direction = (Direction)rnd.Next(0, 2)
                };


                i = verificationLogic.VerifyShipLocations(saveShipDtos, true) ? i : i - 1;
            }
            Assert.True(verificationLogic.VerifyShipLocations(saveShipDtos, true));
        }
    }
}