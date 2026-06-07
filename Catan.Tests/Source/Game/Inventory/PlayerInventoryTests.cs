using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.Inventory
{
    public class PlayerInventoryTests
    {
        [Fact]
        public void NewInventory_StartsEmpty()
        {
            PlayerInventory inventory = new();

            Assert.False(inventory.HasResources());
            Assert.Equal(0, inventory.TotalResourceCards);
            Assert.Equal(0, inventory.TotalDevelopmentCards);
            Assert.Equal(0, inventory.PlayedKnightsCount);
            Assert.Equal(0, inventory.GetTotalResources());
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Wheat));
        }

        [Fact]
        public void AddResource_UpdatesResourceTotals()
        {
            PlayerInventory inventory = new();

            inventory.AddResource(ResourceId.Wood, 2);
            inventory.AddResource(ResourceId.Wheat, 3);

            Assert.True(inventory.HasResources());
            Assert.Equal(2, inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(3, inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(5, inventory.TotalResourceCards);
            Assert.Equal(5, inventory.GetTotalResources());
            Assert.Equal(0, inventory.TotalDevelopmentCards);
        }

        [Fact]
        public void DiscardResources_RemovesInConfiguredOrder()
        {
            PlayerInventory inventory = new();
            inventory.AddResource(ResourceId.Wood, 2);
            inventory.AddResource(ResourceId.Wool, 1);
            inventory.AddResource(ResourceId.Brick, 1);
            inventory.AddResource(ResourceId.Ore, 1);
            inventory.AddResource(ResourceId.Wheat, 1);

            Dictionary<ResourceId, int> discarded = inventory.DiscardResources(4);

            Assert.Equal(3, discarded.Count);
            Assert.Equal(2, discarded[ResourceId.Wood]);
            Assert.Equal(1, discarded[ResourceId.Wool]);
            Assert.Equal(1, discarded[ResourceId.Brick]);
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(1, inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(1, inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(2, inventory.TotalResourceCards);
        }

        [Fact]
        public void TryRemoveRandomResource_WithOneResource_RemovesThatResource()
        {
            PlayerInventory inventory = new();
            inventory.AddResource(ResourceId.Ore, 1);

            bool removed = inventory.TryRemoveRandomResource(out ResourceId resource);

            Assert.True(removed);
            Assert.Equal(ResourceId.Ore, resource);
            Assert.Equal(0, inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(0, inventory.TotalResourceCards);
            Assert.False(inventory.HasResources());
        }

        [Fact]
        public void TryRemoveRandomResource_WhenEmpty_ReturnsFalse()
        {
            PlayerInventory inventory = new();

            bool removed = inventory.TryRemoveRandomResource(out _);

            Assert.False(removed);
            Assert.Equal(0, inventory.TotalResourceCards);
        }

        [Fact]
        public void IncrementPlayedKnights_IncrementsCounter()
        {
            PlayerInventory inventory = new();

            inventory.IncrementPlayedKnights();
            inventory.IncrementPlayedKnights();

            Assert.Equal(2, inventory.PlayedKnightsCount);
        }
    }
}
