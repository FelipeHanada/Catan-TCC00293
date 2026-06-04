using System;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState
    {
        private readonly Player _player;
        private KeyboardState _previousKeyboardState;

        public Button BuildButton { get; private set; }
        public Button TradeButton { get; private set; }
        public Button EndTurnButton { get; private set; }
        public Button DevelopmentCardButton { get; private set; }

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _player = player;
            _previousKeyboardState = Keyboard.GetState();

            TradeButton = new ButtonAction(840, 620, gameScene.Atlas, 75, 30, EnableTradeSlate, "Trocar");
            BuildButton = new ButtonAction(930, 620, gameScene.Atlas, 75, 30, EnableBuildSlate, "Construir");
            DevelopmentCardButton = new ButtonAction(1020, 620, gameScene.Atlas, 75, 30, EnableDevelopmentCardState , "Usar");
            EndTurnButton = new ButtonAction(880, 660, gameScene.Atlas, 175, 30, EndTurn, "Terminar turno");

            AddChild(BuildButton);
            AddChild(TradeButton);
            AddChild(EndTurnButton);
            AddChild(DevelopmentCardButton);
        }

        private void EnableBuildSlate()
        {
            if (_gameScene.GetCurrentStateGame() is BuildingGameState gameState)
            {
                _gameScene.ExitState();
            } else
            {
                _gameScene.AppendState(new BuildingGameState(_gameScene, _player));
            }
        }

        private void EnableTradeSlate() {
            if (_gameScene.GetCurrentStateGame() is TradingGameState gameState)
            {
                _gameScene.ExitState();
            } else
            {
                _gameScene.AppendState(new TradingGameState(_gameScene, _player));
            }
        }

        private void EnableDevelopmentCardState() {
            if (_gameScene.GetCurrentStateGame() is DevelopmentCardState gameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new DevelopmentCardState(_gameScene, _player));
            }
        }

        private void EndTurn()
        {
            _player.Inventory.DevelopmentCards.ReleaseNewCards();
            _gameScene.ExitState();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Controles temporarios ate existir UI de acoes do jogador:
            // T abre a selecao de troca maritima. A taxa pode ser 4:1, 3:1 ou 2:1,
            // dependendo dos portos acessiveis pelo jogador no tabuleiro.
            if (IsJustPressed(keyboardState, Keys.T))
            {
                _gameScene.AppendState(new MaritimeTradeSelectionGameState(_gameScene, _player));
            }

            // Enter encerrar as acoes temporarias do jogador e volta ao fluxo atual do turno.
            if (IsJustPressed(keyboardState, Keys.Enter))
            {
                EndTurn();
            }

            _previousKeyboardState = keyboardState;
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}
