using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Xunit;

namespace Catan.Tests.Source.Game.Rules
{
    public class RobberRuleTests
    {
        [Fact]
        public void GetRobberyTargets_ReturnsUniqueAdjacentOpponentsWithResources()
        {
            Player currentPlayer = new(1);
            Player opponentWithResources = PlayerWith(2, (ResourceId.Wood, 1));
            Player otherOpponentWithResources = PlayerWith(3, (ResourceId.Brick, 1));
            Player opponentWithoutResources = new(4);
            Tile tile = TileWithBuildings(
                new Building(currentPlayer, BuildingType.Settlement),
                new Building(opponentWithResources, BuildingType.Settlement),
                new Building(opponentWithResources, BuildingType.City),
                new Building(opponentWithoutResources, BuildingType.Settlement),
                new Building(otherOpponentWithResources, BuildingType.Settlement));
            var rule = new RobberRule();

            List<Player> targets = rule.GetRobberyTargets(currentPlayer, tile);

            Assert.Equal([opponentWithResources, otherOpponentWithResources], targets);
        }

        [Fact]
        public void TryStealRandomResource_WhenTargetHasSingleResource_TransfersIt()
        {
            Player currentPlayer = new(1);
            Player target = PlayerWith(2, (ResourceId.Ore, 1));
            var rule = new RobberRule();

            bool didSteal = rule.TryStealRandomResource(currentPlayer, target, out ResourceId stolenResource);

            Assert.True(didSteal);
            Assert.Equal(ResourceId.Ore, stolenResource);
            Assert.Equal(1, currentPlayer.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(0, target.Inventory.Resources.GetAmount(ResourceId.Ore));
        }

        [Fact]
        public void TryStealRandomResource_WhenTargetHasNoResources_ReturnsFalse()
        {
            Player currentPlayer = new(1);
            Player target = new(2);
            var rule = new RobberRule();

            bool didSteal = rule.TryStealRandomResource(currentPlayer, target, out _);

            Assert.False(didSteal);
            Assert.False(currentPlayer.Inventory.HasResources());
        }

        private static Tile TileWithBuildings(params Building[] buildings)
        {
            TileVertex[] vertices = buildings
                .Select(building =>
                {
                    var vertex = new TileVertex(0, 0, null!, null!);
                    vertex.PlaceBuilding(building);
                    return vertex;
                })
                .ToArray();

            return new Tile(0, 0, null!, TileType.Desert, 7, vertices, null!);
        }

        private static Player PlayerWith(int number, params (ResourceId Resource, int Amount)[] entries)
        {
            var player = new Player(number);
            foreach (var entry in entries)
            {
                player.Inventory.AddResource(entry.Resource, entry.Amount);
            }

            return player;
        }
    }
}
