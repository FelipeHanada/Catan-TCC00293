using System.Collections.Generic;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PlayerTurnManagerGameState : GameState
    {
        public List<Player> Players { get; private set; }
        public int CurrentPlayerIndex { get; private set; }
        
        public PlayerTurnManagerGameState(GameScene gameScene, List<Player> players)
            : base(gameScene)
        {
            Players = players;
            CurrentPlayerIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            Player currentPlayer = Players[CurrentPlayerIndex];
            _gameScene.AppendState(new PlayerActionsGameState(_gameScene, currentPlayer));
            if (++CurrentPlayerIndex >= Players.Count) CurrentPlayerIndex = 0;
        }
    }
}
