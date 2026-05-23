using System;
using System.Collections.Generic;
using Catan.Source.Game.Resources;

namespace Catan.Source.Game.Inventory
{
    public class ResourceInventory
    {
        private readonly Dictionary<ResourceId, int> _resources;

        public IReadOnlyDictionary<ResourceId, int> Resources => _resources;

        public ResourceInventory()
        {
            _resources = new Dictionary<ResourceId, int>();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                _resources[resource] = 0;
            }
        }

        public int GetAmount(ResourceId resource)
        {
            ValidateResource(resource, nameof(resource));
            return _resources[resource];
        }

        public int TotalCards()
        {
            int total = 0;

            foreach (int amount in _resources.Values)
            {
                total += amount;
            }

            return total;
        }

        public bool Has(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));
            return _resources[resource] >= amount;
        }

        public bool HasEnough(IReadOnlyDictionary<ResourceId, int> resources)
        {
            var entries = GetValidatedEntries(resources, nameof(resources));
            return HasEnoughValidated(entries);
        }

        public void Add(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));

            checked
            {
                _resources[resource] += amount;
            }
        }

        public void Add(IReadOnlyDictionary<ResourceId, int> resources)
        {
            var entries = GetValidatedEntries(resources, nameof(resources));
            EnsureCanAdd(entries);

            foreach (var entry in entries)
            {
                _resources[entry.Key] += entry.Value;
            }
        }

        public void Remove(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));

            if (_resources[resource] < amount)
            {
                throw new InvalidOperationException("Recursos insuficientes para remover.");
            }

            _resources[resource] -= amount;
        }

        public void Remove(IReadOnlyDictionary<ResourceId, int> resources)
        {
            var entries = GetValidatedEntries(resources, nameof(resources));

            if (!HasEnoughValidated(entries))
            {
                throw new InvalidOperationException("Recursos insuficientes para remover.");
            }

            foreach (var entry in entries)
            {
                _resources[entry.Key] -= entry.Value;
            }
        }

        public bool TryRemove(ResourceId resource, int amount)
        {
            ValidateResourceAmount(resource, amount, nameof(resource), nameof(amount));

            if (_resources[resource] < amount)
            {
                return false;
            }

            _resources[resource] -= amount;
            return true;
        }

        public bool TryRemove(IReadOnlyDictionary<ResourceId, int> resources)
        {
            var entries = GetValidatedEntries(resources, nameof(resources));

            if (!HasEnoughValidated(entries))
            {
                return false;
            }

            foreach (var entry in entries)
            {
                _resources[entry.Key] -= entry.Value;
            }

            return true;
        }

        private bool HasEnoughValidated(IReadOnlyList<KeyValuePair<ResourceId, int>> resources)
        {
            foreach (var entry in resources)
            {
                if (_resources[entry.Key] < entry.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private void EnsureCanAdd(IReadOnlyList<KeyValuePair<ResourceId, int>> resources)
        {
            foreach (var entry in resources)
            {
                checked
                {
                    _ = _resources[entry.Key] + entry.Value;
                }
            }
        }

        private List<KeyValuePair<ResourceId, int>> GetValidatedEntries(IReadOnlyDictionary<ResourceId, int> resources, string paramName)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(paramName);
            }

            var entries = new List<KeyValuePair<ResourceId, int>>();

            foreach (var entry in resources)
            {
                ValidateResource(entry.Key, paramName);
                ValidateAmount(entry.Value, paramName);
                entries.Add(entry);
            }

            return entries;
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
