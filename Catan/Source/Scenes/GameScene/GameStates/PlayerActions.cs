using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : GameState
    {
        private readonly Player _player;
        private KeyboardState _previousKeyboardState;

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            _player = player;
            _previousKeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Controles temporários até existir UI de ações do jogador
            // T abre a seleção de troca com o banco.
            if (IsJustPressed(keyboardState, Keys.T))
            {
                _gameScene.AppendState(new BankTradeSelectionGameState(_gameScene, _player));
            }

            // Enter encerra as ações temporárias do jogador e volta ao fluxo atual do turno
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
