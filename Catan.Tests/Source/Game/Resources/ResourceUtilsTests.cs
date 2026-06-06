using System;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.Resources
{
    public class ResourceUtilsTests
    {
        [Fact]
        public void ResourceIds_ContainsResourcesInEnumOrder()
        {
            ResourceId[] expected = Enum.GetValues<ResourceId>();

            Assert.Equal(expected, ResourceUtils.ResourceIds);
        }

        [Fact]
        public void ResourceName_MapsEveryResourceToSpriteName()
        {
            Assert.Equal("wood", ResourceUtils.ResourceName[ResourceId.Wood]);
            Assert.Equal("wool", ResourceUtils.ResourceName[ResourceId.Wool]);
            Assert.Equal("brick", ResourceUtils.ResourceName[ResourceId.Brick]);
            Assert.Equal("ore", ResourceUtils.ResourceName[ResourceId.Ore]);
            Assert.Equal("wheat", ResourceUtils.ResourceName[ResourceId.Wheat]);
            Assert.Equal(Enum.GetValues<ResourceId>().Length, ResourceUtils.ResourceName.Count);
        }
    }
}
