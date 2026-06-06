using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Player;
using System;

namespace Catan.Source.Scenes.Game
{
    public class WaitingForDiceRollGameState : GameState
    {
        private DiceRollControl _diceRollControl;
        private readonly Player _player;
        private bool _startedAiRoll;

        public WaitingForDiceRollGameState(GameScene gameScene, DiceRollControl diceRollControl, Player player)
            : base(gameScene)
        {
            _diceRollControl = diceRollControl;
            _player = player;
            _startedAiRoll = false;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_player.IsAi && !_startedAiRoll && !_diceRollControl.IsRolling && !_diceRollControl.HasSettledResult)
            {
                _startedAiRoll = true;
                _diceRollControl.StartRoll(new DiceRoll(Random.Shared.Next(1, 7), Random.Shared.Next(1, 7)));
                return;
            }

            if (_diceRollControl.HasSettledResult)
            {
                DiceRoll roll = _diceRollControl.ConsumeSettledResult();
                _gameScene.LastDiceRoll = roll;
                _gameScene.Log.Add($"Dado: {roll.First} + {roll.Second} = {roll.Total}");
                _gameScene.ExitState();
            }
        }
    }
}
