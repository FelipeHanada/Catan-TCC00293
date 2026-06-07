using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;
using BoardModel = Catan.Source.Game.Board.Board;

namespace Catan.Tests.Source.Game.Board
{
    public class BoardDomainTests
    {
        [Fact]
        public void BoardGraph_AddEdge_RequiresBothVerticesInGraph()
        {
            var graph = new BoardGraph();
            TileVertex vertexInGraph = Vertex();
            TileVertex vertexOutsideGraph = Vertex();
            graph.AddVertex(vertexInGraph);
            TileEdge edge = Edge(vertexInGraph, vertexOutsideGraph);

            Assert.Throws<InvalidOperationException>(() => graph.AddEdge(edge));
            Assert.Empty(graph.Edges);
            Assert.Empty(graph.Incident[vertexInGraph]);
        }

        [Fact]
        public void BoardGraph_AddEdge_StoresIncidentEdgesForBothVertices()
        {
            var graph = new BoardGraph();
            TileVertex first = Vertex();
            TileVertex second = Vertex();
            TileEdge edge = Edge(first, second);
            graph.AddVertex(first);
            graph.AddVertex(second);

            graph.AddEdge(edge);

            Assert.Contains(edge, graph.Edges);
            Assert.Contains(edge, graph.Incident[first]);
            Assert.Contains(edge, graph.Incident[second]);
        }

        [Fact]
        public void Board_InitializesRobberOnDesertTile()
        {
            Tile forest = Tile(TileType.Forest, 6);
            Tile desert = Tile(TileType.Desert, 0);
            BoardModel board = Board(forest, desert);

            Assert.Same(desert, board.RobberTile);
            Assert.True(board.IsTileBlockedByRobber(desert));
        }

        [Fact]
        public void MoveRobberTo_AllowsMovingToDifferentBoardTileOnly()
        {
            Tile desert = Tile(TileType.Desert, 0);
            Tile forest = Tile(TileType.Forest, 6);
            Tile outsideBoard = Tile(TileType.Brick, 8);
            BoardModel board = Board(desert, forest);

            Assert.False(board.MoveRobberTo(desert));
            Assert.Same(desert, board.RobberTile);
            Assert.False(board.MoveRobberTo(outsideBoard));
            Assert.Same(desert, board.RobberTile);
            Assert.True(board.MoveRobberTo(forest));
            Assert.Same(forest, board.RobberTile);
        }

        [Fact]
        public void GetProducingTiles_ReturnsMatchingUnblockedResourceTilesOnly()
        {
            Tile desert = Tile(TileType.Desert, 0);
            Tile matchingForest = Tile(TileType.Forest, 6);
            Tile blockedBrick = Tile(TileType.Brick, 6);
            Tile differentDiceMountain = Tile(TileType.Mountain, 8);
            Tile matchingDesert = Tile(TileType.Desert, 6);
            BoardModel board = Board(desert, matchingForest, blockedBrick, differentDiceMountain, matchingDesert);
            board.MoveRobberTo(blockedBrick);

            List<Tile> producingTiles = board.GetProducingTiles(6).ToList();

            Assert.Equal([matchingForest], producingTiles);
        }

        [Theory]
        [InlineData(TileType.Forest, ResourceId.Wood)]
        [InlineData(TileType.Sheep, ResourceId.Wool)]
        [InlineData(TileType.Brick, ResourceId.Brick)]
        [InlineData(TileType.Mountain, ResourceId.Ore)]
        [InlineData(TileType.Farm, ResourceId.Wheat)]
        public void ProducedResource_ReturnsResourceForProducingTileTypes(TileType tileType, ResourceId expectedResource)
        {
            Tile tile = Tile(tileType, 6);

            Assert.Equal(expectedResource, tile.ProducedResource);
        }

        [Fact]
        public void ProducedResource_ReturnsNullForDesert()
        {
            Tile tile = Tile(TileType.Desert, 0);

            Assert.Null(tile.ProducedResource);
        }

        [Fact]
        public void PlaceBuildingAndRoad_StoreOwners()
        {
            Player player = new(1);
            TileVertex first = Vertex();
            TileVertex second = Vertex();
            TileEdge edge = Edge(first, second);
            var building = new Building(player, BuildingType.City);

            first.PlaceBuilding(building);
            edge.PlaceRoad(player);

            Assert.True(first.HasBuilding);
            Assert.Same(player, first.Building.Owner);
            Assert.Equal(BuildingType.City, first.Building.Type);
            Assert.Equal(2, first.Building.ProductionAmount);
            Assert.Same(player, edge.RoadOwner);
        }

        private static BoardModel Board(params Tile[] tiles)
        {
            return new BoardModel(0, 0, null!, tiles.ToList(), [], new BoardGraph());
        }

        private static Tile Tile(TileType type, int diceNumber, params TileVertex[] vertices)
        {
            return new Tile(0, 0, null!, type, diceNumber, vertices, null!);
        }

        private static TileVertex Vertex()
        {
            return new TileVertex(0, 0, null!, null!);
        }

        private static TileEdge Edge(TileVertex first, TileVertex second)
        {
            return new TileEdge(first, second, null!, null!);
        }
    }
}
