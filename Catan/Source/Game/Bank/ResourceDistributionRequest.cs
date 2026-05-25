using System;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;

namespace Catan.Source.Game.Bank
{
    public class ResourceDistributionRequest
    {
        public ResourceInventory RecipientInventory { get; }
        public ResourceId Resource { get; }
        public int Amount { get; }

        // Pedido já calculado pela produção; o banco só decide se consegue entregar.
        public ResourceDistributionRequest(ResourceInventory recipientInventory, ResourceId resource, int amount)
        {
            if (recipientInventory == null)
            {
                throw new ArgumentNullException(nameof(recipientInventory), "Inventário de destino não pode ser nulo.");
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Quantidade não pode ser negativa.");
            }

            RecipientInventory = recipientInventory;
            Resource = resource;
            Amount = amount;
        }
    }
}
