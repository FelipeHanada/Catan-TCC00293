using Catan.Source.Game.Logging;
using Xunit;

namespace Catan.Tests.Source.Game.Logging
{
    public class GameLogTests
    {
        [Fact]
        public void Add_StoresMessagesInOrder()
        {
            GameLog log = new();

            log.Add("Turno do Jogador 1");
            log.Add("Jogador 1 rolou 8");

            Assert.Equal(
                new[] { "Turno do Jogador 1", "Jogador 1 rolou 8" },
                log.Messages);
        }

        [Fact]
        public void Add_WhenLimitIsExceeded_RemovesOldestMessages()
        {
            GameLog log = new(maxMessages: 3);

            log.Add("Mensagem 1");
            log.Add("Mensagem 2");
            log.Add("Mensagem 3");
            log.Add("Mensagem 4");

            Assert.Equal(
                new[] { "Mensagem 2", "Mensagem 3", "Mensagem 4" },
                log.Messages);
        }

        [Fact]
        public void Messages_ReturnsDefensiveSnapshot()
        {
            GameLog log = new();
            log.Add("Mensagem original");

            var firstRead = log.Messages;
            log.Add("Mensagem nova");

            Assert.Equal(new[] { "Mensagem original" }, firstRead);
            Assert.Equal(
                new[] { "Mensagem original", "Mensagem nova" },
                log.Messages);
        }

        [Fact]
        public void Add_IgnoresBlankMessages()
        {
            GameLog log = new();

            log.Add("");
            log.Add("   ");
            log.Add(null);

            Assert.Empty(log.Messages);
        }
    }
}
