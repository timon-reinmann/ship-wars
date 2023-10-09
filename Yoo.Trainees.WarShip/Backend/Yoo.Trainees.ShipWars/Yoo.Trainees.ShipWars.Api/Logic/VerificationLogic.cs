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

        //public bool VerifyShipLocations(SaveShipDto[] shipDtos)
        //{
        //    if (shipDtos.Length != 10)
        //    {
        //        return false;
        //    }

        //    // mehr checks


        //    int[,] playingFieldBoarder = new int[9, 9];
        //    int[,] playingFieldShip = new int[9, 9];

        //    for (int i = 0; i < shipDtos.Length; i++)
        //    {
        //        var shipType = ships.SingleOrDefault(x => x.Name == shipDtos[i].ShipType);
        //        if (shipDtos[i].X != 0 && shipDtos[i].X + shipType.Length - 1 != 9 && shipDtos[i].Y != 0 && shipDtos[i].Y != 9)
        //        {

        //            for (int j = 0; j < shipType.Length - 1; j++)
        //            {
        //                for (int k = 0; k < 3 - 1; k++)
        //                {
        //                        playingFieldBoarder[j + shipDtos[i].X - 1, k + shipDtos[i].Y - 1] = 1;
        //                }
        //            }
        //            for (int j = 0; j  <= shipType.Length; j++)
        //            {
        //                if (playingFieldBoarder[j + shipDtos[i].X, shipDtos[i].Y] == 1)
        //                {
        //                    return false;
        //                }
        //                else
        //                {
        //                    playingFieldShip[j + shipDtos[i].X, shipDtos[i].Y] = 1;
        //                }
        //            }
        //        }
        //    }


        //    return true;
        //}

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
