using System.Collections.Generic;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class SetupGameState : GameState
    {
        private Queue<GameState> _stateQueue; 

        public SetupGameState(GameScene gameScene)
            : base(gameScene)
        {
            _stateQueue = [];

            foreach (Player player in gameScene._players)
            {
                _stateQueue.Enqueue(new PositionSettlementGameState(gameScene, player));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_stateQueue.Count > 0)
            {
                _gameScene.AppendState(_stateQueue.Dequeue());
            } else
            {
                // _gameScene.ExitState();
                // add next state
            }
        }
    }
}
