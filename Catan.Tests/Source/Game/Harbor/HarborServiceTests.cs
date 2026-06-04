using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;
using BoardModel = Catan.Source.Game.Board.Board;

namespace Catan.Tests.Source.Game.Harbor
{
    public class HarborServiceTests
    {
        [Fact]
        public void GetBestTradeRate_WithoutHarbor_ReturnsDefaultFourToOne()
        {
            Player player = new(0);
            HarborService service = new(CreateBoard(new List<Catan.Source.Game.Harbor.Harbor>()));

            int rate = service.GetBestTradeRate(player, ResourceId.Wood);

            Assert.Equal(4, rate);
        }

        [Fact]
        public void GetBestTradeRate_WithGenericHarbor_ReturnsThreeToOne()
        {
            Player player = new(0);
            TileVertex vertex = CreateVertexOwnedBy(player);
            HarborService service = new(CreateBoard(new List<Catan.Source.Game.Harbor.Harbor>
            {
                Catan.Source.Game.Harbor.Harbor.CreateGeneric(new[] { vertex })
            }));

            int rate = service.GetBestTradeRate(player, ResourceId.Wood);

            Assert.Equal(3, rate);
        }

        [Fact]
        public void GetBestTradeRate_WithSpecificHarborForPaidResource_ReturnsTwoToOne()
        {
            Player player = new(0);
            TileVertex vertex = CreateVertexOwnedBy(player);
            HarborService service = new(CreateBoard(new List<Catan.Source.Game.Harbor.Harbor>
            {
                Catan.Source.Game.Harbor.Harbor.CreateSpecific(ResourceId.Wood, new[] { vertex })
            }));

            int rate = service.GetBestTradeRate(player, ResourceId.Wood);

            Assert.Equal(2, rate);
        }

        private static BoardModel CreateBoard(List<Catan.Source.Game.Harbor.Harbor> harbors)
        {
            return new BoardModel(0, 0, null, new List<Tile>(), harbors, new BoardGraph());
        }

        private static TileVertex CreateVertexOwnedBy(Player player)
        {
            TileVertex vertex = new(0, 0, null, null);
            vertex.PlaceBuilding(new Building(player, BuildingType.Settlement));
            return vertex;
        }
    }
}
