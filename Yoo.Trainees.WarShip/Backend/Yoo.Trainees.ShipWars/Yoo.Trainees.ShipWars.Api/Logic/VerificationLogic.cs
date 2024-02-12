using System.Data;
using Yoo.Trainees.ShipWars.Api.Controllers;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public class VerificationLogic : IVerificationLogic
    {
        private List<Ship> ships = GameController.Ships;

        public bool VerifyEverything(SaveShipDto[] shipDtos, bool isBotLobby)
        {
            return TestVerifyeToManyShipsFromSameType(shipDtos) && VerifyShipLocations(shipDtos, isBotLobby);
        }

        public bool VerifyShipLocations(SaveShipDto[] shipDtos, bool isBotLobby)
        {

            if (isBotLobby)
            {
                shipDtos = shipDtos.Where(x => x != null).ToArray();
            }
            else
            {
                var amountOfShips = 10;
                if (shipDtos.Length != amountOfShips)
                {
                    return false;
                }
            }

            foreach (var _ishipDtos in shipDtos)
            {
                var shipType = ships.SingleOrDefault(x => x.Name.ToUpper() == _ishipDtos.ShipType.ToUpper());
                var shipX = _ishipDtos.X;
                var shipY = _ishipDtos.Y;
                var shipLength = shipType.Length;
                var _iY1 = shipY - 1;
                var _iY2 = shipY + 1;
                var _iX1 = shipX - 1;
                var _iX2 = shipX + 1;
                var shipDirection = _ishipDtos.Direction;
                int _iXl;
                int _iYl;
                var maxBoardLength = 9;
                var minBoardLength = 0;

                if (shipX > maxBoardLength || shipY > maxBoardLength || shipX < minBoardLength || shipY < minBoardLength || shipType == null)
                {
                    return false;
                }

                if (shipDirection == 0)
                {
                    if (shipX + shipLength - 1 > maxBoardLength || shipY > maxBoardLength)
                    {
                        return false;
                    }
                }
                else
                {
                    if (shipX > maxBoardLength || shipY + shipLength - 1 > maxBoardLength)
                    {
                        return false;
                    }
                }

                foreach (var ship in shipDtos)
                {
                    var ShipType = ships.SingleOrDefault(x => x.Name.ToLower() == ship.ShipType.ToLower());
                    int Length = ShipType.Length;
                    //var jShipType = ships.SingleOrDefault(x => x.Name == j.ShipType);
                    for (int i = 0; i < Length; i++)
                    {
                        for (int l = -1; l <= shipLength; l++)
                        {
                            _iXl = shipX + l;
                            _iYl = shipY + l;

                            if (_ishipDtos != ship)
                            {
                                switch (shipDirection)
                                {
                                    case Direction.horizontal:
                                        switch (ship.Direction)
                                        {
                                            case Direction.horizontal:
                                                if (ship.X + i == _iXl && ship.Y == shipY || ship.X + i == _iXl && ship.Y == _iY1 || ship.X + i == _iXl && ship.Y == _iY2)
                                                {
                                                    return false;
                                                }
                                                break;
                                            case Direction.vertical:
                                                if (ship.X == _iXl && ship.Y + i == shipY || ship.X == _iXl && ship.Y + i == _iY1 || ship.X == _iXl && ship.Y + i == _iY2)
                                                {
                                                    return false;
                                                }
                                                break;
                                        }
                                        break;
                                    case Direction.vertical:
                                        switch (ship.Direction)
                                        {
                                            case Direction.horizontal:
                                                if (ship.Y == _iYl && ship.X + i == shipX || ship.Y == _iYl && ship.X + i == _iX1 || ship.Y == _iYl && ship.X + i == _iX2)
                                                {
                                                    return false;
                                                }
                                                break;

                                            case Direction.vertical:

                                                if (ship.Y + i == _iYl && ship.X == shipX || ship.Y + i == _iYl && ship.X == _iX1 || ship.Y + i == _iYl && ship.X == _iX2)
                                                {
                                                    return false;
                                                }
                                                break;
                                            default:
                                                return false;
                                        }
                                        break;
                                    default:
                                        return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool TestVerifyeToManyShipsFromSameType(SaveShipDto[] shipDtos)
        {
            var allShips = new Dictionary<string, int>();
            foreach (var _shipType in shipDtos)
            {
                if (!allShips.ContainsKey(_shipType.ShipType))
                {
                    allShips.Add(_shipType.ShipType, 1);
                }
                else
                {
                    allShips[_shipType.ShipType]++;
                }
            }
            foreach (var c in allShips)
            {
                if (c.Value == 1 && c.Key.ToLower() != "warship" || c.Value == 2 && c.Key.ToLower() != "cruiser" || c.Value == 3 && c.Key.ToLower() != "destroyer" || c.Value == 4 && c.Key.ToLower() != "submarine")
                {
                    return false;
                }
            }
            return true;
        }

        public bool VerifyShot(List<SaveShotsDto> shots, SaveShotsDto shot)
        {
            var validShotAreaMin = 0;
            var validShotAreaMax = 9;
            foreach (var sh in shots)
            {
                if (shot.X > validShotAreaMax || shot.X < validShotAreaMin || shot.Y > validShotAreaMax || shot.Y < validShotAreaMin)
                    return false;
                if (sh.X == shot.X && sh.Y == shot.Y)
                    return false;
                if (shots == null || shot == null)
                    return false;
            }

            return true;
        }

        public bool VerifyBotShot(IList<SaveBotShotsDto> shots, SaveBotShotsDto lastShot)
        {
            if (lastShot.X > 9 || lastShot.X < 0 || lastShot.Y > 9 || lastShot.Y < 0)
            {
                return false;
            }

            if (shots.Count > 1)
            {
                // Check if a shot with this coordinates already exists. If yes, verification fails
                var shotAlreadyExists = shots.Any(x => x.X == lastShot.X && x.Y == lastShot.Y);

                return !shotAlreadyExists;

            }

            return true;
        }
        public SaveShipDto VerifyShipHit(List<SaveShipDto> ships, SaveShotsDto shot)
        {
            foreach (var ship in ships)
            {
                var shipLength = (from s in this.ships
                                  where s.Name.ToLower() == ship.ShipType.ToLower()
                                  select s.Length).SingleOrDefault();
                var direction = ship.Direction;
                var yMaxLength = (direction == Direction.horizontal ? 1 : shipLength) + ship.Y;
                var xMaxLength = (direction == Direction.horizontal ? shipLength : 1) + ship.X;
                for (int y = ship.Y; y < yMaxLength; y++)
                {
                    for (int x = ship.X; x < xMaxLength; x++)
                    {
                        if (shot.X == x && shot.Y == y)
                            return ship;
                    }
                }
            }

            return null;
        }
    }
}


