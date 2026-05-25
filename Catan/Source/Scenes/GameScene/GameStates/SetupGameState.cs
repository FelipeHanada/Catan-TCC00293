using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Catan.Source.Game.Player;
using Catan.Source.Game.Board;
using System.Security.Cryptography.X509Certificates;

namespace Catan.Source.Scenes.Game
{
    public class SetupGameState : GameState
    {
        private Queue<GameState> _stateQueue; 

        public SetupGameState(GameScene gameScene)
            : base(gameScene)
        {
            _stateQueue = [];

            Stack<Player> stk = new();
            foreach (Player player in gameScene._players)
            {
                _stateQueue.Enqueue(new PositionSettlementGameState(gameScene, player, BuildingType.Settlement));
                stk.Push(player);
            }

            while (stk.Count > 0)
            {
                Player player = stk.Pop();
                _stateQueue.Enqueue(new PositionSettlementGameState(gameScene, player, BuildingType.Settlement));
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
