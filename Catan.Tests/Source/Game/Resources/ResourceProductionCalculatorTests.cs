using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;
using BoardModel = Catan.Source.Game.Board.Board;

namespace Catan.Tests.Source.Game.Resources
{
    public class ResourceProductionCalculatorTests
    {
        [Fact]
        public void CalculateExpectedProductions_ReturnsOneEntryPerAdjacentBuilding()
        {
            Player firstPlayer = new(1);
            Player secondPlayer = new(2);
            Tile tile = Tile(
                TileType.Forest,
                6,
                VertexWithBuilding(firstPlayer, BuildingType.Settlement),
                VertexWithBuilding(secondPlayer, BuildingType.City));
            BoardModel board = Board(Tile(TileType.Desert, 0), tile);
            var calculator = new ResourceProductionCalculator(board);

            List<ResourceProductionEntry> productions = calculator.CalculateExpectedProductions(6);

            Assert.Collection(productions,
                production => AssertProduction(production, firstPlayer, ResourceId.Wood, 1),
                production => AssertProduction(production, secondPlayer, ResourceId.Wood, 2));
        }

        [Fact]
        public void CalculateExpectedProductions_IgnoresBlockedTiles()
        {
            Player player = new(1);
            Tile desert = Tile(TileType.Desert, 0);
            Tile blockedTile = Tile(TileType.Brick, 8, VertexWithBuilding(player, BuildingType.Settlement));
            BoardModel board = Board(desert, blockedTile);
            board.MoveRobberTo(blockedTile);
            var calculator = new ResourceProductionCalculator(board);

            List<ResourceProductionEntry> productions = calculator.CalculateExpectedProductions(8);

            Assert.Empty(productions);
        }

        [Fact]
        public void CalculateExpectedProductions_WhenDiceDoesNotMatch_ReturnsEmptyList()
        {
            Player player = new(1);
            Tile tile = Tile(TileType.Mountain, 9, VertexWithBuilding(player, BuildingType.Settlement));
            BoardModel board = Board(Tile(TileType.Desert, 0), tile);
            var calculator = new ResourceProductionCalculator(board);

            List<ResourceProductionEntry> productions = calculator.CalculateExpectedProductions(6);

            Assert.Empty(productions);
        }

        private static void AssertProduction(
            ResourceProductionEntry production,
            Player expectedPlayer,
            ResourceId expectedResource,
            int expectedAmount)
        {
            Assert.Same(expectedPlayer, production.Player);
            Assert.Equal(expectedResource, production.Resource);
            Assert.Equal(expectedAmount, production.Amount);
        }

        private static BoardModel Board(params Tile[] tiles)
        {
            return new BoardModel(0, 0, null!, tiles.ToList(), [], new BoardGraph());
        }

        private static Tile Tile(TileType type, int diceNumber, params TileVertex[] vertices)
        {
            return new Tile(0, 0, null!, type, diceNumber, vertices, null!);
        }

        private static TileVertex VertexWithBuilding(Player player, BuildingType buildingType)
        {
            var vertex = new TileVertex(0, 0, null!, null!);
            vertex.PlaceBuilding(new Building(player, buildingType));
            return vertex;
        }
    }
}
