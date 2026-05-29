using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Inventory
{
    public class DevelopmentCardInventory
    {
        private readonly Dictionary<DevelopmentCard, int> _cards;

        public IReadOnlyDictionary<DevelopmentCard, int> Cards => _cards;
        public int Count => TotalCards();

        public DevelopmentCardInventory()
        {
            _cards = new Dictionary<DevelopmentCard, int>();
        }

        public void Add(DevelopmentCard card, int amount = 1)
        {
            ValidateCard(card, nameof(card));
            ValidateAmount(amount, nameof(amount));

            if (!_cards.ContainsKey(card))
            {
                _cards[card] = 0;
            }

            checked
            {
                _cards[card] += amount;
            }
        }

        public bool Remove(DevelopmentCard card, int amount = 1)
        {
            ValidateCard(card, nameof(card));
            ValidateAmount(amount, nameof(amount));

            if (!_cards.TryGetValue(card, out int currentAmount) || currentAmount < amount)
            {
                return false;
            }

            int newAmount = currentAmount - amount;
            if (newAmount == 0)
            {
                _cards.Remove(card);
            }
            else
            {
                _cards[card] = newAmount;
            }

            return true;
        }

        public int CountByCard(DevelopmentCard card)
        {
            ValidateCard(card, nameof(card));
            return _cards.TryGetValue(card, out int amount) ? amount : 0;
        }

        public int CountByType(DevelopmentCardType type)
        {
            int total = 0;

            foreach (var entry in _cards)
            {
                if (entry.Key.Type == type)
                {
                    checked
                    {
                        total += entry.Value;
                    }
                }
            }

            return total;
        }

        private int TotalCards()
        {
            int total = 0;

            foreach (int amount in _cards.Values)
            {
                checked
                {
                    total += amount;
                }
            }

            return total;
        }

        private static void ValidateCard(DevelopmentCard card, string paramName)
        {
            if (card == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateAmount(int amount, string paramName)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, amount, "Quantidade não pode ser negativa.");
            }
        }
    }
}
