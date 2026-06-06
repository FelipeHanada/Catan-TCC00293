using System;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;
using BankModel = Catan.Source.Game.Bank.Bank;

namespace Catan.Tests.Source.Game.DevelopmentCards
{
    public class DevelopmentCardPurchaseServiceTests
    {
        [Fact]
        public void Purchase_WithResourcesAndCard_SucceedsAtomically()
        {
            BankModel bank = new();
            Player player = new(0);
            GiveFromBank(bank, player, ResourceId.Wool, 1);
            GiveFromBank(bank, player, ResourceId.Wheat, 1);
            GiveFromBank(bank, player, ResourceId.Ore, 1);
            DevelopmentCardDeck deck = DeckWith(DevelopmentCardType.Knight);
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.Purchase(player, bank, deck);

            Assert.True(result.Success, result.Message);
            Assert.NotNull(result.Card);
            Assert.Equal(DevelopmentCardType.Knight, result.Card!.Type);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wool));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wheat));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(0, deck.Count);
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountNewByType(DevelopmentCardType.Knight));
            Assert.Equal(0, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Knight));
        }

        [Fact]
        public void CanPurchase_WithResourcesAndCard_SucceedsWithoutChangingState()
        {
            BankModel bank = new();
            Player player = new(0);
            GiveFromBank(bank, player, ResourceId.Wool, 1);
            GiveFromBank(bank, player, ResourceId.Wheat, 1);
            GiveFromBank(bank, player, ResourceId.Ore, 1);
            DevelopmentCardDeck deck = DeckWith(DevelopmentCardType.Knight);
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.CanPurchase(player, bank, deck);

            Assert.True(result.Success, result.Message);
            Assert.Null(result.Card);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(1, deck.Count);
            Assert.Equal(0, player.Inventory.DevelopmentCards.Count);
        }

        [Fact]
        public void CanPurchase_WithoutEnoughResources_FailsWithoutChangingState()
        {
            BankModel bank = new();
            Player player = new(0);
            GiveFromBank(bank, player, ResourceId.Wool, 1);
            GiveFromBank(bank, player, ResourceId.Wheat, 1);
            DevelopmentCardDeck deck = DeckWith(DevelopmentCardType.Knight);
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.CanPurchase(player, bank, deck);

            Assert.False(result.Success);
            Assert.Null(result.Card);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(1, deck.Count);
            Assert.Equal(0, player.Inventory.DevelopmentCards.Count);
        }

        [Fact]
        public void Purchase_WithoutEnoughResources_FailsWithoutChangingState()
        {
            BankModel bank = new();
            Player player = new(0);
            GiveFromBank(bank, player, ResourceId.Wool, 1);
            GiveFromBank(bank, player, ResourceId.Wheat, 1);
            DevelopmentCardDeck deck = DeckWith(DevelopmentCardType.Knight);
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.Purchase(player, bank, deck);

            Assert.False(result.Success);
            Assert.Null(result.Card);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wool));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wheat));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(1, deck.Count);
            Assert.Equal(0, player.Inventory.DevelopmentCards.Count);
        }

        [Fact]
        public void Purchase_WithEmptyDeck_FailsWithoutChangingResources()
        {
            BankModel bank = new();
            Player player = new(0);
            GiveFromBank(bank, player, ResourceId.Wool, 1);
            GiveFromBank(bank, player, ResourceId.Wheat, 1);
            GiveFromBank(bank, player, ResourceId.Ore, 1);
            DevelopmentCardDeck deck = new(Array.Empty<DevelopmentCard>());
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.Purchase(player, bank, deck);

            Assert.False(result.Success);
            Assert.Null(result.Card);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wool));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wheat));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(0, player.Inventory.DevelopmentCards.Count);
        }

        [Fact]
        public void Purchase_WhenBankCannotReceiveResources_FailsWithoutChangingState()
        {
            BankModel bank = new();
            Player player = new(0);
            player.Inventory.Resources.Add(ResourceId.Wool, 1);
            player.Inventory.Resources.Add(ResourceId.Wheat, 1);
            player.Inventory.Resources.Add(ResourceId.Ore, 1);
            DevelopmentCardDeck deck = DeckWith(DevelopmentCardType.Knight);
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.Purchase(player, bank, deck);

            Assert.False(result.Success);
            Assert.Null(result.Card);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wool));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wheat));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(1, deck.Count);
            Assert.Equal(0, player.Inventory.DevelopmentCards.Count);
        }

        private static DevelopmentCardDeck DeckWith(DevelopmentCardType type)
        {
            return new DevelopmentCardDeck(new[] { new DevelopmentCard(type) });
        }

        private static void GiveFromBank(BankModel bank, Player player, ResourceId resource, int amount)
        {
            bank.Give(player.Inventory.Resources, resource, amount);
        }
    }
}
