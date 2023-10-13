using Yoo.Trainees.ShipWars.Api.Logic;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Test
{
    public class VerificationLogicTest
    {
        private List<Ship> _ships;

        [SetUp]
        public void Setup()
        {
            _ships = new List<Ship>
            {
                new Ship { Length = 2, Name = "Destroyer" },
                new Ship { Length = 4, Name = "Warship" },
                new Ship { Length = 3, Name = "Cruiser" },
                new Ship { Length = 1, Name = "Submarine" }
            };
        }

        [Test]
        public void TestVerifyEmptyList_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic(_ships);

            var shipDtos = new SaveShipDto[] {};

            Assert.False(verificationLogic.VerifyEverything(shipDtos));
        }

        [Test]
        public void TestVerifyCorrectFilledList_ShouldReturnTrue()
        {
            var verificationLogic = new VerificationLogic(_ships);

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

            Assert.True(verificationLogic.VerifyEverything(shipDtos));
        }

        [Test]
        public void TestVerifyOverlappingShips_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic(_ships);

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

            Assert.False(verificationLogic.VerifyEverything(shipDtos));
        }

        [Test]
        public void TestVerifyeOutOfBound_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic(_ships);

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

            Assert.False(verificationLogic.VerifyEverything(shipDtos));
        }

        [Test]
        public void TestVerifyeUnkownShips_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic(_ships);

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

            Assert.False(verificationLogic.VerifyEverything(shipDtos));
        }

        [Test]
        public void TestVerifyeToManyShipsFromSameType_ShouldReturnFalse()
        {
            var verificationLogic = new VerificationLogic(_ships);

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

            Assert.False(verificationLogic.VerifyEverything(shipDtos));
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
            var shot = new SaveShotsDto{ X = 3, Y = 2 };

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
            var shot = new SaveShotsDto{ X =  3, Y = -7 };

            Assert.False(verificationLogic.VerifyShot(shotsDto, shot));
        }

        [Test]
        public void TestVerifyShotRight_ShouldReturnRight()
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
            var shot = new SaveShotsDto{ X = 3, Y = 7 };

            Assert.True(verificationLogic.VerifyShot(shotsDto, shot));
        }
    }
}