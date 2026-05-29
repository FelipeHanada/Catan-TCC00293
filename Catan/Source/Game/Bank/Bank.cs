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

        public ResourceInventory Resources { get; }

        public Bank(int cardsPerResource = DefaultCardsPerResource)
        {
            if (cardsPerResource < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cardsPerResource), cardsPerResource, "Quantidade inicial de cartas não pode ser negativa.");
            }

            _cardsPerResource = cardsPerResource;
            Resources = new ResourceInventory();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                Resources.Add(resource, cardsPerResource);
            }
        }

        public int GetAmount(ResourceId resource)
        {
            ValidateResource(resource, nameof(resource));
            return Resources.GetAmount(resource);
        }

        public bool CanGive(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));
            return Resources.Has(resource, amount);
        }

        public bool CanReceive(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));
            return amount <= _cardsPerResource - Resources.GetAmount(resource);
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
            Resources.Remove(resource, amount);
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
            Resources.Add(resource, amount);
        }

        public bool CanTrade(
            ResourceInventory inventory,
            ResourceId giveToBank,
            int giveAmount,
            ResourceId receiveFromBank,
            int receiveAmount)
        {
            ValidateTrade(inventory, giveToBank, giveAmount, receiveFromBank, receiveAmount);

            return inventory.Has(giveToBank, giveAmount)
                && CanReceive(giveToBank, giveAmount)
                && CanGive(receiveFromBank, receiveAmount);
        }

        public void Trade(
            ResourceInventory inventory,
            ResourceId giveToBank,
            int giveAmount,
            ResourceId receiveFromBank,
            int receiveAmount)
        {
            ValidateTrade(inventory, giveToBank, giveAmount, receiveFromBank, receiveAmount);

            checked
            {
                _ = inventory.GetAmount(receiveFromBank) + receiveAmount;
            }

            if (!inventory.Has(giveToBank, giveAmount))
            {
                throw new InvalidOperationException("Jogador não possui recursos suficientes para a troca.");
            }

            if (!CanReceive(giveToBank, giveAmount))
            {
                throw new InvalidOperationException("Banco não pode receber essa quantidade de recursos.");
            }

            if (!CanGive(receiveFromBank, receiveAmount))
            {
                throw new InvalidOperationException("Banco não possui recursos suficientes.");
            }

            inventory.Remove(giveToBank, giveAmount);
            Resources.Add(giveToBank, giveAmount);

            Resources.Remove(receiveFromBank, receiveAmount);
            inventory.Add(receiveFromBank, receiveAmount);
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

                if (Resources.GetAmount(resource) >= totalRequested)
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
                int amount = Math.Min(Resources.GetAmount(resource), request.Value);
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

                ValidateInventory(production.RecipientInventory, nameof(production.RecipientInventory));
                ValidateResourceAmount(
                    production.Resource,
                    production.Amount,
                    nameof(production.Resource),
                    nameof(production.Amount));

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

        private void ValidateTrade(
            ResourceInventory inventory,
            ResourceId giveToBank,
            int giveAmount,
            ResourceId receiveFromBank,
            int receiveAmount)
        {
            ValidateInventory(inventory, nameof(inventory));
            ValidateResourceAmount(giveToBank, giveAmount, nameof(giveToBank), nameof(giveAmount));
            ValidateResourceAmount(receiveFromBank, receiveAmount, nameof(receiveFromBank), nameof(receiveAmount));

            if (giveAmount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(giveAmount), "Quantidades da troca devem ser maiores que zero.");
            }

            if (receiveAmount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(receiveAmount), "Quantidades da troca devem ser maiores que zero.");
            }

            if (giveToBank == receiveFromBank)
            {
                throw new ArgumentException("Recursos da troca devem ser diferentes.");
            }
        }

        private void ValidateResourceAmount(ResourceId resource, int amount, string resourceParamName, string amountParamName)
        {
            ValidateResource(resource, resourceParamName);
            ValidateAmount(amount, amountParamName);
        }

        private void ValidateResource(ResourceId resource, string paramName)
        {
            if (!Resources.Resources.ContainsKey(resource))
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
