using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Game.Dice;

namespace Catan.Source.Scenes.Game
{
    public class WaitingForDiceRollGameState : GameState
    {
        private DiceRollControl _diceRollControl;

        public WaitingForDiceRollGameState(GameScene gameScene, DiceRollControl diceRollControl)
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
                _gameScene.ExitState();
            }
        }
    }
}
