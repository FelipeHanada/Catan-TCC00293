using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.Inventory
{
    public class ResourceInventoryTests
    {
        [Fact]
        public void NewInventory_StartsWithZeroForEveryResource()
        {
            ResourceInventory inventory = new();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                Assert.Equal(0, inventory.GetAmount(resource));
            }

            Assert.Equal(0, inventory.TotalCards());
        }

        [Fact]
        public void AddAndRemove_UpdatesAmountsAndTotal()
        {
            ResourceInventory inventory = new();

            inventory.Add(ResourceId.Wood, 3);
            inventory.Add(ResourceId.Wheat, 2);
            inventory.Remove(ResourceId.Wood, 1);

            Assert.Equal(2, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(2, inventory.GetAmount(ResourceId.Wheat));
            Assert.Equal(4, inventory.TotalCards());
        }

        [Fact]
        public void Remove_WhenInsufficient_ThrowsAndKeepsAmount()
        {
            ResourceInventory inventory = new();
            inventory.Add(ResourceId.Brick, 1);

            Assert.Throws<InvalidOperationException>(() => inventory.Remove(ResourceId.Brick, 2));

            Assert.Equal(1, inventory.GetAmount(ResourceId.Brick));
            Assert.Equal(1, inventory.TotalCards());
        }

        [Fact]
        public void TryRemove_WhenInsufficient_ReturnsFalseAndKeepsAmount()
        {
            ResourceInventory inventory = new();
            inventory.Add(ResourceId.Ore, 1);

            bool removed = inventory.TryRemove(ResourceId.Ore, 2);

            Assert.False(removed);
            Assert.Equal(1, inventory.GetAmount(ResourceId.Ore));
            Assert.Equal(1, inventory.TotalCards());
        }

        [Fact]
        public void AddDictionary_AddsAllResourcesAtomically()
        {
            ResourceInventory inventory = new();
            Dictionary<ResourceId, int> resources = new()
            {
                [ResourceId.Wood] = 2,
                [ResourceId.Wool] = 1,
                [ResourceId.Wheat] = 3
            };

            inventory.Add(resources);

            Assert.Equal(2, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, inventory.GetAmount(ResourceId.Wool));
            Assert.Equal(3, inventory.GetAmount(ResourceId.Wheat));
            Assert.Equal(6, inventory.TotalCards());
        }

        [Fact]
        public void RemoveDictionary_WhenAnyResourceIsInsufficient_ThrowsWithoutPartialRemoval()
        {
            ResourceInventory inventory = new();
            inventory.Add(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = 2,
                [ResourceId.Wool] = 1
            });

            Dictionary<ResourceId, int> resources = new()
            {
                [ResourceId.Wood] = 1,
                [ResourceId.Wool] = 2
            };

            Assert.Throws<InvalidOperationException>(() => inventory.Remove(resources));

            Assert.Equal(2, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, inventory.GetAmount(ResourceId.Wool));
            Assert.Equal(3, inventory.TotalCards());
        }

        [Fact]
        public void TryRemoveDictionary_WhenEnoughResources_RemovesAllAndReturnsTrue()
        {
            ResourceInventory inventory = new();
            inventory.Add(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = 2,
                [ResourceId.Wheat] = 1
            });

            bool removed = inventory.TryRemove(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = 1,
                [ResourceId.Wheat] = 1
            });

            Assert.True(removed);
            Assert.Equal(1, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(0, inventory.GetAmount(ResourceId.Wheat));
            Assert.Equal(1, inventory.TotalCards());
        }

        [Fact]
        public void NegativeAmounts_AreRejected()
        {
            ResourceInventory inventory = new();

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Add(ResourceId.Wood, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Remove(ResourceId.Wood, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.TryRemove(ResourceId.Wood, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Add(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = -1
            }));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Remove(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = -1
            }));
            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.TryRemove(new Dictionary<ResourceId, int>
            {
                [ResourceId.Wood] = -1
            }));

            Assert.Equal(0, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(0, inventory.TotalCards());
        }
    }
}
