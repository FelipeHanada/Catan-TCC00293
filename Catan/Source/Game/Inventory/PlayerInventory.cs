using Catan.Source.Game.Resources;
using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Inventory
{
    public class PlayerInventory
    {
        // Temporario pra descarte automatico enquanto nao temos a escolha do jogador.
        private static readonly ResourceId[] _discardOrder = [
            ResourceId.Wood,
            ResourceId.Wool,
            ResourceId.Brick,
            ResourceId.Ore,
            ResourceId.Wheat,
        ];

        public ResourceInventory Resources { get; }
        public DevelopmentCardInventory DevelopmentCards { get; }
        public int TotalResourceCards => Resources.TotalCards();
        public int TotalDevelopmentCards => DevelopmentCards.Count;

        public PlayerInventory()
        {
            Resources = new ResourceInventory();
            DevelopmentCards = new DevelopmentCardInventory();
        }

        public void AddResource(ResourceId resource, int amount)
        {
            Resources.Add(resource, amount);
        }

        public int GetTotalResources()
        {
            return TotalResourceCards;
        }

        public bool HasResources()
        {
            return TotalResourceCards > 0;
        }

        public int RemoveResource(ResourceId resource, int amount)
        {
            int removedAmount = Math.Min(Resources.GetAmount(resource), amount);
            Resources.Remove(resource, removedAmount);
            return removedAmount;
        }

        public Dictionary<ResourceId, int> DiscardResources(int amount)
        {
            Dictionary<ResourceId, int> discardedResources = new();
            int remainingAmount = amount;

            foreach (ResourceId resource in _discardOrder)
            {
                if (remainingAmount <= 0)
                {
                    break;
                }

                int removedAmount = RemoveResource(resource, remainingAmount);
                if (removedAmount > 0)
                {
                    discardedResources[resource] = removedAmount;
                    remainingAmount -= removedAmount;
                }
            }

            return discardedResources;
        }

        public bool TryRemoveRandomResource(out ResourceId resource)
        {
            int totalResources = TotalResourceCards;
            if (totalResources == 0)
            {
                resource = default;
                return false;
            }

            int resourceIndex = Random.Shared.Next(totalResources);
            foreach (KeyValuePair<ResourceId, int> resourceAmount in Resources.Resources)
            {
                if (resourceIndex < resourceAmount.Value)
                {
                    resource = resourceAmount.Key;
                    Resources.Remove(resource, 1);
                    return true;
                }

                resourceIndex -= resourceAmount.Value;
            }

            resource = default;
            return false;
        }
    }
}
