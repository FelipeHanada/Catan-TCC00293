using System.Collections.Generic;
using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Catan.Source.Game.Resources
{
    public class ResourceBankPlaceholder
    {
        public void DistributeResources(IEnumerable<ResourceProductionEntry> productions)
        {
            foreach (ResourceProductionEntry production in productions)
            {
                production.Player.Inventory.AddResource(production.Resource, production.Amount);

                #if DEBUG
                DiagnosticsDebug.WriteLine(
                    $"Producao: Jogador {production.Player.PlayerNumber} recebeu {production.Amount} {production.Resource}."
                );
                #endif
            }
        }
    }
}
