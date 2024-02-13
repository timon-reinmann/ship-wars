using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics.Metrics;
using Yoo.Trainees.ShipWars.Api.Controllers;
using Yoo.Trainees.ShipWars.DataBase;
using Yoo.Trainees.ShipWars.DataBase.Entities;
using Yoo.Trainees.ShipWars.DataBase.Migrations;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public class VerificationLogic : IVerificationLogic
    {
        private List<Ship> ships = GameController.Ships;
        private DataTable dtShip = default;
        private readonly ApplicationDbContext applicationDbContext;

        public bool VerifyEverything(SaveShipDto[] shipDtos, bool isBotLobby)
        {
            return TestVerifyeToManyShipsFromSameType(shipDtos) && VerifyShipLocations(shipDtos, isBotLobby); ;
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

            foreach (var ship in shipDtos)
            {
                var shipType = ships.SingleOrDefault(x => x.Name.ToUpper() == ship.ShipType.ToUpper());
                var shipX = ship.X;
                var shipY = ship.Y;
                var shipLength = shipType.Length;
                var y1 = shipY - 1;
                var y2 = shipY + 1;
                var x1 = shipX - 1;
                var x2 = shipX + 1;
                var shipDirection = ship.Direction;
                int xl;
                int yl;
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

                foreach (var innerSaveShipDto in shipDtos)
                {
                    var jShipType = ships.SingleOrDefault(x => x.Name.ToLower() == innerSaveShipDto.ShipType.ToLower());
                    int jLength = jShipType.Length;
                    //var jShipType = ships.SingleOrDefault(x => x.Name == j.ShipType);
                    for (int i = 0; i < jLength; i++)
                    {
                        for (int l = -1; l <= shipLength; l++)
                        {
                            xl = shipX + l;
                            yl = shipY + l;

                            if (ship != innerSaveShipDto)
                            {
                                switch (shipDirection)
                                {
                                    case Direction.horizontal:
                                        switch (innerSaveShipDto.Direction)
                                        {
                                            case Direction.horizontal:
                                                if (innerSaveShipDto.X + i == xl && innerSaveShipDto.Y == shipY || innerSaveShipDto.X + i == xl && innerSaveShipDto.Y == y1 || innerSaveShipDto.X + i == xl && innerSaveShipDto.Y == y2)
                                                {
                                                    return false;
                                                }
                                                break;
                                            case Direction.vertical:
                                                if (innerSaveShipDto.X == xl && innerSaveShipDto.Y + i == shipY || innerSaveShipDto.X == xl && innerSaveShipDto.Y + i == y1 || innerSaveShipDto.X == xl && innerSaveShipDto.Y + i == y2)
                                                {
                                                    return false;
                                                }
                                                break;
                                        }
                                        break;
                                    case Direction.vertical:
                                        switch (innerSaveShipDto.Direction)
                                        {
                                            case Direction.horizontal:
                                                if (innerSaveShipDto.Y == yl && innerSaveShipDto.X + i == shipX || innerSaveShipDto.Y == yl && innerSaveShipDto.X + i == x1 || innerSaveShipDto.Y == yl && innerSaveShipDto.X + i == x2)
                                                {
                                                    return false;
                                                }
                                                break;

                                            case Direction.vertical:

                                                if (innerSaveShipDto.Y + i == yl && innerSaveShipDto.X == shipX || innerSaveShipDto.Y + i == yl && innerSaveShipDto.X == x1 || innerSaveShipDto.Y + i == yl && innerSaveShipDto.X == x2)
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

        public bool VerifyBotShot(IList<SaveShotsDto> shots, SaveShotsDto lastShot)
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


