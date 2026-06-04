using System;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState, PlayerTradeButtonCallback, PlayerBuildButtonCallback, PlayerDevelopmentCardButtonCallback, PlayerEndTurnButtonCallback
    {
        private readonly Player _player;
        // private KeyboardState _previousKeyboardState;

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _player = player;
            // _previousKeyboardState = Keyboard.GetState();
        }

        public void OnPlayerBuildButtonClicked()
        {
            if (_gameScene.GetCurrentStateGame() is BuildingGameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new BuildingGameState(_gameScene, _player));
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
                _gameScene.AppendState(new TradingGameState(_gameScene, _player));
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
                _gameScene.AppendState(new DevelopmentCardState(_gameScene, _player));
            }
        }

        public void OnPlayerEndTurnButtonClicked()
        {
            _gameScene.ExitState();
        }

        // public override void Update(GameTime gameTime)
        // {
        //     KeyboardState keyboardState = Keyboard.GetState();

        //     // Controles temporarios ate existir UI de acoes do jogador:
        //     // T abre a selecao de troca maritima. A taxa pode ser 4:1, 3:1 ou 2:1,
        //     // dependendo dos portos acessiveis pelo jogador no tabuleiro.
        //     if (IsJustPressed(keyboardState, Keys.T))
        //     {
        //         _gameScene.AppendState(new MaritimeTradeSelectionGameState(_gameScene, _player));
        //     }

        //     // Enter encerrar as acoes temporarias do jogador e volta ao fluxo atual do turno.
        //     if (IsJustPressed(keyboardState, Keys.Enter))
        //     {
        //         _gameScene.ExitState();
        //     }

        //     _previousKeyboardState = keyboardState;
        // }

        // private bool IsJustPressed(KeyboardState currentState, Keys key)
        // {
        //     return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        // }
    }
}
