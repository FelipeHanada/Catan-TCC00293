using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Inventory
{
    public class DevelopmentCardInventory
    {
        private readonly Dictionary<DevelopmentCard, int> _playableCards;
        private readonly Dictionary<DevelopmentCard, int> _newCards;

        public IReadOnlyDictionary<DevelopmentCard, int> Cards => BuildTotalCards();
        public IReadOnlyDictionary<DevelopmentCard, int> PlayableCards => _playableCards;
        public IReadOnlyDictionary<DevelopmentCard, int> NewCards => _newCards;
        public int Count => TotalCards();

        public DevelopmentCardInventory()
        {
            _playableCards = new Dictionary<DevelopmentCard, int>();
            _newCards = new Dictionary<DevelopmentCard, int>();
        }

        public void Add(DevelopmentCard card, int amount = 1)
        {
            ValidateCard(card, nameof(card));
            ValidateAmount(amount, nameof(amount));

            AddTo(_playableCards, card, amount);
        }

        public void AddNew(DevelopmentCard card, int amount = 1)
        {
            ValidateCard(card, nameof(card));
            ValidateAmount(amount, nameof(amount));

            AddTo(_newCards, card, amount);
        }

        public void ReleaseNewCards()
        {
            foreach (var entry in _newCards)
            {
                AddTo(_playableCards, entry.Key, entry.Value);
            }

            _newCards.Clear();
        }

        public bool Remove(DevelopmentCard card, int amount = 1)
        {
            ValidateCard(card, nameof(card));
            ValidateAmount(amount, nameof(amount));

            return RemoveFrom(_playableCards, card, amount);
        }

        public int CountByCard(DevelopmentCard card)
        {
            ValidateCard(card, nameof(card));
            return CountByCard(_playableCards, card) + CountByCard(_newCards, card);
        }

        public int CountByType(DevelopmentCardType type)
        {
            return CountByType(_playableCards, type) + CountByType(_newCards, type);
        }

        public int CountPlayableByType(DevelopmentCardType type)
        {
            return CountByType(_playableCards, type);
        }

        public int CountNewByType(DevelopmentCardType type)
        {
            return CountByType(_newCards, type);
        }

        private static void AddTo(Dictionary<DevelopmentCard, int> cards, DevelopmentCard card, int amount)
        {
            if (!cards.ContainsKey(card))
            {
                cards[card] = 0;
            }

            checked
            {
                cards[card] += amount;
            }
        }

        private static bool RemoveFrom(Dictionary<DevelopmentCard, int> cards, DevelopmentCard card, int amount)
        {
            if (!cards.TryGetValue(card, out int currentAmount) || currentAmount < amount)
            {
                return false;
            }

            int newAmount = currentAmount - amount;
            if (newAmount == 0)
            {
                cards.Remove(card);
            }
            else
            {
                cards[card] = newAmount;
            }

            return true;
        }

        private static int CountByCard(Dictionary<DevelopmentCard, int> cards, DevelopmentCard card)
        {
            return cards.TryGetValue(card, out int amount) ? amount : 0;
        }

        private static int CountByType(Dictionary<DevelopmentCard, int> cards, DevelopmentCardType type)
        {
            int total = 0;

            foreach (var entry in cards)
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
            return TotalCards(_playableCards) + TotalCards(_newCards);
        }

        private static int TotalCards(Dictionary<DevelopmentCard, int> cards)
        {
            int total = 0;

            foreach (int amount in cards.Values)
            {
                checked
                {
                    total += amount;
                }
            }

            return total;
        }

        private Dictionary<DevelopmentCard, int> BuildTotalCards()
        {
            Dictionary<DevelopmentCard, int> cards = new();

            foreach (var entry in _playableCards)
            {
                cards[entry.Key] = entry.Value;
            }

            foreach (var entry in _newCards)
            {
                if (!cards.ContainsKey(entry.Key))
                {
                    cards[entry.Key] = 0;
                }

                cards[entry.Key] += entry.Value;
            }

            return cards;
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
