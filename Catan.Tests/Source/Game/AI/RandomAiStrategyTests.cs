using Catan.Source.Game.AI;
using Xunit;

namespace Catan.Tests.Source.Game.AI
{
    public class RandomAiStrategyTests
    {
        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 3)]
        [InlineData(5, 4)]
        [InlineData(6, 5)]
        [InlineData(7, 0)]
        [InlineData(8, 5)]
        [InlineData(9, 4)]
        [InlineData(10, 3)]
        [InlineData(11, 2)]
        [InlineData(12, 1)]
        public void GetDiceWeight_ReturnsExpectedWeight(int diceNumber, int expected)
        {
            Assert.Equal(expected, RandomAiStrategy.GetDiceWeight(diceNumber));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void GetDiceWeight_WithInvalidDiceNumber_ReturnsZero(int diceNumber)
        {
            Assert.Equal(0, RandomAiStrategy.GetDiceWeight(diceNumber));
        }
    }
}
