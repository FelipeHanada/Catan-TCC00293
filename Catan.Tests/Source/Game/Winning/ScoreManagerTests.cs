using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Xunit;

namespace Catan.Tests.Source.Game.Winning
{
    public class ScoreManagerTests
    {
        [Fact]
        public void HasReachedTargetScore_WithScoreBelowConfiguredTarget_ReturnsFalse()
        {
            Assert.False(ScoreManager.HasReachedTargetScore(3, 10));
        }

        [Fact]
        public void HasReachedTargetScore_WithScoreAtConfiguredTarget_ReturnsTrue()
        {
            Assert.True(ScoreManager.HasReachedTargetScore(10, 10));
        }

        [Fact]
        public void GameScene_DefaultTargetScore_IsTen()
        {
            GameScene gameScene = new();

            Assert.Equal(10, gameScene.TargetScore);
        }
    }
}
