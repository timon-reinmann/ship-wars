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
                var shipDirection = shipDtos[i].Direction;
                int _iXl;
                foreach (var j in shipDtos)
                {
                    for (int l = -1; l <= shipLength; l++)
                    {
                        _iXl = shipX + l;
                        if (shipDtos[i] != j)
                        {
                            switch (shipDirection)
                            {
                                case "horizontal":
                                    if (j.X == _iXl && j.Y == shipY || j.X == _iXl && j.Y == shipY - 1 || j.X == _iXl && j.Y == _iY2)
                                    {
                                        return false;
                                    }
                                    break;
                                case "vertical":
                                    //todo
                                    break;
                            }
                        }
                    }
                }

            }
            return true;
        }


    }

}
