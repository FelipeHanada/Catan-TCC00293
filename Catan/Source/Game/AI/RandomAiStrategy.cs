using System;
using System.Collections.Generic;
using System.Linq;
using Catan.Source.Game.Board;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.AI
{
    public class RandomAiStrategy
    {
        private const double TradeAcceptanceChance = 0.25;
        private const double BuildSettlementWhenPossibleChance = 0.90;
        private const double BuildRoadWhileWaitingForSettlementChance = 0.03;
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
                .Select(vertex => new WeightedCandidate<TileVertex>(vertex, GetSettlementVertexWeight(board, vertex)))
                .ToList();

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        public TileVertex ChooseSettlement(Board.Board board, IEnumerable<TileVertex> validVertices)
        {
            return ChooseSetupSettlement(board, validVertices);
        }

        public TileEdge ChooseRoad(IEnumerable<TileEdge> validEdges)
        {
            List<TileEdge> edges = validEdges.ToList();
            return edges[_random.Next(edges.Count)];
        }

        public TileEdge ChooseRoad(Board.Board board, IEnumerable<TileEdge> validEdges)
        {
            List<WeightedCandidate<TileEdge>> candidates = validEdges
                .Select(edge => new WeightedCandidate<TileEdge>(edge, GetRoadWeight(board, edge)))
                .ToList();

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        public TileEdge ChooseRoad(Board.Board board, BoardGraph graph, GamePlayer player, IEnumerable<TileEdge> validEdges)
        {
            List<TileEdge> usefulEdges = GetUsefulRoadCandidates(board, graph, validEdges).ToList();
            if (usefulEdges.Count == 0)
            {
                return null;
            }

            List<TileVertex> fronts = GetRoadFrontCandidates(graph, player, usefulEdges).ToList();
            if (fronts.Count == 0)
            {
                return ChooseRoadByFutureSettlementWeight(board, graph, usefulEdges);
            }

            List<WeightedCandidate<TileVertex>> frontCandidates = fronts
                .Select(front => new WeightedCandidate<TileVertex>(front, GetRoadFrontWeight(board, graph, usefulEdges, front)))
                .ToList();
            TileVertex chosenFront = WeightedRandomPicker.Pick(frontCandidates, _random);

            List<TileEdge> localEdges = GetLocalRoadCandidates(usefulEdges, chosenFront).ToList();
            if (localEdges.Count == 0)
            {
                return ChooseRoadByFutureSettlementWeight(board, graph, usefulEdges);
            }

            return ChooseRoadByFutureSettlementWeight(board, graph, localEdges);
        }

        public Tile ChooseRobberTile(Board.Board board, GamePlayer currentPlayer, ScoreManager scoreManager)
        {
            RobberRule robberRule = new();
            List<Tile> movableTiles = board.Tiles
                .Where(board.CanMoveRobberTo)
                .ToList();

            Dictionary<ResourceId, int> productionCounts = GetResourceProductionCounts(board, currentPlayer);
            List<ResourceId> missingProductionResources = ResourceUtils.ResourceIds
                .Where(resource => productionCounts[resource] == 0 &&
                    currentPlayer.Inventory.Resources.GetAmount(resource) <= 1)
                .ToList();

            Tile neededResourceTile = ChooseRobberTileForResources(
                movableTiles,
                currentPlayer,
                scoreManager,
                robberRule,
                missingProductionResources);

            if (neededResourceTile != null)
            {
                return neededResourceTile;
            }

            List<ResourceId> scarceProducedResources = ResourceUtils.ResourceIds
                .Where(resource => productionCounts[resource] == 1 &&
                    currentPlayer.Inventory.Resources.GetAmount(resource) == 0)
                .ToList();

            Tile scarceResourceTile = ChooseRobberTileForResources(
                movableTiles,
                currentPlayer,
                scoreManager,
                robberRule,
                scarceProducedResources);

            if (scarceResourceTile != null)
            {
                return scarceResourceTile;
            }

            List<WeightedCandidate<Tile>> candidates = new();

            foreach (Tile tile in movableTiles)
            {
                int weight = 0;

                foreach (GamePlayer target in robberRule.GetRobberyTargets(currentPlayer, tile))
                {
                    if (target.Inventory.HasResources())
                    {
                        weight += 1 + GetScore(scoreManager, target);
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

        public bool ShouldBuildSettlementWhenPossible()
        {
            return _random.NextDouble() < BuildSettlementWhenPossibleChance;
        }

        public bool ShouldBuildRoadWhileWaitingForSettlement(int missingSettlementResourceCount)
        {
            if (missingSettlementResourceCount <= 1)
            {
                return false;
            }

            return _random.NextDouble() < BuildRoadWhileWaitingForSettlementChance;
        }

        public static int GetSettlementVertexWeight(Board.Board board, TileVertex vertex)
        {
            int diceWeightSum = 0;
            int validTileCount = 0;
            HashSet<ResourceId> resourceTypes = new();

            foreach (Tile tile in board.Tiles)
            {
                if (!tile.Vertices.Contains(vertex) ||
                    tile.ProducedResource is not ResourceId resource)
                {
                    continue;
                }

                validTileCount++;
                resourceTypes.Add(resource);
                diceWeightSum += GetDiceWeight(tile.DiceNumber);
            }

            if (diceWeightSum == 0 || validTileCount == 0 || resourceTypes.Count == 0)
            {
                return 0;
            }

            return diceWeightSum * diceWeightSum
                * validTileCount * validTileCount * validTileCount
                * resourceTypes.Count * resourceTypes.Count;
        }

        public static int GetRoadWeight(Board.Board board, TileEdge edge)
        {
            int vertexAWeight = edge.VertexA.HasBuilding
                ? 0
                : GetSettlementVertexWeight(board, edge.VertexA);

            int vertexBWeight = edge.VertexB.HasBuilding
                ? 0
                : GetSettlementVertexWeight(board, edge.VertexB);

            return Math.Max(vertexAWeight, vertexBWeight);
        }

        public static int GetFutureSettlementRoadWeight(Board.Board board, BoardGraph graph, TileEdge edge)
        {
            int vertexAWeight = CanBecomeSettlementSpot(graph, edge.VertexA)
                ? GetSettlementVertexWeight(board, edge.VertexA)
                : 0;

            int vertexBWeight = CanBecomeSettlementSpot(graph, edge.VertexB)
                ? GetSettlementVertexWeight(board, edge.VertexB)
                : 0;

            int directWeight = Math.Max(vertexAWeight, vertexBWeight);
            int oneRoadAheadWeight = GetOneRoadAheadSettlementWeight(board, graph, edge) / 2;

            return Math.Max(directWeight, oneRoadAheadWeight);
        }

        public static IEnumerable<TileEdge> GetUsefulRoadCandidates(Board.Board board, BoardGraph graph, IEnumerable<TileEdge> validEdges)
        {
            return validEdges.Where(edge => GetFutureSettlementRoadWeight(board, graph, edge) > 0);
        }

        public static IEnumerable<TileEdge> GetLocalRoadCandidates(IEnumerable<TileEdge> validEdges, TileVertex front)
        {
            return validEdges.Where(edge => edge.VertexA == front || edge.VertexB == front);
        }

        public static int CountMissingResources(ResourceInventory inventory, IReadOnlyDictionary<ResourceId, int> cost)
        {
            int missingCount = 0;

            foreach (KeyValuePair<ResourceId, int> requiredResource in cost)
            {
                int missingAmount = requiredResource.Value - inventory.GetAmount(requiredResource.Key);
                if (missingAmount > 0)
                {
                    missingCount += missingAmount;
                }
            }

            return missingCount;
        }

        public static Dictionary<ResourceId, int> GetResourceProductionCounts(Board.Board board, GamePlayer player)
        {
            Dictionary<ResourceId, int> productionCounts = new();
            foreach (ResourceId resource in ResourceUtils.ResourceIds)
            {
                productionCounts[resource] = 0;
            }

            foreach (Tile tile in board.Tiles)
            {
                if (tile.ProducedResource is not ResourceId resource)
                {
                    continue;
                }

                foreach (Building building in tile.GetAdjacentBuildings())
                {
                    if (building.Owner == player)
                    {
                        productionCounts[resource]++;
                    }
                }
            }

            return productionCounts;
        }

        private TileEdge ChooseRoadByFutureSettlementWeight(Board.Board board, BoardGraph graph, IEnumerable<TileEdge> validEdges)
        {
            List<WeightedCandidate<TileEdge>> candidates = validEdges
                .Select(edge => new WeightedCandidate<TileEdge>(edge, GetFutureSettlementRoadWeight(board, graph, edge)))
                .ToList();

            return WeightedRandomPicker.Pick(candidates, _random);
        }

        private Tile ChooseRobberTileForResources(
            IEnumerable<Tile> movableTiles,
            GamePlayer currentPlayer,
            ScoreManager scoreManager,
            RobberRule robberRule,
            IReadOnlyCollection<ResourceId> resources)
        {
            if (resources.Count == 0)
            {
                return null;
            }

            List<WeightedCandidate<Tile>> candidates = new();

            foreach (Tile tile in movableTiles)
            {
                if (tile.ProducedResource is not ResourceId resource ||
                    !resources.Contains(resource))
                {
                    continue;
                }

                int bestTargetWeight = 0;
                foreach (GamePlayer target in robberRule.GetRobberyTargets(currentPlayer, tile))
                {
                    if (target.Inventory.HasResources())
                    {
                        bestTargetWeight = Math.Max(bestTargetWeight, 1 + GetScore(scoreManager, target));
                    }
                }

                if (bestTargetWeight > 0)
                {
                    candidates.Add(new WeightedCandidate<Tile>(tile, bestTargetWeight));
                }
            }

            return candidates.Count == 0
                ? null
                : WeightedRandomPicker.Pick(candidates, _random);
        }

        private static int GetScore(ScoreManager scoreManager, GamePlayer player)
        {
            return scoreManager?.GetScore(player) ?? 0;
        }

        public static IEnumerable<TileVertex> GetRoadFrontCandidates(BoardGraph graph, GamePlayer player, IEnumerable<TileEdge> validEdges)
        {
            HashSet<TileVertex> roadEndpointFronts = new();
            HashSet<TileVertex> settlementFronts = new();

            foreach (TileEdge edge in validEdges)
            {
                AddFrontIfConnectedToPlayer(graph, player, edge.VertexA, roadEndpointFronts, settlementFronts);
                AddFrontIfConnectedToPlayer(graph, player, edge.VertexB, roadEndpointFronts, settlementFronts);
            }

            return roadEndpointFronts.Count > 0
                ? roadEndpointFronts
                : settlementFronts;
        }

        private static void AddFrontIfConnectedToPlayer(
            BoardGraph graph,
            GamePlayer player,
            TileVertex vertex,
            HashSet<TileVertex> roadEndpointFronts,
            HashSet<TileVertex> settlementFronts)
        {
            if (vertex.HasBuilding && vertex.Building.Owner == player)
            {
                settlementFronts.Add(vertex);
                return;
            }

            foreach (TileEdge edge in graph.Incident[vertex])
            {
                if (edge.RoadOwner == player)
                {
                    roadEndpointFronts.Add(vertex);
                    return;
                }
            }
        }

        private static int GetRoadFrontWeight(Board.Board board, BoardGraph graph, IEnumerable<TileEdge> validEdges, TileVertex front)
        {
            return GetLocalRoadCandidates(validEdges, front)
                .Select(edge => GetFutureSettlementRoadWeight(board, graph, edge))
                .DefaultIfEmpty(0)
                .Max();
        }

        private static int GetOneRoadAheadSettlementWeight(Board.Board board, BoardGraph graph, TileEdge edge)
        {
            int vertexAWeight = GetOneRoadAheadSettlementWeightFrom(board, graph, edge, edge.VertexA);
            int vertexBWeight = GetOneRoadAheadSettlementWeightFrom(board, graph, edge, edge.VertexB);

            return Math.Max(vertexAWeight, vertexBWeight);
        }

        private static int GetOneRoadAheadSettlementWeightFrom(Board.Board board, BoardGraph graph, TileEdge currentEdge, TileVertex roadFront)
        {
            if (roadFront.HasBuilding)
            {
                return 0;
            }

            int bestWeight = 0;

            foreach (TileEdge nextEdge in graph.Incident[roadFront])
            {
                if (nextEdge == currentEdge)
                {
                    continue;
                }

                TileVertex candidateVertex = nextEdge.VertexA == roadFront
                    ? nextEdge.VertexB
                    : nextEdge.VertexA;

                if (CanBecomeSettlementSpot(graph, candidateVertex))
                {
                    bestWeight = Math.Max(bestWeight, GetSettlementVertexWeight(board, candidateVertex));
                }
            }

            return bestWeight;
        }

        private static bool CanBecomeSettlementSpot(BoardGraph graph, TileVertex vertex)
        {
            if (vertex.HasBuilding)
            {
                return false;
            }

            foreach (TileEdge edge in graph.Incident[vertex])
            {
                TileVertex otherVertex = edge.VertexA == vertex ? edge.VertexB : edge.VertexA;
                if (otherVertex.HasBuilding)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
