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

        public WaitingForDiceRollGameState(GameScene gameScene, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _diceRollControl = diceRollControl;
            _diceRoller = new RandomDiceRoller();
            _previousMouseState = Mouse.GetState();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouseState = Mouse.GetState();

            if (_diceRollControl.WasClicked(mouseState, _previousMouseState))
            {
                _diceRollControl.StartRoll(_diceRoller.Roll());
                
                _gameScene.ExitState();
                _gameScene.AppendState(new RollingDiceGameState(_gameScene, _diceRollControl));
            }

            _previousMouseState = mouseState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
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
