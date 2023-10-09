using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public bool VerifyShipLocations(SaveShipDto[] shipDtos)
        {
            if (shipDtos.Length != 10)
            {
                return false;
            }

            for (int i = 0; i < shipDtos.Length; i++)
            {
                var shipType = ships.SingleOrDefault(x => x.Name == shipDtos[i].ShipType);
                var shipX = shipDtos[i].X;
                var shipY = shipDtos[i].Y;
                var shipLength = shipType.Length;
                var _iY1 = shipY - 1;
                var _iY2 = shipY + 1;
                var _iX1 = shipX - 1;
                var _iX2 = shipX + 1;
                var shipDirection = shipDtos[i].Direction;
                int _iXl;
                int _iYl;
                var allShips = new Dictionary<string, int>();

                if (shipX > 9 || shipY > 9 || shipX < 0 || shipY < 0 || shipType == null)
                {
                    return false;
                }

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
                    if (c.Value == 1 && c.Key != "Warship" || c.Value == 2 && c.Key != "Cruiser" || c.Value == 3 && c.Key != "Destroyer" || c.Value == 4 && c.Key != "Submarine")
                    {
                        return false;
                    }
                }
                foreach (var j in shipDtos)
                    for (int l = -1; l <= shipLength; l++)
                    {
                        _iXl = shipX + l;
                        _iYl = shipY + l;

                        if (shipDtos[i] != j)
                        {
                            switch (shipDirection)
                            {
                                case "horizontal":
                                    if (j.X == _iXl && j.Y == shipY || j.X == _iXl && j.Y == _iY1 || j.X == _iXl && j.Y == _iY2)
                                    {
                                        return false;
                                    }
                                    break;
                                case "vertical":
                                    if (j.Y == _iYl && j.X == shipX || j.Y == _iYl && j.X == _iX1 || j.Y == _iYl && j.X == _iX2)
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
            return true;
        }
    }
}


