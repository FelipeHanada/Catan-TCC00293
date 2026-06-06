using Catan.Source.Game.AI;
using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using BoardModel = Catan.Source.Game.Board.Board;
using HarborModel = Catan.Source.Game.Harbor.Harbor;

namespace Catan.Tests.Source.Game.AI
{
    public class RandomAiStrategyTests
    {
        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 3)]
        [InlineData(5, 4)]
        [InlineData(6, 5)]
        [InlineData(7, 0)]
        [InlineData(8, 5)]
        [InlineData(9, 4)]
        [InlineData(10, 3)]
        [InlineData(11, 2)]
        [InlineData(12, 1)]
        public void GetDiceWeight_ReturnsExpectedWeight(int diceNumber, int expected)
        {
            Assert.Equal(expected, RandomAiStrategy.GetDiceWeight(diceNumber));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void GetDiceWeight_WithInvalidDiceNumber_ReturnsZero(int diceNumber)
        {
            Assert.Equal(0, RandomAiStrategy.GetDiceWeight(diceNumber));
        }

        [Fact]
        public void GetSettlementVertexWeight_WithDiverseHighProbabilityTiles_HeavilyOutweighsWeakVertex()
        {
            TileVertex strongVertex = Vertex();
            TileVertex weakVertex = Vertex();
            BoardModel board = BoardWith(
                Tile(TileType.Forest, 6, strongVertex),
                Tile(TileType.Brick, 8, strongVertex),
                Tile(TileType.Farm, 5, strongVertex),
                Tile(TileType.Sheep, 2, weakVertex));

            int strongWeight = RandomAiStrategy.GetSettlementVertexWeight(board, strongVertex);
            int weakWeight = RandomAiStrategy.GetSettlementVertexWeight(board, weakVertex);

            Assert.Equal(47628, strongWeight);
            Assert.Equal(1, weakWeight);
            Assert.True(strongWeight > weakWeight * 1000);
        }

        [Fact]
        public void GetSettlementVertexWeight_TreatsDesertAsEmptyNeighbor()
        {
            TileVertex vertex = Vertex();
            BoardModel board = BoardWith(
                Tile(TileType.Desert, 6, vertex),
                Tile(TileType.Sheep, 8, vertex),
                Tile(TileType.Sheep, 5, vertex));

            int weight = RandomAiStrategy.GetSettlementVertexWeight(board, vertex);

            Assert.Equal(648, weight);
        }

        [Fact]
        public void GetRoadWeight_ReturnsBestFutureSettlementWeightFromRoadEndpoints()
        {
            TileVertex strongVertex = Vertex();
            TileVertex weakVertex = Vertex();
            TileEdge edge = Edge(strongVertex, weakVertex);
            BoardModel board = BoardWith(
                Tile(TileType.Forest, 6, strongVertex),
                Tile(TileType.Brick, 8, strongVertex),
                Tile(TileType.Farm, 5, strongVertex),
                Tile(TileType.Sheep, 2, weakVertex));

            int weight = RandomAiStrategy.GetRoadWeight(board, edge);

            Assert.Equal(47628, weight);
        }

        [Fact]
        public void GetCityVertexWeight_UsesSettlementWeightForExistingSettlement()
        {
            TileVertex vertex = Vertex();
            BoardModel board = BoardWith(
                Tile(TileType.Forest, 6, vertex),
                Tile(TileType.Brick, 8, vertex),
                Tile(TileType.Farm, 5, vertex));

            int cityWeight = RandomAiStrategy.GetCityVertexWeight(board, vertex);

            Assert.Equal(RandomAiStrategy.GetSettlementVertexWeight(board, vertex), cityWeight);
        }

        [Fact]
        public void GetFutureSettlementRoadWeight_IgnoresEndpointsBlockedByDistanceRule()
        {
            Player player = new(0);
            TileVertex blockedVertex = Vertex();
            TileVertex adjacentBuildingVertex = Vertex();
            TileEdge blockedEdge = Edge(blockedVertex, adjacentBuildingVertex);
            adjacentBuildingVertex.PlaceBuilding(new Building(player, BuildingType.Settlement));

            BoardGraph graph = Graph(blockedEdge);
            BoardModel board = BoardWith(
                Tile(TileType.Forest, 6, blockedVertex),
                Tile(TileType.Brick, 8, blockedVertex),
                Tile(TileType.Farm, 5, blockedVertex));

            int weight = RandomAiStrategy.GetFutureSettlementRoadWeight(board, graph, blockedEdge);

            Assert.Equal(0, weight);
        }

        [Fact]
        public void GetFutureSettlementRoadWeight_ValuesRoadThatCanOpenSettlementWithOneMoreRoad()
        {
            Player player = new(0);
            TileVertex settlementVertex = Vertex();
            TileVertex roadFront = Vertex();
            TileVertex futureSettlementVertex = Vertex();
            settlementVertex.PlaceBuilding(new Building(player, BuildingType.Settlement));
            TileEdge firstRoad = Edge(settlementVertex, roadFront);
            TileEdge nextRoad = Edge(roadFront, futureSettlementVertex);

            BoardGraph graph = Graph(firstRoad, nextRoad);
            BoardModel board = BoardWith(
                Tile(TileType.Forest, 6, futureSettlementVertex),
                Tile(TileType.Brick, 8, futureSettlementVertex),
                Tile(TileType.Farm, 5, futureSettlementVertex));

            int weight = RandomAiStrategy.GetFutureSettlementRoadWeight(board, graph, firstRoad);

            Assert.Equal(23814, weight);
        }

        [Fact]
        public void GetLocalRoadCandidates_ReturnsOnlyEdgesIncidentToChosenFront()
        {
            Player player = new(0);
            TileVertex ownedSettlement = Vertex();
            TileVertex chosenFront = Vertex();
            TileVertex otherFront = Vertex();
            TileVertex localTarget = Vertex();
            TileVertex distantTarget = Vertex();

            ownedSettlement.PlaceBuilding(new Building(player, BuildingType.Settlement));
            TileEdge ownedRoadToChosenFront = Edge(ownedSettlement, chosenFront);
            TileEdge ownedRoadToOtherFront = Edge(ownedSettlement, otherFront);
            TileEdge localCandidate = Edge(chosenFront, localTarget);
            TileEdge distantCandidate = Edge(otherFront, distantTarget);
            ownedRoadToChosenFront.PlaceRoad(player);
            ownedRoadToOtherFront.PlaceRoad(player);

            BoardGraph graph = Graph(ownedRoadToChosenFront, ownedRoadToOtherFront, localCandidate, distantCandidate);

            List<TileEdge> candidates = RandomAiStrategy
                .GetLocalRoadCandidates(new[] { localCandidate, distantCandidate }, chosenFront)
                .ToList();

            Assert.Single(candidates);
            Assert.Same(localCandidate, candidates[0]);
        }

        [Fact]
        public void GetRoadFrontCandidates_WhenRoadEndpointsExist_DoesNotUseOwnedSettlementAsFront()
        {
            Player player = new(0);
            TileVertex ownedSettlement = Vertex();
            TileVertex roadEndpoint = Vertex();
            TileVertex settlementSideTarget = Vertex();
            TileVertex endpointTarget = Vertex();
            ownedSettlement.PlaceBuilding(new Building(player, BuildingType.Settlement));

            TileEdge ownedRoad = Edge(ownedSettlement, roadEndpoint);
            TileEdge settlementSideCandidate = Edge(ownedSettlement, settlementSideTarget);
            TileEdge endpointCandidate = Edge(roadEndpoint, endpointTarget);
            ownedRoad.PlaceRoad(player);

            BoardGraph graph = Graph(ownedRoad, settlementSideCandidate, endpointCandidate);

            List<TileVertex> fronts = RandomAiStrategy
                .GetRoadFrontCandidates(graph, player, new[] { settlementSideCandidate, endpointCandidate })
                .ToList();

            Assert.Single(fronts);
            Assert.Same(roadEndpoint, fronts[0]);
        }

        [Fact]
        public void ChooseRobberTile_PrioritizesResourceCurrentPlayerDoesNotProduceAndHasAtMostOne()
        {
            Player currentPlayer = new(0);
            Player targetPlayer = PlayerWithResources(1);
            Tile brickTile = TileWithBuilding(TileType.Brick, targetPlayer);
            Tile forestTile = TileWithBuilding(TileType.Forest, targetPlayer);
            BoardModel board = BoardWith(
                TileWithBuilding(TileType.Forest, currentPlayer),
                TileWithBuilding(TileType.Sheep, currentPlayer),
                TileWithBuilding(TileType.Mountain, currentPlayer),
                TileWithBuilding(TileType.Farm, currentPlayer),
                brickTile,
                forestTile);
            RandomAiStrategy strategy = new(new FixedRandom(0));

            Tile chosenTile = strategy.ChooseRobberTile(board, currentPlayer, null);

            Assert.Same(brickTile, chosenTile);
        }

        [Fact]
        public void ChooseRobberTile_IgnoresNeededResourceTileWithoutRobberyTarget()
        {
            Player currentPlayer = new(0);
            Player targetPlayer = PlayerWithResources(1);
            Tile brickTileWithoutTarget = Tile(TileType.Brick, 6, Vertex());
            Tile forestTileWithTarget = TileWithBuilding(TileType.Forest, targetPlayer);
            BoardModel board = BoardWith(
                TileWithBuilding(TileType.Forest, currentPlayer),
                TileWithBuilding(TileType.Sheep, currentPlayer),
                TileWithBuilding(TileType.Mountain, currentPlayer),
                TileWithBuilding(TileType.Farm, currentPlayer),
                brickTileWithoutTarget,
                forestTileWithTarget);
            RandomAiStrategy strategy = new(new FixedRandom(0));

            Tile chosenTile = strategy.ChooseRobberTile(board, currentPlayer, null);

            Assert.Same(forestTileWithTarget, chosenTile);
        }

        [Fact]
        public void ChooseRobberTile_WhenProducedByOneBuildingAndInventoryIsZero_PrioritizesThatResource()
        {
            Player currentPlayer = new(0);
            currentPlayer.Inventory.Resources.Add(ResourceId.Brick, 2);
            Player targetPlayer = PlayerWithResources(1);
            Tile sheepTile = TileWithBuildings(TileType.Sheep, currentPlayer, targetPlayer);
            Tile brickTile = TileWithBuilding(TileType.Brick, targetPlayer);
            BoardModel board = BoardWith(
                TileWithBuilding(TileType.Forest, currentPlayer),
                TileWithBuilding(TileType.Brick, currentPlayer),
                TileWithBuilding(TileType.Mountain, currentPlayer),
                TileWithBuilding(TileType.Farm, currentPlayer),
                sheepTile,
                brickTile);
            RandomAiStrategy strategy = new(new FixedRandom(0));

            Tile chosenTile = strategy.ChooseRobberTile(board, currentPlayer, null);

            Assert.Same(sheepTile, chosenTile);
        }

        [Fact]
        public void GetTradeNeeds_WhenMissingOneResourceForSettlement_MarksHighPriorityPlayerTrade()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            player.Inventory.Resources.Add(ResourceId.Wheat, 1);
            BoardModel board = BoardWith(TileWithBuilding(TileType.Forest, player));

            List<AiTradeNeed> needs = RandomAiStrategy.GetTradeNeeds(
                board,
                player,
                SettlementCost(),
                RoadCost()).ToList();

            AiTradeNeed brickNeed = Assert.Single(needs, need => need.Resource == ResourceId.Brick);
            Assert.Equal(100, brickNeed.Weight);
            Assert.Equal(0.90, brickNeed.AttemptChance);
            Assert.False(brickNeed.PreferBank);
            Assert.Equal(2, brickNeed.PlayerOfferAmount);
        }

        [Fact]
        public void GetTradeNeeds_WhenMissingOneResourceForRoad_UsesLowerPriorityPlayerTrade()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Brick, 1);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            player.Inventory.Resources.Add(ResourceId.Wheat, 1);
            Dictionary<ResourceId, int> roadCost = new()
            {
                { ResourceId.Ore, 1 },
            };
            BoardModel board = BoardWith(
                TileWithBuilding(TileType.Forest, player),
                TileWithBuilding(TileType.Sheep, player),
                TileWithBuilding(TileType.Farm, player));

            List<AiTradeNeed> needs = RandomAiStrategy.GetTradeNeeds(
                board,
                player,
                SettlementCost(),
                roadCost).ToList();

            AiTradeNeed oreNeed = Assert.Single(needs, need => need.Resource == ResourceId.Ore);
            Assert.Equal(70, oreNeed.Weight);
            Assert.Equal(0.75, oreNeed.AttemptChance);
            Assert.False(oreNeed.PreferBank);
            Assert.Equal(2, oreNeed.PlayerOfferAmount);
        }

        [Fact]
        public void GetTradeNeeds_WhenMissingTwoOfUnproducedSettlementResource_UsesStrongCriticalPriority()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            Dictionary<ResourceId, int> settlementCost = SettlementCost();
            settlementCost[ResourceId.Brick] = 2;
            BoardModel board = BoardWith(
                TileWithBuilding(TileType.Forest, player),
                TileWithBuilding(TileType.Sheep, player));

            List<AiTradeNeed> needs = RandomAiStrategy.GetTradeNeeds(
                board,
                player,
                settlementCost,
                RoadCost()).ToList();

            AiTradeNeed brickNeed = Assert.Single(needs, need => need.Resource == ResourceId.Brick);
            Assert.Equal(75, brickNeed.Weight);
            Assert.Equal(0.75, brickNeed.AttemptChance);
            Assert.False(brickNeed.PreferBank);
        }

        [Fact]
        public void ChooseTradeOfferedResource_PrefersResourceWithExcessOverResourceNeededForSettlement()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Ore, 4);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            player.Inventory.Resources.Add(ResourceId.Wheat, 1);
            RandomAiStrategy strategy = new(new FixedRandom(0));

            ResourceId? offeredResource = strategy.ChooseTradeOfferedResource(
                player,
                ResourceId.Brick,
                SettlementCost(),
                RoadCost(),
                4);

            Assert.Equal(ResourceId.Ore, offeredResource);
        }

        [Fact]
        public void ChooseTradeOfferedResource_ReturnsNullWhenOnlyRequestedResourceHasEnoughCards()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Brick, 4);
            RandomAiStrategy strategy = new(new FixedRandom(0));

            ResourceId? offeredResource = strategy.ChooseTradeOfferedResource(
                player,
                ResourceId.Brick,
                SettlementCost(),
                RoadCost(),
                4);

            Assert.Null(offeredResource);
        }

        [Fact]
        public void ChooseBankTradeOfferedResource_UsesResourceSpecificTradeRate()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Wood, 2);
            RandomAiStrategy strategy = new(new FixedRandom(0));
            Dictionary<ResourceId, int> tradeRates = TradeRates(
                (ResourceId.Wood, 2),
                (ResourceId.Wool, 4),
                (ResourceId.Brick, 4),
                (ResourceId.Ore, 4),
                (ResourceId.Wheat, 4));

            ResourceId? offeredResource = strategy.ChooseBankTradeOfferedResource(
                player,
                ResourceId.Brick,
                SettlementCost(),
                RoadCost(),
                tradeRates);

            Assert.Equal(ResourceId.Wood, offeredResource);
        }

        [Fact]
        public void ChooseBankTradeOfferedResource_ReturnsNullWhenNoResourceMeetsItsTradeRate()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Wood, 2);
            player.Inventory.Resources.Add(ResourceId.Ore, 3);
            RandomAiStrategy strategy = new(new FixedRandom(0));
            Dictionary<ResourceId, int> tradeRates = TradeRates(
                (ResourceId.Wood, 3),
                (ResourceId.Wool, 4),
                (ResourceId.Brick, 4),
                (ResourceId.Ore, 4),
                (ResourceId.Wheat, 4));

            ResourceId? offeredResource = strategy.ChooseBankTradeOfferedResource(
                player,
                ResourceId.Brick,
                SettlementCost(),
                RoadCost(),
                tradeRates);

            Assert.Null(offeredResource);
        }

        [Fact]
        public void CountMissingResources_ReturnsHowManyCostEntriesAreStillMissing()
        {
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Brick, 1);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            Dictionary<ResourceId, int> settlementCost = new()
            {
                { ResourceId.Brick, 1 },
                { ResourceId.Wood, 1 },
                { ResourceId.Wool, 1 },
                { ResourceId.Wheat, 1 },
            };

            int missingCount = RandomAiStrategy.CountMissingResources(player.Inventory.Resources, settlementCost);

            Assert.Equal(1, missingCount);
        }

        [Fact]
        public void ShouldBuildRoadWhileWaitingForSettlement_WithOneMissingResource_ReturnsFalse()
        {
            RandomAiStrategy strategy = new(new FixedRandom(0));

            bool shouldBuildRoad = strategy.ShouldBuildRoadWhileWaitingForSettlement(1);

            Assert.False(shouldBuildRoad);
        }

        [Fact]
        public void ShouldBuildRoadWhileWaitingForSettlement_WithTwoMissingResources_UsesSmallChance()
        {
            RandomAiStrategy accepts = new(new FixedRandom(0.02));
            RandomAiStrategy rejects = new(new FixedRandom(0.04));

            Assert.True(accepts.ShouldBuildRoadWhileWaitingForSettlement(2));
            Assert.False(rejects.ShouldBuildRoadWhileWaitingForSettlement(2));
        }

        [Fact]
        public void ShouldBuildSettlementWhenPossible_UsesHighChance()
        {
            RandomAiStrategy accepts = new(new FixedRandom(0.89));
            RandomAiStrategy rejects = new(new FixedRandom(0.91));

            Assert.True(accepts.ShouldBuildSettlementWhenPossible());
            Assert.False(rejects.ShouldBuildSettlementWhenPossible());
        }

        [Theory]
        [InlineData(2, 0.14, 0.16)]
        [InlineData(3, 0.24, 0.26)]
        [InlineData(4, 0.39, 0.41)]
        [InlineData(5, 0.49, 0.51)]
        public void ShouldBuildCityWhenPossible_UsesChanceBySettlementCount(
            int settlementCount,
            double acceptedRoll,
            double rejectedRoll)
        {
            RandomAiStrategy accepts = new(new FixedRandom(acceptedRoll));
            RandomAiStrategy rejects = new(new FixedRandom(rejectedRoll));

            Assert.True(accepts.ShouldBuildCityWhenPossible(settlementCount));
            Assert.False(rejects.ShouldBuildCityWhenPossible(settlementCount));
        }

        private static TileVertex Vertex()
        {
            return new TileVertex(0, 0, null, null);
        }

        private static Tile Tile(TileType type, int diceNumber, TileVertex vertex)
        {
            return new Tile(0, 0, null, type, diceNumber, new[] { vertex }, null);
        }

        private static Tile TileWithBuilding(TileType type, Player player)
        {
            return TileWithBuildings(type, player);
        }

        private static Tile TileWithBuildings(TileType type, params Player[] players)
        {
            List<TileVertex> vertices = new();

            foreach (Player player in players)
            {
                TileVertex vertex = Vertex();
                vertex.PlaceBuilding(new Building(player, BuildingType.Settlement));
                vertices.Add(vertex);
            }

            while (vertices.Count < 3)
            {
                vertices.Add(Vertex());
            }

            return new Tile(0, 0, null, type, 6, vertices.ToArray(), null);
        }

        private static Dictionary<ResourceId, int> SettlementCost()
        {
            return new Dictionary<ResourceId, int>
            {
                { ResourceId.Brick, 1 },
                { ResourceId.Wood, 1 },
                { ResourceId.Wool, 1 },
                { ResourceId.Wheat, 1 },
            };
        }

        private static Dictionary<ResourceId, int> RoadCost()
        {
            return new Dictionary<ResourceId, int>
            {
                { ResourceId.Brick, 1 },
                { ResourceId.Wood, 1 },
            };
        }

        private static Dictionary<ResourceId, int> TradeRates(params (ResourceId Resource, int Rate)[] rates)
        {
            Dictionary<ResourceId, int> tradeRates = new();

            foreach ((ResourceId resource, int rate) in rates)
            {
                tradeRates[resource] = rate;
            }

            return tradeRates;
        }

        private static Player PlayerWithResources(int playerNumber)
        {
            Player player = new(playerNumber);
            player.Inventory.Resources.Add(ResourceId.Wood, 1);
            return player;
        }

        private static TileEdge Edge(TileVertex vertexA, TileVertex vertexB)
        {
            return new TileEdge(vertexA, vertexB, null, null);
        }

        private static BoardModel BoardWith(params Tile[] tiles)
        {
            return new BoardModel(0, 0, null, new List<Tile>(tiles), new List<HarborModel>(), new BoardGraph());
        }

        private static BoardGraph Graph(params TileEdge[] edges)
        {
            BoardGraph graph = new();

            foreach (TileEdge edge in edges)
            {
                graph.AddVertex(edge.VertexA);
                graph.AddVertex(edge.VertexB);
            }

            foreach (TileEdge edge in edges)
            {
                graph.AddEdge(edge);
            }

            return graph;
        }

        private class FixedRandom : System.Random
        {
            private readonly double _nextDouble;

            public FixedRandom(double nextDouble)
            {
                _nextDouble = nextDouble;
            }

            public override double NextDouble()
            {
                return _nextDouble;
            }
        }
    }
}
