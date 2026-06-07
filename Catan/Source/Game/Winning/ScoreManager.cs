using System.Linq;
using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Inventory;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using System;


namespace Catan.Source.Game.Player
{
    public class ScoreManager : GameObject
    {
        public const int ScoreToWin = 10;
        private GameScene _gameScene;
        private Dictionary<Player, int> _playerScores;
        public ScoreManager(GameScene gameScene)
            : base(0, 0)
        {
            _gameScene = gameScene;
            _playerScores = [];
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

            Player hasLongestRoad = UpdateLongestRoad();
            Player hasLargestArmy = UpdateLargestArmy();

            foreach (Player player in _gameScene.Players)
            {
                int score = CalculateScore(player, hasLongestRoad == player, hasLargestArmy == player);
                _playerScores[player] = score;

                if (score >= ScoreToWin)
                {
                    Game1.ChangeScene(new EndGameScene(player));
                }
            }
        }
        private int CalculateScore(Player player, bool hasLongestRoad, bool hasLargestArmy)
        {
            int score = 0;

            score += CalculateBuildingPoints(player);
            score += CalculateVictoryPointCards(player);
            score += hasLongestRoad ? 2 : 0;
            score += hasLargestArmy ? 2 : 0;

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

        private Player UpdateLongestRoad()
        {
            BoardGraph graph = _gameScene.Board.Graph;

            Player hasLongestRoad = null;
            int longestRoad = 4;

            foreach (Player player in _gameScene.Players)
            {
                HashSet<TileVertex> ends = [.. graph.Vertices.Where(
                    (vertex) =>
                    {
                        List<TileEdge> roads = [.. graph.Incident[vertex].Where(edge => edge.RoadOwner != null)];
                        List<TileEdge> own_roads = [.. roads.Where(road => road.RoadOwner == player)];
                        if (own_roads.Count == 0) return false;
                        if (own_roads.Count == 1) return true;
                        return own_roads.Count < roads.Count;
                    }
                )];

                int maxLength = 0;

                // Função local de DFS para buscar o maior caminho entre dois nós do conjunto 'ends'
                int DFS(TileVertex current, HashSet<TileVertex> visited)
                {
                    int maxPath = -1; // -1 indica que o caminho não encontrou outro vértice 'end'

                    foreach (TileEdge edge in graph.Incident[current])
                    {
                        if (edge.RoadOwner != player) continue;

                        TileVertex neighbor = edge.VertexA == current ? edge.VertexB : edge.VertexA;

                        if (visited.Contains(neighbor)) continue;

                        if (ends.Contains(neighbor))
                        {
                            maxPath = Math.Max(maxPath, 1);
                        }
                        else
                        {
                            visited.Add(neighbor);
                            int subPathLength = DFS(neighbor, visited);
                            visited.Remove(neighbor);

                            if (subPathLength != -1)
                            {
                                maxPath = Math.Max(maxPath, 1 + subPathLength);
                            }
                        }
                    }

                    return maxPath;
                }

                foreach (TileVertex startNode in ends)
                {
                    HashSet<TileVertex> visited = [startNode];
                    int pathLength = DFS(startNode, visited);
                    
                    if (pathLength > maxLength)
                    {
                        maxLength = pathLength;
                    }
                }

                if (maxLength == longestRoad)
                {
                    hasLongestRoad = null;
                } else if (maxLength > longestRoad)
                {
                    hasLongestRoad = player;
                    longestRoad = maxLength;
                }
            }

            return hasLongestRoad;
        }

        private Player UpdateLargestArmy()
        {
            Player hasLargestArmy = null;
            int largestArmy = 0;
            foreach (Player player in _gameScene.Players)
            {
                int playedKnightsCount = player.Inventory.PlayedKnightsCount;
                if (playedKnightsCount == largestArmy)
                {
                    hasLargestArmy = null;
                } else if (playedKnightsCount > largestArmy)
                {
                    hasLargestArmy = player;
                    largestArmy = playedKnightsCount;
                }
            }

            return hasLargestArmy;
        }
    }
}
