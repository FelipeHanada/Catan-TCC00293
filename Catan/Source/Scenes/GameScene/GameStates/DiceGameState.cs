using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Game.Dice;

namespace Catan.Source.Scenes.Game
{
    public class WaitingForDiceRollGameState : GameState
    {
        private DiceRollControl _diceRollControl;
        private RandomDiceRoller _diceRoller;
        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;

        public WaitingForDiceRollGameState(GameScene gameScene, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _diceRollControl = diceRollControl;
            _diceRoller = new RandomDiceRoller();
            _previousMouseState = Mouse.GetState();
            _previousKeyboardState = Keyboard.GetState();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            #if DEBUG
            if (IsJustPressed(keyboardState, Keys.F7))
            {
                // Atalho de debug para testar o fluxo do ladrao sem depender de rolar 7.
                _gameScene.LastDiceRoll = new DiceRoll(3, 4);
                _gameScene.ExitState();
                return;
            }
            #endif

            if (_diceRollControl.WasClicked(mouseState, _previousMouseState))
            {
                _diceRollControl.StartRoll(_diceRoller.Roll());
                
                _gameScene.ExitState();
                _gameScene.AppendState(new RollingDiceGameState(_gameScene, _diceRollControl));
            }

            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }

    public class RollingDiceGameState : GameState
    {
        private DiceRollControl _diceRollControl;

        public RollingDiceGameState(GameScene gameScene, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _diceRollControl = diceRollControl;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_diceRollControl.HasSettledResult)
            {
                DiceRoll roll = _diceRollControl.ConsumeSettledResult();
                _gameScene.LastDiceRoll = roll;
                // ResolveDiceRoll(roll);

                _gameScene.ExitState();
                // _gameScene.AppendState(PlayerActions);
                // _turnPhase = TurnPhase.PlayerActions;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

    }
}
