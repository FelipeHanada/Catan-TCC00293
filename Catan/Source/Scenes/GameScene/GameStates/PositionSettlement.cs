using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionSettlementGameState : GameState
    {
        private Player _player;
        public PositionSettlementGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            _player = player;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
