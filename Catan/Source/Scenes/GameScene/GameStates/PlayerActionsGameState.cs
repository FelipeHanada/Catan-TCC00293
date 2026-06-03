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

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _player = player;
            _previousKeyboardState = Keyboard.GetState();

            Action EnableBuildSlate = () =>
            {
                if (gameScene.GetCurrentStateGame() is BuildingGameState gameState)
                {
                    gameScene.ExitState();
                } else
                {
                    gameScene.AppendState(new BuildingGameState(gameScene, player));
                }
            };

            Action EnableTradeSlate = () => {
                if (gameScene.GetCurrentStateGame() is TradingGameState gameState)
                {
                    gameScene.ExitState();
                } else
                {
                    gameScene.AppendState(new TradingGameState(gameScene, player));
                }
            };

            BuildButton = new ButtonAction(1000, 620, gameScene.Atlas, 75, 30, EnableBuildSlate, "Construir");
            TradeButton = new ButtonAction(900, 620, gameScene.Atlas, 75, 30, EnableTradeSlate, "Trocar");

            AddChild(BuildButton);
            AddChild(TradeButton);
            AddChild(new ButtonAction(900, 660, gameScene.Atlas, 175, 30, () => { }, "Terminar turno"));
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
                _gameScene.ExitState();
            }

            _previousKeyboardState = keyboardState;
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}
