using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Xunit;

namespace Catan.Tests.Source.Game.DevelopmentCards
{
    public class DevelopmentCardActivationRulesTests
    {
        [Fact]
        public void CanActivate_WithNewCardOnly_ReturnsFalse()
        {
            DevelopmentCardInventory inventory = new();
            inventory.AddNew(new DevelopmentCard(DevelopmentCardType.Knight));

            bool canActivate = DevelopmentCardActivationRules.CanActivate(
                DevelopmentCardType.Knight,
                inventory,
                hasUsedDevelopmentCardThisTurn: false);

            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_WithVictoryPoint_ReturnsFalse()
        {
            DevelopmentCardInventory inventory = new();
            inventory.Add(new DevelopmentCard(DevelopmentCardType.VictoryPoint));

            bool canActivate = DevelopmentCardActivationRules.CanActivate(
                DevelopmentCardType.VictoryPoint,
                inventory,
                hasUsedDevelopmentCardThisTurn: false);

            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_WithNoPlayableCards_ReturnsFalse()
        {
            DevelopmentCardInventory inventory = new();

            bool canActivate = DevelopmentCardActivationRules.CanActivate(
                DevelopmentCardType.Monopoly,
                inventory,
                hasUsedDevelopmentCardThisTurn: false);

            Assert.False(canActivate);
        }

        [Fact]
        public void CanActivate_WhenCardAlreadyUsedThisTurn_ReturnsFalse()
        {
            DevelopmentCardInventory inventory = new();
            inventory.Add(new DevelopmentCard(DevelopmentCardType.RoadBuilding));

            bool canActivate = DevelopmentCardActivationRules.CanActivate(
                DevelopmentCardType.RoadBuilding,
                inventory,
                hasUsedDevelopmentCardThisTurn: true);

            Assert.False(canActivate);
        }

        [Theory]
        [InlineData(DevelopmentCardType.Knight)]
        [InlineData(DevelopmentCardType.RoadBuilding)]
        [InlineData(DevelopmentCardType.Invention)]
        [InlineData(DevelopmentCardType.Monopoly)]
        public void CanActivate_WithPlayableActivatableCardBeforeUse_ReturnsTrue(DevelopmentCardType type)
        {
            DevelopmentCardInventory inventory = new();
            inventory.Add(new DevelopmentCard(type));

            bool canActivate = DevelopmentCardActivationRules.CanActivate(
                type,
                inventory,
                hasUsedDevelopmentCardThisTurn: false);

            Assert.True(canActivate);
        }

        [Fact]
        public void ActivatableTypes_ExcludesVictoryPoint()
        {
            Assert.DoesNotContain(DevelopmentCardType.VictoryPoint, DevelopmentCardActivationRules.ActivatableTypes);
        }
    }
}
