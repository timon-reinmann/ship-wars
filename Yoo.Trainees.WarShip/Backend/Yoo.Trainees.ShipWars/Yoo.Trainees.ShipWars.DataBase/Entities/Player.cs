
namespace Yoo.Trainees.ShipWars.DataBase.Entities
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<GamePlayer> GamePlayers { get; set; }

    }
}