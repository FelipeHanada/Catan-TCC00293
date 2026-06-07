using System;
using Catan.Source.Game.AI;
using Xunit;

namespace Catan.Tests.Source.Game.AI
{
    public class WeightedRandomPickerTests
    {
        [Fact]
        public void Pick_WithOnlyPositiveCandidate_ReturnsThatCandidate()
        {
            string result = WeightedRandomPicker.Pick(
                new[]
                {
                    new WeightedCandidate<string>("A", 0),
                    new WeightedCandidate<string>("B", 5),
                    new WeightedCandidate<string>("C", 0),
                },
                new Random(1));

            Assert.Equal("B", result);
        }

        [Fact]
        public void Pick_WithNoPositiveWeights_ReturnsRandomCandidate()
        {
            string result = WeightedRandomPicker.Pick(
                new[]
                {
                    new WeightedCandidate<string>("A", 0),
                    new WeightedCandidate<string>("B", 0),
                },
                new Random(1));

            Assert.Contains(result, new[] { "A", "B" });
        }

        [Fact]
        public void Pick_WithEmptyCandidates_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                WeightedRandomPicker.Pick(Array.Empty<WeightedCandidate<string>>(), new Random(1)));
        }
    }
}
