using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState, PlayerTradeButtonCallback, PlayerBuildButtonCallback, PlayerDevelopmentCardButtonCallback, PlayerEndTurnButtonCallback
    {
        private const double AiActionDelaySeconds = 0.35;
        private double _aiElapsedSeconds;
        private bool _aiActed;

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _aiElapsedSeconds = 0;
            _aiActed = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Player.IsAi || _aiActed)
            {
                return;
            }

            _aiElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_aiElapsedSeconds < AiActionDelaySeconds)
            {
                return;
            }

            _aiActed = true;

            OnPlayerEndTurnButtonClicked();
        }

        public void OnPlayerBuildButtonClicked()
        {
            if (_gameScene.GetCurrentState() is BuildingGameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new BuildingGameState(_gameScene, Player));
            }
        }

        public void OnPlayerTradeButtonClicked()
        {
            if (_gameScene.GetCurrentState() is TradingGameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new TradingGameState(_gameScene, Player));
            }
        }

        public void OnPlayerDevelopmentCardButtonClicked()
        {
            if (_gameScene.GetCurrentState() is DevelopmentCardState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new DevelopmentCardState(_gameScene, Player));
            }
        }

        public void OnPlayerEndTurnButtonClicked()
        {
            Player.Inventory.DevelopmentCards.ReleaseNewCards();
            _gameScene.ResetDevelopmentCardUsageForTurn();
            _gameScene.ExitState();
        }

    }
}
