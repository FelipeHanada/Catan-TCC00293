using DiagnosticsDebug = System.Diagnostics.Debug;
using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;
using Catan.Source.Content;

namespace Catan.Source.Scenes.Game
{
    public class MoveRobberGameState : GameState
    {
        private const double AiActionDelaySeconds = 0.35;
        private readonly Player _currentPlayer;
        private double _aiElapsedSeconds;
        private bool _aiActed;

        public MoveRobberGameState(GameScene gameScene, Player currentPlayer)
            : base(gameScene)
        {
            _currentPlayer = currentPlayer;
            _aiElapsedSeconds = 0;
            _aiActed = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_currentPlayer.IsAi || _aiActed)
            {
                return;
            }

            _aiElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_aiElapsedSeconds < AiActionDelaySeconds)
            {
                return;
            }

            _aiActed = true;

            Tile tile = _gameScene.AiStrategy.ChooseRobberTile(
                _gameScene.Board,
                _currentPlayer,
                _gameScene.ScoreManager);

            TryMoveRobber(tile);
        }

        public bool TryMoveRobber(Tile tile)
        {
            if (!_gameScene.Board.MoveRobberTo(tile))
            {
                return false;
            }

            OnPlaceRobber(tile);
            _gameScene.ExitState();
            return true;
        }

        public virtual void OnPlaceRobber(Tile tile)
        {
            SoundManager.Instance.Play(SfxId.LadraoDado7);
        }
    }
}
