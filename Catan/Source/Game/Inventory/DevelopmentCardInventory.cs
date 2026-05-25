using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Inventory
{
    public class DevelopmentCardInventory
    {
        private readonly List<DevelopmentCard> _cards;

        public IReadOnlyList<DevelopmentCard> Cards => _cards;
        public int Count => _cards.Count;

        public DevelopmentCardInventory()
        {
            _cards = new List<DevelopmentCard>();
        }

        public void Add(DevelopmentCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            _cards.Add(card);
        }

        public bool Remove(DevelopmentCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            return _cards.Remove(card);
        }

        public int CountByType(DevelopmentCardType type)
        {
            int total = 0;

            foreach (DevelopmentCard card in _cards)
            {
                if (card.Type == type)
                {
                    total++;
                }
            }

            return total;
        }
    }
}
