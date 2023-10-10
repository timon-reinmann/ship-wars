using Yoo.Trainees.ShipWars.DataBase.Entities;

namespace Yoo.Trainees.ShipWars.Api.Logic
{
    public interface IGameLogic
    {
        Game CreateGame(string name);
        void CreateBoard(SaveShipsDto Ships);
    }
}
