using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Catan.Source.Game.Player;
using Catan.Source.Game.Board;
using Catan.Source.Game.Dice;

namespace Catan.Source.Scenes.Game
{
    public class SetupGameState : GameState
    {
        private readonly Queue<GameState> _stateQueue;
        private readonly DiceRollControl _diceRollControl;

        public SetupGameState(GameScene gameScene, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _stateQueue = [];
            _diceRollControl = diceRollControl;

            Stack<Player> stk = new();
            foreach (Player player in gameScene._players)
            {
                _stateQueue.Enqueue(new SetupPositionSettlementGameState(gameScene, player, BuildingType.Settlement));
                stk.Push(player);
            }

            while (stk.Count > 0)
            {
                Player player = stk.Pop();
                _stateQueue.Enqueue(new SetupPositionSettlementGameState(gameScene, player, BuildingType.Settlement, true));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_stateQueue.Count > 0)
            {
                _gameScene.AppendState(_stateQueue.Dequeue());
            }
            else
            {
                _gameScene.ExitState();
                _gameScene.AppendState(new ResourceProductionGameState(_gameScene, _gameScene._players, _diceRollControl));
            }
        }
    }
}
