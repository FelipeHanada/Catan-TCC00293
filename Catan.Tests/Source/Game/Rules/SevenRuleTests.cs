using System.Collections.Generic;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Xunit;
using GameBank = Catan.Source.Game.Bank.Bank;

namespace Catan.Tests.Source.Game.Rules
{
    public class SevenRuleTests
    {
        [Fact]
        public void DiscardResourcesToBank_WhenPlayerMustDiscard_ReturnsDiscardedResourcesToBank()
        {
            GameBank bank = new();
            Player player = new(0);
            bank.Give(player.Inventory.Resources, ResourceId.Wood, 8);
            SevenRule rule = new();

            Dictionary<ResourceId, int> discardedResources = rule.DiscardResourcesToBank(player, bank);

            Assert.Equal(4, discardedResources[ResourceId.Wood]);
            Assert.Equal(4, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(GameBank.DefaultCardsPerResource - 4, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void DiscardResourcesToBank_WhenPlayerDoesNotNeedDiscard_ReturnsEmptyDiscard()
        {
            GameBank bank = new();
            Player player = new(0);
            bank.Give(player.Inventory.Resources, ResourceId.Wood, 7);
            SevenRule rule = new();

            Dictionary<ResourceId, int> discardedResources = rule.DiscardResourcesToBank(player, bank);

            Assert.Empty(discardedResources);
            Assert.Equal(7, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(GameBank.DefaultCardsPerResource - 7, bank.GetAmount(ResourceId.Wood));
        }

        [Theory]
        [InlineData(7, false)]
        [InlineData(8, true)]
        [InlineData(9, true)]
        public void ShouldDiscard_DependsOnHavingAtLeastEightResources(int resourceCount, bool expected)
        {
            Player player = PlayerWithResources(resourceCount);
            SevenRule rule = new();

            bool shouldDiscard = rule.ShouldDiscard(player);

            Assert.Equal(expected, shouldDiscard);
        }

        [Theory]
        [InlineData(8, 4)]
        [InlineData(9, 4)]
        [InlineData(10, 5)]
        public void GetDiscardAmount_ReturnsHalfRoundedDown(int resourceCount, int expectedAmount)
        {
            Player player = PlayerWithResources(resourceCount);
            SevenRule rule = new();

            int discardAmount = rule.GetDiscardAmount(player);

            Assert.Equal(expectedAmount, discardAmount);
        }

        private static Player PlayerWithResources(int resourceCount)
        {
            Player player = new();
            player.Inventory.AddResource(ResourceId.Wood, resourceCount);
            return player;
        }
    }
}
