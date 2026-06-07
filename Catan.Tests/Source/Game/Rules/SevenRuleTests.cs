using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Xunit;

namespace Catan.Tests.Source.Game.Rules
{
    public class SevenRuleTests
    {
        [Theory]
        [InlineData(7, false)]
        [InlineData(8, true)]
        [InlineData(9, true)]
        public void ShouldDiscard_DependsOnHavingAtLeastEightResources(int resourceCount, bool expected)
        {
            Player player = PlayerWithResources(resourceCount);
            var rule = new SevenRule();

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
            var rule = new SevenRule();

            int discardAmount = rule.GetDiscardAmount(player);

            Assert.Equal(expectedAmount, discardAmount);
        }

        private static Player PlayerWithResources(int resourceCount)
        {
            var player = new Player();
            player.Inventory.AddResource(ResourceId.Wood, resourceCount);
            return player;
        }
    }
}
