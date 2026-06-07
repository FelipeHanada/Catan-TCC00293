using System;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.Bank
{
    public class ResourceDistributionRequestTests
    {
        [Fact]
        public void Constructor_StoresRecipientResourceAndAmount()
        {
            ResourceInventory inventory = new();

            var request = new Catan.Source.Game.Bank.ResourceDistributionRequest(inventory, ResourceId.Wood, 2);

            Assert.Same(inventory, request.RecipientInventory);
            Assert.Equal(ResourceId.Wood, request.Resource);
            Assert.Equal(2, request.Amount);
        }

        [Fact]
        public void Constructor_WithNullInventory_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Catan.Source.Game.Bank.ResourceDistributionRequest(null!, ResourceId.Wood, 1));
        }

        [Fact]
        public void Constructor_WithNegativeAmount_Throws()
        {
            ResourceInventory inventory = new();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Catan.Source.Game.Bank.ResourceDistributionRequest(inventory, ResourceId.Wood, -1));
        }
    }
}
