using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;

namespace Catan.Source.Game.Bank
{
    public class Bank
    {
        public const int DefaultCardsPerResource = 19;

        private readonly int _cardsPerResource;
        private readonly Dictionary<ResourceId, int> _resources;

        public Bank(int cardsPerResource = DefaultCardsPerResource)
        {
            if (cardsPerResource < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cardsPerResource), cardsPerResource, "Quantidade inicial de cartas não pode ser negativa.");
            }

            _cardsPerResource = cardsPerResource;
            _resources = new Dictionary<ResourceId, int>();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                _resources[resource] = cardsPerResource;
            }
        }

        public int GetAmount(ResourceId resource)
        {
            ValidateResource(resource, nameof(resource));
            return _resources[resource];
        }

        public bool CanGive(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));
            return _resources[resource] >= amount;
        }

        public bool CanReceive(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));
            return amount <= _cardsPerResource - _resources[resource];
        }

        public void Give(ResourceInventory inventory, ResourceId resource, int amount)
        {
            ValidateInventory(inventory, nameof(inventory));
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));

            if (amount == 0)
            {
                return;
            }

            if (!CanGive(resource, amount))
            {
                throw new InvalidOperationException("Banco não possui recursos suficientes.");
            }

            inventory.Add(resource, amount);
            _resources[resource] -= amount;
        }

        public void Receive(ResourceInventory inventory, ResourceId resource, int amount)
        {
            ValidateInventory(inventory, nameof(inventory));
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));

            if (amount == 0)
            {
                return;
            }

            if (!CanReceive(resource, amount))
            {
                throw new InvalidOperationException("Banco não pode receber essa quantidade de recursos.");
            }

            inventory.Remove(resource, amount);
            _resources[resource] += amount;
        }

        public IReadOnlyList<ResourceDistributionRequest> DistributeProduction(IEnumerable<ResourceDistributionRequest> productions)
        {
            var requestsByResource = GetValidatedProductionRequests(productions);
            var deliveries = DecideProductionDeliveries(requestsByResource);

            foreach (ResourceDistributionRequest delivery in deliveries)
            {
                Give(delivery.RecipientInventory, delivery.Resource, delivery.Amount);
            }

            return deliveries;
        }

        private List<ResourceDistributionRequest> DecideProductionDeliveries(Dictionary<ResourceId, Dictionary<ResourceInventory, int>> requestsByResource)
        {
            var deliveries = new List<ResourceDistributionRequest>();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                if (!requestsByResource.TryGetValue(resource, out var requestsByInventory))
                {
                    continue;
                }

                int totalRequested = GetTotalRequested(requestsByInventory);
                if (totalRequested == 0)
                {
                    continue;
                }

                if (_resources[resource] >= totalRequested)
                {
                    AddFullDeliveries(deliveries, resource, requestsByInventory);
                    continue;
                }

                if (requestsByInventory.Count == 1)
                {
                    AddSinglePartialDelivery(deliveries, resource, requestsByInventory);
                }
            }

            return deliveries;
        }

        private void AddFullDeliveries(List<ResourceDistributionRequest> deliveries, ResourceId resource, Dictionary<ResourceInventory, int> requestsByInventory)
        {
            foreach (var request in requestsByInventory)
            {
                if (request.Value == 0)
                {
                    continue;
                }

                deliveries.Add(new ResourceDistributionRequest(request.Key, resource, request.Value));
            }
        }

        private void AddSinglePartialDelivery(List<ResourceDistributionRequest> deliveries, ResourceId resource, Dictionary<ResourceInventory, int> requestsByInventory)
        {
            foreach (var request in requestsByInventory)
            {
                int amount = Math.Min(_resources[resource], request.Value);
                if (amount > 0)
                {
                    deliveries.Add(new ResourceDistributionRequest(request.Key, resource, amount));
                }

                return;
            }
        }

        private int GetTotalRequested(Dictionary<ResourceInventory, int> requestsByInventory)
        {
            int total = 0;

            foreach (int amount in requestsByInventory.Values)
            {
                checked
                {
                    total += amount;
                }
            }

            return total;
        }

        private Dictionary<ResourceId, Dictionary<ResourceInventory, int>> GetValidatedProductionRequests(IEnumerable<ResourceDistributionRequest> productions)
        {
            if (productions == null)
            {
                throw new ArgumentNullException(nameof(productions), "Produção não pode ser nula.");
            }

            var requestsByResource = new Dictionary<ResourceId, Dictionary<ResourceInventory, int>>();

            foreach (ResourceDistributionRequest production in productions)
            {
                if (production == null)
                {
                    throw new ArgumentException("Produção não pode conter itens nulos.", nameof(productions));
                }

                ValidateInventory(production.RecipientInventory, nameof(productions));
                ValidateResourceAmount(production.Resource, production.Amount, nameof(productions), nameof(productions));

                if (production.Amount == 0)
                {
                    continue;
                }

                if (!requestsByResource.TryGetValue(production.Resource, out var requestsByInventory))
                {
                    requestsByInventory = new Dictionary<ResourceInventory, int>();
                    requestsByResource[production.Resource] = requestsByInventory;
                }

                if (!requestsByInventory.ContainsKey(production.RecipientInventory))
                {
                    requestsByInventory[production.RecipientInventory] = 0;
                }

                checked
                {
                    requestsByInventory[production.RecipientInventory] += production.Amount;
                }
            }

            return requestsByResource;
        }

        private void ValidateInventory(ResourceInventory inventory, string paramName)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(paramName, "Inventário não pode ser nulo.");
            }
        }

        private void ValidateResourceAmount(ResourceId resource, int amount, string resourceParamName, string amountParamName)
        {
            ValidateResource(resource, resourceParamName);
            ValidateAmount(amount, amountParamName);
        }

        private void ValidateResource(ResourceId resource, string paramName)
        {
            if (!_resources.ContainsKey(resource))
            {
                throw new ArgumentOutOfRangeException(paramName, resource, "Recurso não suportado.");
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
