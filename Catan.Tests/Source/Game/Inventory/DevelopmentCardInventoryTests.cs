using Catan.Source.Game.Inventory;
using Xunit;

namespace Catan.Tests.Source.Game.Inventory
{
    public class DevelopmentCardInventoryTests
    {
        [Fact]
        public void AddNew_DoesNotMakeCardPlayableImmediately()
        {
            DevelopmentCardInventory inventory = new();

            inventory.AddNew(new DevelopmentCard(DevelopmentCardType.Knight));

            Assert.Equal(1, inventory.Count);
            Assert.Equal(1, inventory.CountByType(DevelopmentCardType.Knight));
            Assert.Equal(1, inventory.CountNewByType(DevelopmentCardType.Knight));
            Assert.Equal(0, inventory.CountPlayableByType(DevelopmentCardType.Knight));
        }

        [Fact]
        public void ReleaseNewCards_MovesNewCardsToPlayableCards()
        {
            DevelopmentCardInventory inventory = new();
            inventory.AddNew(new DevelopmentCard(DevelopmentCardType.Knight));
            inventory.AddNew(new DevelopmentCard(DevelopmentCardType.Knight));

            inventory.ReleaseNewCards();

            Assert.Equal(2, inventory.Count);
            Assert.Equal(0, inventory.CountNewByType(DevelopmentCardType.Knight));
            Assert.Equal(2, inventory.CountPlayableByType(DevelopmentCardType.Knight));
        }

        [Fact]
        public void Add_AddsPlayableCardForCompatibility()
        {
            DevelopmentCardInventory inventory = new();

            inventory.Add(new DevelopmentCard(DevelopmentCardType.Monopoly));

            Assert.Equal(1, inventory.CountByType(DevelopmentCardType.Monopoly));
            Assert.Equal(1, inventory.CountPlayableByType(DevelopmentCardType.Monopoly));
            Assert.Equal(0, inventory.CountNewByType(DevelopmentCardType.Monopoly));
        }
    }
}
