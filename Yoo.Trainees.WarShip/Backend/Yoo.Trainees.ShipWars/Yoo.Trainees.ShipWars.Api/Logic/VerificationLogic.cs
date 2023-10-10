﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Data;
using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public class VerificationLogic
    {
        private List<Ship> ships;
        private DataTable dtShip = default;

        public VerificationLogic(List<Ship> ships)
        {
            this.ships = ships;
        }

        public bool verifyEvrything(SaveShipDto[] shipDtos)
        {
            return TestVerifyeToManyShipsFromSameType(shipDtos) && VerifyShipLocations(shipDtos);
        }

        public bool VerifyShipLocations(SaveShipDto[] shipDtos)
        {
            var amountOfShips = 10;
            if (shipDtos.Length != amountOfShips)
            {
                return false;
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
                var shipDirection = _ishipDtos._direction;
                int _iXl;
                int _iYl;
                var maxBoardLength = 9;
                var minBoardLength = 0;

                if (shipX > maxBoardLength || shipY > maxBoardLength || shipX < minBoardLength || shipY < minBoardLength || shipType == null)
                {
                    return false;
                }

                foreach (var j in shipDtos)
                {
                    var jShipType = ships.SingleOrDefault(x => x.Name == j.ShipType);
                    int jLength = jShipType.Length;
                    //var jShipType = ships.SingleOrDefault(x => x.Name == j.ShipType);
                    for (int i = 0; i < jLength ; i++)
                    {
                        for (int l = -1; l <= shipLength; l++)
                        {
                            _iXl = shipX + l;
                            _iYl = shipY + l;

                            if (_ishipDtos != j)
                            {
                                switch (shipDirection)
                                {
                                    case Direction.horizontal:
                                         if (j.X + i == _iXl && j.Y == shipY || j.X + i == _iXl && j.Y == _iY1 || j.X + i == _iXl && j.Y == _iY2)
                                        {
                                            return false;
                                        }
                                        break;
                                    case Direction.vertical:
                                        if (j.Y + i == _iYl && j.X == shipX || j.Y + i == _iYl && j.X == _iX1 || j.Y + i == _iYl && j.X == _iX2)
                                        {
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
                if (c.Value == 1 && c.Key != "warship" || c.Value == 2 && c.Key != "cruiser" || c.Value == 3 && c.Key != "destroyer" || c.Value == 4 && c.Key != "submarine")
                {
                       return false;
                }
            }
            return true;
        }

    }
}


