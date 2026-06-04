using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using GameBank = Catan.Source.Game.Bank.Bank;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.DevelopmentCards
{
    public class DevelopmentCardPurchaseService
    {
        private static readonly IReadOnlyDictionary<ResourceId, int> Cost = new Dictionary<ResourceId, int>
        {
            [ResourceId.Wool] = 1,
            [ResourceId.Wheat] = 1,
            [ResourceId.Ore] = 1,
        };

        public DevelopmentCardPurchaseResult Purchase(GamePlayer player, GameBank bank, DevelopmentCardDeck deck)
        {
            DevelopmentCardPurchaseResult canPurchase = CanPurchase(player, bank, deck);
            if (!canPurchase.Success)
            {
                return canPurchase;
            }

            ResourceInventory resources = player.Inventory.Resources;

            foreach (var entry in Cost)
            {
                bank.Receive(resources, entry.Key, entry.Value);
            }

            DevelopmentCard card = deck.Draw();
            player.Inventory.DevelopmentCards.AddNew(card);
            return DevelopmentCardPurchaseResult.Ok("Carta de desenvolvimento comprada.", card);
        }

        public DevelopmentCardPurchaseResult CanPurchase(GamePlayer player, GameBank bank, DevelopmentCardDeck deck)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (bank == null)
            {
                throw new ArgumentNullException(nameof(bank));
            }

            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (!deck.HasCards)
            {
                return DevelopmentCardPurchaseResult.Fail("Baralho de desenvolvimento vazio.");
            }

            ResourceInventory resources = player.Inventory.Resources;
            if (!resources.HasEnough(Cost))
            {
                return DevelopmentCardPurchaseResult.Fail("Recursos insuficientes para comprar carta de desenvolvimento.");
            }

            foreach (var entry in Cost)
            {
                if (!bank.CanReceive(entry.Key, entry.Value))
                {
                    return DevelopmentCardPurchaseResult.Fail("Banco nao pode receber os recursos da compra.");
                }
            }

            return DevelopmentCardPurchaseResult.Ok("Compra disponivel.");
        }
    }
}
