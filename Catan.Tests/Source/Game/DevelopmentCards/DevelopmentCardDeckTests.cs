using System;
using System.Collections.Generic;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Xunit;

namespace Catan.Tests.Source.Game.DevelopmentCards
{
    public class DevelopmentCardDeckTests
    {
        [Fact]
        public void NewDeck_HasOfficialCardCount()
        {
            DevelopmentCardDeck deck = new();

            Assert.True(deck.HasCards);
            Assert.Equal(25, deck.Count);
        }

        [Fact]
        public void NewDeck_HasOfficialComposition()
        {
            DevelopmentCardDeck deck = new();
            Dictionary<DevelopmentCardType, int> counts = DrawAll(deck);

            Assert.Equal(14, counts[DevelopmentCardType.Knight]);
            Assert.Equal(5, counts[DevelopmentCardType.VictoryPoint]);
            Assert.Equal(2, counts[DevelopmentCardType.RoadBuilding]);
            Assert.Equal(2, counts[DevelopmentCardType.YearOfPlenty]);
            Assert.Equal(2, counts[DevelopmentCardType.Monopoly]);
        }

        [Fact]
        public void Draw_RemovesTopCard()
        {
            DevelopmentCardDeck deck = new(new[]
            {
                new DevelopmentCard(DevelopmentCardType.Knight),
            });

            DevelopmentCard card = deck.Draw();

            Assert.Equal(DevelopmentCardType.Knight, card.Type);
            Assert.False(deck.HasCards);
            Assert.Equal(0, deck.Count);
        }

        [Fact]
        public void Draw_WhenDeckIsEmpty_Throws()
        {
            DevelopmentCardDeck deck = new(Array.Empty<DevelopmentCard>());

            Assert.Throws<InvalidOperationException>(() => deck.Draw());
        }

        private static Dictionary<DevelopmentCardType, int> DrawAll(DevelopmentCardDeck deck)
        {
            Dictionary<DevelopmentCardType, int> counts = new()
            {
                [DevelopmentCardType.Knight] = 0,
                [DevelopmentCardType.VictoryPoint] = 0,
                [DevelopmentCardType.RoadBuilding] = 0,
                [DevelopmentCardType.YearOfPlenty] = 0,
                [DevelopmentCardType.Monopoly] = 0,
            };

            while (deck.HasCards)
            {
                DevelopmentCard card = deck.Draw();
                counts[card.Type]++;
            }

            return counts;
        }
    }
}
