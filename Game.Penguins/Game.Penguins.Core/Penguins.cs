using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;

namespace Game.Penguins
{
    public class Penguins : IPenguin
    {
        public Penguins(PlayerClass player)
        {
            PlayerObject = player;
        }
        public IPlayer Player
        {
            get
            {
                return PlayerObject;
            }
        }

        private PlayerClass PlayerObject { get; set; }
    }
}
