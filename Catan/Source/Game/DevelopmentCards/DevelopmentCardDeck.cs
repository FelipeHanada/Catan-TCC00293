using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;

namespace Catan.Source.Game.DevelopmentCards
{
    public class DevelopmentCardDeck
    {
        private readonly List<DevelopmentCard> _cards;

        public bool HasCards => _cards.Count > 0;
        public int Count => _cards.Count;

        public DevelopmentCardDeck()
            : this(CreateOfficialCards(), true)
        {
        }

        public DevelopmentCardDeck(IEnumerable<DevelopmentCard> cards)
            : this(cards, false)
        {
        }

        private DevelopmentCardDeck(IEnumerable<DevelopmentCard> cards, bool shuffle)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            _cards = new List<DevelopmentCard>();
            foreach (DevelopmentCard card in cards)
            {
                if (card == null)
                {
                    throw new ArgumentException("Baralho nao pode conter cartas nulas.", nameof(cards));
                }

                _cards.Add(card);
            }

            if (shuffle)
            {
                Shuffle();
            }
        }

        public DevelopmentCard Draw()
        {
            if (!HasCards)
            {
                throw new InvalidOperationException("Baralho de desenvolvimento vazio.");
            }

            int lastIndex = _cards.Count - 1;
            DevelopmentCard card = _cards[lastIndex];
            _cards.RemoveAt(lastIndex);
            return card;
        }

        private void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        private static IEnumerable<DevelopmentCard> CreateOfficialCards()
        {
            for (int i = 0; i < 14; i++) yield return new DevelopmentCard(DevelopmentCardType.Knight);
            for (int i = 0; i < 5; i++) yield return new DevelopmentCard(DevelopmentCardType.VictoryPoint);
            for (int i = 0; i < 2; i++) yield return new DevelopmentCard(DevelopmentCardType.RoadBuilding);
            for (int i = 0; i < 2; i++) yield return new DevelopmentCard(DevelopmentCardType.YearOfPlenty);
            for (int i = 0; i < 2; i++) yield return new DevelopmentCard(DevelopmentCardType.Monopoly);
        }
    }
}
