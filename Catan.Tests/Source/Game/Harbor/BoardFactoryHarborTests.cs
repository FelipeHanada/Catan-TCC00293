using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.Harbor
{
    public class BoardFactoryHarborTests
    {
        [Fact]
        public void CreateHarbors_UsesDrawnHarborVertexPairs()
        {
            StandardTilePositionIterator iterator = new(192, 64, null, null);

            List<Catan.Source.Game.Harbor.Harbor> harbors = iterator.CreateHarbors();

            Assert.Collection(harbors,
                harbor => AssertHarbor(harbor, HarborType.Generic, null, 448, 96, 512, 64),
                harbor => AssertHarbor(harbor, HarborType.Specific, ResourceId.Ore, 256, 192, 320, 160),
                harbor => AssertHarbor(harbor, HarborType.Specific, ResourceId.Wood, 640, 64, 704, 96),
                harbor => AssertHarbor(harbor, HarborType.Generic, null, 768, 192, 768, 256),
                harbor => AssertHarbor(harbor, HarborType.Specific, ResourceId.Brick, 768, 384, 768, 448),
                harbor => AssertHarbor(harbor, HarborType.Generic, null, 704, 544, 640, 576),
                harbor => AssertHarbor(harbor, HarborType.Specific, ResourceId.Wheat, 512, 576, 448, 544),
                harbor => AssertHarbor(harbor, HarborType.Specific, ResourceId.Wool, 320, 480, 256, 448),
                harbor => AssertHarbor(harbor, HarborType.Generic, null, 192, 288, 192, 352));
        }

        private static void AssertHarbor(
            Catan.Source.Game.Harbor.Harbor harbor,
            HarborType type,
            ResourceId? resource,
            float firstX,
            float firstY,
            float secondX,
            float secondY)
        {
            Assert.Equal(type, harbor.Type);
            Assert.Equal(resource, harbor.Resource);
            Assert.Equal(2, harbor.Vertices.Count);
            Assert.Equal(firstX, harbor.Vertices[0].X);
            Assert.Equal(firstY, harbor.Vertices[0].Y);
            Assert.Equal(secondX, harbor.Vertices[1].X);
            Assert.Equal(secondY, harbor.Vertices[1].Y);
        }
    }
}
