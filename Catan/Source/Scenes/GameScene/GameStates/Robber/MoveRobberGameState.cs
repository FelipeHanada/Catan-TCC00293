using DiagnosticsDebug = System.Diagnostics.Debug;
using Catan.Source.Game.Board;
using Microsoft.Xna.Framework;
using Catan.Source.Content;

namespace Catan.Source.Scenes.Game
{
    public class MoveRobberGameState : GameState
    {
        public MoveRobberGameState(GameScene gameScene)
            : base(gameScene)
        {
        }

        public virtual void OnPlaceRobber(Tile tile)
        {
            SoundManager.Instance.Play(SfxId.LadraoDado7);
        }
    }
}
