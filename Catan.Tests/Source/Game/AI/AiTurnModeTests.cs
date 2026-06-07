using Catan.Source.Game.AI;
using Xunit;

namespace Catan.Tests.Source.Game.AI
{
    public class AiTurnModeTests
    {
        [Fact]
        public void ShouldAutoEndTurn_ReturnsTrueForAutoMode()
        {
            Assert.True(AiTurnMode.Auto.ShouldAutoEndTurn());
        }

        [Fact]
        public void ShouldAutoEndTurn_ReturnsFalseForManualMode()
        {
            Assert.False(AiTurnMode.Manual.ShouldAutoEndTurn());
        }

        [Fact]
        public void CanUseManualActionButtons_ReturnsFalseForAiPlayerActionsState()
        {
            Assert.False(AiTurnInputPolicy.CanUseManualActionButtons(isAiPlayerActionsState: true));
        }

        [Fact]
        public void CanUseManualActionButtons_ReturnsTrueOutsideAiPlayerActionsState()
        {
            Assert.True(AiTurnInputPolicy.CanUseManualActionButtons(isAiPlayerActionsState: false));
        }
    }
}
