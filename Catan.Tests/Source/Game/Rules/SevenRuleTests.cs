using System.Collections.Generic;
using Catan.Source.Game.Bank;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Xunit;

namespace Catan.Tests.Source.Game.Rules
{
    public class SevenRuleTests
    {
        [Fact]
        public void DiscardResourcesToBank_WhenPlayerMustDiscard_ReturnsDiscardedResourcesToBank()
        {
            Bank bank = new();
            Player player = new(0);
            bank.Give(player.Inventory.Resources, ResourceId.Wood, 8);
            SevenRule rule = new();

            Dictionary<ResourceId, int> discardedResources = rule.DiscardResourcesToBank(player, bank);

            Assert.Equal(4, discardedResources[ResourceId.Wood]);
            Assert.Equal(4, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(Bank.DefaultCardsPerResource - 4, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void DiscardResourcesToBank_WhenPlayerDoesNotNeedDiscard_ReturnsEmptyDiscard()
        {
            Bank bank = new();
            Player player = new(0);
            bank.Give(player.Inventory.Resources, ResourceId.Wood, 7);
            SevenRule rule = new();

            Dictionary<ResourceId, int> discardedResources = rule.DiscardResourcesToBank(player, bank);

            Assert.Empty(discardedResources);
            Assert.Equal(7, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(Bank.DefaultCardsPerResource - 7, bank.GetAmount(ResourceId.Wood));
        }
    }
}
