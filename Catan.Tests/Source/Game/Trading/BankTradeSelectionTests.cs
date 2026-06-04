using System.Collections.Generic;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Xunit;

namespace Catan.Tests.Source.Game.Trading
{
    public class BankTradeSelectionTests
    {
        [Fact]
        public void TryCreate_WithValidSelection_Succeeds()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 4 },
                new Dictionary<ResourceId, int> { [ResourceId.Ore] = 1 },
                expectedPaidAmount: 4,
                out BankTradeSelection selection,
                out string message);

            Assert.True(success, message);
            Assert.Equal(ResourceId.Wood, selection.PaidResource);
            Assert.Equal(ResourceId.Ore, selection.ReceivedResource);
        }

        [Fact]
        public void TryCreate_WithMultiplePaidResources_Fails()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 2, [ResourceId.Brick] = 2 },
                new Dictionary<ResourceId, int> { [ResourceId.Ore] = 1 },
                expectedPaidAmount: 4,
                out _,
                out _);

            Assert.False(success);
        }

        [Fact]
        public void TryCreate_WithMultipleReceivedResources_Fails()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 4 },
                new Dictionary<ResourceId, int> { [ResourceId.Ore] = 1, [ResourceId.Wheat] = 1 },
                expectedPaidAmount: 4,
                out _,
                out _);

            Assert.False(success);
        }

        [Fact]
        public void TryCreate_WithSamePaidAndReceivedResource_Fails()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 4 },
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 1 },
                expectedPaidAmount: 4,
                out _,
                out _);

            Assert.False(success);
        }

        [Fact]
        public void TryCreate_WithPaidAmountDifferentFromRate_Fails()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 3 },
                new Dictionary<ResourceId, int> { [ResourceId.Ore] = 1 },
                expectedPaidAmount: 4,
                out _,
                out _);

            Assert.False(success);
        }

        [Fact]
        public void TryCreate_WithReceivedAmountDifferentFromOne_Fails()
        {
            bool success = BankTradeSelection.TryCreate(
                new Dictionary<ResourceId, int> { [ResourceId.Wood] = 4 },
                new Dictionary<ResourceId, int> { [ResourceId.Ore] = 2 },
                expectedPaidAmount: 4,
                out _,
                out _);

            Assert.False(success);
        }
    }
}
