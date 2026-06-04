using System.Collections.Generic;
using Catan.Source.Game.Resources;

namespace Catan.Source.Game.Trading
{
    public class BankTradeSelection
    {
        public ResourceId PaidResource { get; }
        public ResourceId ReceivedResource { get; }

        private BankTradeSelection(ResourceId paidResource, ResourceId receivedResource)
        {
            PaidResource = paidResource;
            ReceivedResource = receivedResource;
        }

        public static bool TryCreate(
            IReadOnlyDictionary<ResourceId, int> paidResources,
            IReadOnlyDictionary<ResourceId, int> receivedResources,
            int expectedPaidAmount,
            out BankTradeSelection selection,
            out string message)
        {
            selection = null;

            if (!TryGetSingleResource(paidResources, out ResourceId paidResource, out int paidAmount))
            {
                message = "Selecione exatamente um recurso para pagar ao banco.";
                return false;
            }

            if (!TryGetSingleResource(receivedResources, out ResourceId receivedResource, out int receivedAmount))
            {
                message = "Selecione exatamente um recurso para receber do banco.";
                return false;
            }

            if (paidResource == receivedResource)
            {
                message = "Recurso pago e recebido devem ser diferentes.";
                return false;
            }

            if (paidAmount != expectedPaidAmount)
            {
                message = $"Quantidade paga deve ser exatamente {expectedPaidAmount}.";
                return false;
            }

            if (receivedAmount != 1)
            {
                message = "Quantidade recebida do banco deve ser exatamente 1.";
                return false;
            }

            selection = new BankTradeSelection(paidResource, receivedResource);
            message = "Seleção de troca com banco válida.";
            return true;
        }

        private static bool TryGetSingleResource(
            IReadOnlyDictionary<ResourceId, int> resources,
            out ResourceId resource,
            out int amount)
        {
            resource = default;
            amount = 0;

            if (resources == null || resources.Count != 1)
            {
                return false;
            }

            foreach (var entry in resources)
            {
                resource = entry.Key;
                amount = entry.Value;
                return amount > 0;
            }

            return false;
        }
    }
}
