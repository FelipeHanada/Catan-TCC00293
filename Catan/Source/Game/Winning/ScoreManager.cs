using System.Linq;
using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Inventory;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;


namespace Catan.Source.Game.Player
{
    public class ScoreManager : GameObject
    {
        private GameScene _gameScene;
        private Dictionary<Player, int> _playerScores = new();
        public ScoreManager(GameScene gameScene)
            : base(0, 0)
        {
            _gameScene = gameScene;
            _playerScores = new Dictionary<Player, int>();
            foreach (Player player in _gameScene.Players)
            {
                _playerScores[player] = 0;
            }
        }

        public int GetScore(Player player)
        {
            return _playerScores.TryGetValue(player, out int score) ? score : 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Player player in _gameScene.Players)
            {
                int score = CalculateScore(player);
                _playerScores[player] = score;
            }
        }
        private int CalculateScore(Player player)
        {
            int score = 0;

            score += CalculateBuildingPoints(player);
            score += CalculateVictoryPointCards(player);
            
            // score += HasLongestRoad(player) ? 2 : 0; // [not implemented]
            // score += HasLargestArmy(player) ? 2 : 0; // [not implemented]

            return score;
        }
        private int CalculateBuildingPoints(Player player)
        {
            return _gameScene.Board.Graph.Vertices
                .Where(v => v.Building != null && v.Building.Owner == player)
                .Sum(v => v.Building.Type == BuildingType.Settlement ? 1 : 2);
        }
        private int CalculateVictoryPointCards(Player player)
        {
            if (player.Inventory?.DevelopmentCards == null)
                return 0;

            return player.Inventory.DevelopmentCards.CountByType(DevelopmentCardType.VictoryPoint);
        }
    }
}
