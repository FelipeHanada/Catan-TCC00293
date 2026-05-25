using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionRoadGameState : GameState
    {
        public Player Player { get; private set; }
        public PositionRoadGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
