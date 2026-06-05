using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState, PlayerTradeButtonCallback, PlayerBuildButtonCallback, PlayerDevelopmentCardButtonCallback, PlayerEndTurnButtonCallback
    {
        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
        }

        public void OnPlayerBuildButtonClicked()
        {
            if (_gameScene.GetCurrentStateGame() is BuildingGameState)
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
            if (_gameScene.GetCurrentStateGame() is TradingGameState)
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
            if (_gameScene.GetCurrentStateGame() is DevelopmentCardState)
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
