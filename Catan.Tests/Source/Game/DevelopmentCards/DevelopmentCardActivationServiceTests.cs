using System.Collections.Generic;
using Catan.Source.Game.Bank;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Xunit;

namespace Catan.Tests.Source.Game.DevelopmentCards
{
    public class DevelopmentCardActivationServiceTests
    {
        [Fact]
        public void UseYearOfPlenty_WithDifferentAvailableResources_TransfersResourcesAndConsumesCard()
        {
            Bank bank = new();
            Player player = PlayerWithCard(DevelopmentCardType.YearOfPlenty);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wood,
                ResourceId.Ore);

            Assert.True(result.Success, result.Message);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(Bank.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wood));
            Assert.Equal(Bank.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(0, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty));
        }

        [Fact]
        public void UseYearOfPlenty_WithSameAvailableResource_TransfersTwoResourcesAndConsumesCard()
        {
            Bank bank = new();
            Player player = PlayerWithCard(DevelopmentCardType.YearOfPlenty);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Ore,
                ResourceId.Ore);

            Assert.True(result.Success, result.Message);
            Assert.Equal(2, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(Bank.DefaultCardsPerResource - 2, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(0, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty));
        }

        [Fact]
        public void UseYearOfPlenty_WithInsufficientBankResources_FailsWithoutChangingState()
        {
            Bank bank = new();
            bank.Resources.Remove(ResourceId.Ore, Bank.DefaultCardsPerResource);
            Player player = PlayerWithCard(DevelopmentCardType.YearOfPlenty);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Ore,
                ResourceId.Ore);

            Assert.False(result.Success);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(0, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty));
        }

        [Fact]
        public void UseYearOfPlenty_WithDifferentResourcesAndOneMissing_FailsWithoutChangingState()
        {
            Bank bank = new();
            bank.Resources.Remove(ResourceId.Ore, Bank.DefaultCardsPerResource);
            Player player = PlayerWithCard(DevelopmentCardType.YearOfPlenty);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wood,
                ResourceId.Ore);

            Assert.False(result.Success);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(Bank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wood));
            Assert.Equal(0, bank.GetAmount(ResourceId.Ore));
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty));
        }

        [Fact]
        public void UseYearOfPlenty_WithoutPlayableCard_FailsWithoutChangingState()
        {
            Bank bank = new();
            Player player = new(0);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wood,
                ResourceId.Ore);

            Assert.False(result.Success);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(Bank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wood));
            Assert.Equal(Bank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Ore));
        }

        [Fact]
        public void UseMonopoly_WithResourcesInOtherPlayers_TransfersAllAndConsumesCard()
        {
            Player activePlayer = PlayerWithCard(DevelopmentCardType.Monopoly);
            Player playerTwo = new(1);
            Player playerThree = new(2);
            Player playerFour = new(3);
            playerTwo.Inventory.Resources.Add(ResourceId.Wood, 2);
            playerFour.Inventory.Resources.Add(ResourceId.Wood, 1);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseMonopoly(
                activePlayer,
                new[] { activePlayer, playerTwo, playerThree, playerFour },
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wood);

            Assert.True(result.Success, result.Message);
            Assert.Equal(3, activePlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, playerTwo.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, playerThree.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, playerFour.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, activePlayer.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly));
        }

        [Fact]
        public void UseMonopoly_WhenNobodyHasResource_ConsumesCardWithoutChangingResources()
        {
            Player activePlayer = PlayerWithCard(DevelopmentCardType.Monopoly);
            Player playerTwo = new(1);
            Player playerThree = new(2);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseMonopoly(
                activePlayer,
                new[] { activePlayer, playerTwo, playerThree },
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Brick);

            Assert.True(result.Success, result.Message);
            Assert.Equal(0, activePlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(0, playerTwo.Inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(0, playerThree.Inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(0, activePlayer.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly));
        }

        [Fact]
        public void UseMonopoly_WithoutPlayableCard_FailsWithoutChangingState()
        {
            Player activePlayer = new(0);
            Player playerTwo = new(1);
            playerTwo.Inventory.Resources.Add(ResourceId.Wood, 2);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseMonopoly(
                activePlayer,
                new[] { activePlayer, playerTwo },
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wood);

            Assert.False(result.Success);
            Assert.Equal(0, activePlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(2, playerTwo.Inventory.Resources.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void UseMonopoly_WhenCardAlreadyUsedThisTurn_FailsWithoutChangingState()
        {
            Player activePlayer = PlayerWithCard(DevelopmentCardType.Monopoly);
            Player playerTwo = new(1);
            playerTwo.Inventory.Resources.Add(ResourceId.Wood, 2);
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseMonopoly(
                activePlayer,
                new[] { activePlayer, playerTwo },
                hasUsedDevelopmentCardThisTurn: true,
                ResourceId.Wood);

            Assert.False(result.Success);
            Assert.Equal(0, activePlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(2, playerTwo.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(1, activePlayer.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly));
        }

        [Fact]
        public void UseYearOfPlenty_WhenCardAlreadyUsedThisTurn_FailsWithoutChangingState()
        {
            Bank bank = new();
            Player player = PlayerWithCard(DevelopmentCardType.YearOfPlenty);
            player.Inventory.DevelopmentCards.Add(new DevelopmentCard(DevelopmentCardType.Monopoly));
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn: true,
                ResourceId.Wood,
                ResourceId.Ore);

            Assert.False(result.Success);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty));
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly));
        }

        private static Player PlayerWithCard(DevelopmentCardType type)
        {
            Player player = new(0);
            player.Inventory.DevelopmentCards.Add(new DevelopmentCard(type));
            return player;
        }
    }
}
