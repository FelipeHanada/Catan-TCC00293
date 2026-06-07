using System;
using System.Collections.Generic;
using Catan.Source.Game.Resources;
using GameBank = Catan.Source.Game.Bank.Bank;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Rules
{
    public class SevenRule
    {
        private const int MinimumResourcesToDiscard = 8;

        public bool ShouldDiscard(GamePlayer player)
        {
            return player.Inventory.GetTotalResources() >= MinimumResourcesToDiscard;
        }

        public int GetDiscardAmount(GamePlayer player)
        {
            return player.Inventory.GetTotalResources() / 2;
        }

        public Dictionary<ResourceId, int> DiscardResourcesToBank(GamePlayer player, GameBank bank)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(bank);

            if (!ShouldDiscard(player))
            {
                return new Dictionary<ResourceId, int>();
            }

            Dictionary<ResourceId, int> discardedResources = player.Inventory.DiscardResources(GetDiscardAmount(player));
            foreach (KeyValuePair<ResourceId, int> discardedResource in discardedResources)
            {
                bank.Resources.Add(discardedResource.Key, discardedResource.Value);
            }

            return discardedResources;
        }
    }
}
