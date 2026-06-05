using System;
using System.Collections.Generic;
using System.Linq;
using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.AI
{
    public class RandomAiStrategy
    {
        private const double TradeAcceptanceChance = 0.25;
        private readonly Random _random;

        public RandomAiStrategy(Random random = null)
        {
            _random = random ?? Random.Shared;
        }

        public static int GetDiceWeight(int diceNumber)
        {
            return diceNumber switch
            {
                2 => 1,
                3 => 2,
                4 => 3,
                5 => 4,
                6 => 5,
                8 => 5,
                9 => 4,
                10 => 3,
                11 => 2,
                12 => 1,
                _ => 0,
            };
        }

        public TileVertex ChooseSetupSettlement(Board.Board board, IEnumerable<TileVertex> validVertices)
        {
            List<WeightedCandidate<TileVertex>> candidates = validVertices
                .Select(vertex => new WeightedCandidate<TileVertex>(vertex, GetSetupVertexWeight(board, vertex)))
                .ToList();

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        public TileEdge ChooseRoad(IEnumerable<TileEdge> validEdges)
        {
            List<TileEdge> edges = validEdges.ToList();
            return edges[_random.Next(edges.Count)];
        }

        public Tile ChooseRobberTile(Board.Board board, GamePlayer currentPlayer, ScoreManager scoreManager)
        {
            RobberRule robberRule = new();
            List<WeightedCandidate<Tile>> candidates = new();

            foreach (Tile tile in board.Tiles.Where(board.CanMoveRobberTo))
            {
                int weight = 0;

                foreach (GamePlayer target in robberRule.GetRobberyTargets(currentPlayer, tile))
                {
                    if (target.Inventory.HasResources())
                    {
                        weight += 1 + scoreManager.GetScore(target);
                    }
                }

                candidates.Add(new WeightedCandidate<Tile>(tile, weight));
            }

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        public GamePlayer ChooseRobberyTarget(GamePlayer currentPlayer, Tile robberTile, ScoreManager scoreManager)
        {
            RobberRule robberRule = new();
            List<WeightedCandidate<GamePlayer>> candidates = robberRule
                .GetRobberyTargets(currentPlayer, robberTile)
                .Where(player => player.Inventory.HasResources())
                .Select(player => new WeightedCandidate<GamePlayer>(player, 1 + scoreManager.GetScore(player)))
                .ToList();

            if (candidates.Count == 0)
            {
                return null;
            }

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        public bool ShouldAcceptTrade()
        {
            return _random.NextDouble() < TradeAcceptanceChance;
        }

        private static int GetSetupVertexWeight(Board.Board board, TileVertex vertex)
        {
            int weight = 0;

            foreach (Tile tile in board.Tiles)
            {
                if (!tile.Vertices.Contains(vertex) || tile.ProducedResource is not ResourceId)
                {
                    continue;
                }

                weight += GetDiceWeight(tile.DiceNumber);
            }

            return weight;
        }
    }
}
