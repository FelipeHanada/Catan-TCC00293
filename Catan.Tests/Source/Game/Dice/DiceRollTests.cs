using Catan.Source.Game.Dice;
using Xunit;

namespace Catan.Tests.Source.Game.Dice
{
    public class DiceRollTests
    {
        [Fact]
        public void Constructor_StoresValuesAndTotal()
        {
            var roll = new DiceRoll(2, 5);

            Assert.Equal(2, roll.First);
            Assert.Equal(5, roll.Second);
            Assert.Equal(7, roll.Total);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(7, 1)]
        [InlineData(1, 0)]
        [InlineData(1, 7)]
        public void Constructor_WithInvalidDieValues_ThrowsArgumentOutOfRangeException(int first, int second)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll(first, second));
        }
    }
}
