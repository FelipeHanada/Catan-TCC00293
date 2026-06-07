using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using Xunit;
using BankModel = Catan.Source.Game.Bank.Bank;
using ResourceDistributionRequest = Catan.Source.Game.Bank.ResourceDistributionRequest;

namespace Catan.Tests.Source.Game.Bank
{
    public class BankTests
    {
        [Fact]
        public void Constructor_FillsEveryResourceWithDefaultAmount()
        {
            BankModel bank = new();

            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(resource));
            }
        }

        [Fact]
        public void Give_MovesResourceFromBankToInventory()
        {
            BankModel bank = new();
            ResourceInventory inventory = new();

            bank.Give(inventory, ResourceId.Wood, 2);

            Assert.Equal(2, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(BankModel.DefaultCardsPerResource - 2, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void Give_WhenBankInsufficient_ThrowsWithoutChangingInventory()
        {
            BankModel bank = new(cardsPerResource: 1);
            ResourceInventory inventory = new();

            Assert.Throws<InvalidOperationException>(() => bank.Give(inventory, ResourceId.Wood, 2));

            Assert.Equal(0, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void Receive_MovesResourceFromInventoryToBank()
        {
            BankModel bank = new();
            ResourceInventory inventory = new();
            bank.Give(inventory, ResourceId.Brick, 1);

            bank.Receive(inventory, ResourceId.Brick, 1);

            Assert.Equal(0, inventory.GetAmount(ResourceId.Brick));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Brick));
        }

        [Fact]
        public void Trade_WhenValid_MovesBothSides()
        {
            BankModel bank = new();
            ResourceInventory inventory = new();
            bank.Give(inventory, ResourceId.Wood, 1);

            bank.Trade(inventory, ResourceId.Wood, 1, ResourceId.Ore, 1);

            Assert.Equal(0, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, inventory.GetAmount(ResourceId.Ore));
            Assert.Equal(BankModel.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wood));
            Assert.Equal(BankModel.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Ore));
        }

        [Fact]
        public void DistributeProduction_WhenBankCanPayEveryone_PaysAllRequests()
        {
            BankModel bank = new(cardsPerResource: 3);
            ResourceInventory firstInventory = new();
            ResourceInventory secondInventory = new();
            List<ResourceDistributionRequest> requests = new()
            {
                new ResourceDistributionRequest(firstInventory, ResourceId.Wood, 1),
                new ResourceDistributionRequest(secondInventory, ResourceId.Wood, 2),
                new ResourceDistributionRequest(firstInventory, ResourceId.Brick, 1)
            };

            IReadOnlyList<ResourceDistributionRequest> deliveries = bank.DistributeProduction(requests);

            Assert.Equal(3, deliveries.Count);
            Assert.Equal(1, firstInventory.GetAmount(ResourceId.Wood));
            Assert.Equal(2, secondInventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, firstInventory.GetAmount(ResourceId.Brick));
            Assert.Equal(0, bank.GetAmount(ResourceId.Wood));
            Assert.Equal(2, bank.GetAmount(ResourceId.Brick));
        }

        [Fact]
        public void DistributeProduction_WhenMultiplePlayersNeedMoreThanBankHas_PaysNobodyForThatResource()
        {
            BankModel bank = new(cardsPerResource: 1);
            ResourceInventory firstInventory = new();
            ResourceInventory secondInventory = new();
            List<ResourceDistributionRequest> requests = new()
            {
                new ResourceDistributionRequest(firstInventory, ResourceId.Wood, 1),
                new ResourceDistributionRequest(secondInventory, ResourceId.Wood, 1)
            };

            IReadOnlyList<ResourceDistributionRequest> deliveries = bank.DistributeProduction(requests);

            Assert.Empty(deliveries);
            Assert.Equal(0, firstInventory.GetAmount(ResourceId.Wood));
            Assert.Equal(0, secondInventory.GetAmount(ResourceId.Wood));
            Assert.Equal(1, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        public void DistributeProduction_WhenOnePlayerNeedsMoreThanBankHas_PaysPartialAmount()
        {
            BankModel bank = new(cardsPerResource: 1);
            ResourceInventory inventory = new();
            List<ResourceDistributionRequest> requests = new()
            {
                new ResourceDistributionRequest(inventory, ResourceId.Wood, 2)
            };

            IReadOnlyList<ResourceDistributionRequest> deliveries = bank.DistributeProduction(requests);

            Assert.Single(deliveries);
            Assert.Equal(1, deliveries[0].Amount);
            Assert.Equal(1, inventory.GetAmount(ResourceId.Wood));
            Assert.Equal(0, bank.GetAmount(ResourceId.Wood));
        }
    }
}
