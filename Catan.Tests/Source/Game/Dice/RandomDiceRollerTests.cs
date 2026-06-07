using Catan.Source.Game.Dice;
using Xunit;

namespace Catan.Tests.Source.Game.Dice
{
    public class RandomDiceRollerTests
    {
        [Fact]
        public void Roll_ReturnsValuesInDiceRanges()
        {
            var roller = new RandomDiceRoller();

            for (int i = 0; i < 100; i++)
            {
                DiceRoll roll = roller.Roll();

                Assert.InRange(roll.First, 1, 6);
                Assert.InRange(roll.Second, 1, 6);
                Assert.InRange(roll.Total, 2, 12);
            }
        }
    }
}
