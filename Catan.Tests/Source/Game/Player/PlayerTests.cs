using Catan.Source.Game.Player;
using Xunit;

namespace Catan.Tests.Source.Game.Players
{
    public class PlayerTests
    {
        [Fact]
        public void DisplayName_ForHumanPlayer_ReturnsConfiguredName()
        {
            Player player = new Player(0, "Rafael", false);

            Assert.Equal("Rafael", player.DisplayName);
        }

        [Fact]
        public void DisplayName_ForAiPlayer_ReturnsAiLabelWithPlayerNumber()
        {
            Player player = new Player(1, "Bot", true);

            Assert.Equal("IA 2", player.DisplayName);
        }
    }
}
