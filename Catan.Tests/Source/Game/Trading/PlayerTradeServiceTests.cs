using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Xunit;

namespace Catan.Tests.Source.Game.Trading
{
    public class PlayerTradeServiceTests
    {
        [Fact]
        public void CreateOffer_WithValidResources_Succeeds()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2), (ResourceId.Brick, 1));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2), (ResourceId.Brick, 1)),
                Resources((ResourceId.Ore, 1), (ResourceId.Wheat, 1)),
                out PlayerTradeOffer? offer);

            Assert.True(result.Success, result.Message);
            Assert.NotNull(offer);
            Assert.Same(offeringPlayer, offer!.OfferingPlayer);
            Assert.Equal(2, offer.OfferedResources[ResourceId.Wood]);
            Assert.Equal(1, offer.OfferedResources[ResourceId.Brick]);
            Assert.Equal(1, offer.RequestedResources[ResourceId.Ore]);
            Assert.Equal(1, offer.RequestedResources[ResourceId.Wheat]);
        }

        [Fact]
        public void CreateOffer_WithEmptyOfferedResources_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources(),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithEmptyRequestedResources_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources(),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithZeroQuantity_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 0)),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithNegativeQuantity_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, -1)),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithNullOfferingPlayer_Fails()
        {
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                null!,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithNullOfferedResources_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                null!,
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithNullRequestedResources_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                null!,
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithUnsupportedResource_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources(((ResourceId)999, 1)),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WithSameResourceOnBothSides_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2), (ResourceId.Brick, 1));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1), (ResourceId.Brick, 1)),
                Resources((ResourceId.Wood, 1), (ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void CreateOffer_WhenOfferingPlayerLacksOfferedResources_Fails()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 1));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2)),
                Resources((ResourceId.Ore, 1)),
                out PlayerTradeOffer? offer);

            Assert.False(result.Success);
            Assert.Null(offer);
        }

        [Fact]
        public void Execute_WhenAcceptingPlayerIsOfferingPlayer_FailsWithoutChangingInventory()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Ore, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, offeringPlayer);

            Assert.False(result.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 2);
            AssertAmount(offeringPlayer, ResourceId.Ore, 0);
        }

        [Fact]
        public void Execute_WithNullOffer_Fails()
        {
            Player acceptingPlayer = PlayerWith(2, (ResourceId.Ore, 1));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(null!, acceptingPlayer);

            Assert.False(result.Success);
            AssertAmount(acceptingPlayer, ResourceId.Ore, 1);
        }

        [Fact]
        public void Execute_WithNullAcceptingPlayer_FailsWithoutChangingInventory()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 1));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Ore, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, null!);

            Assert.False(result.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 1);
            Assert.True(offer.IsOpen);
        }

        [Fact]
        public void Execute_WithInvalidOfferShape_FailsWithoutChangingInventories()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 1));
            Player acceptingPlayer = PlayerWith(2, (ResourceId.Ore, 1));
            var offer = new PlayerTradeOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources());
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, acceptingPlayer);

            Assert.False(result.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 1);
            AssertAmount(acceptingPlayer, ResourceId.Ore, 1);
            Assert.True(offer.IsOpen);
        }

        [Fact]
        public void Execute_WhenAcceptingPlayerLacksRequestedResources_FailsWithoutChangingInventories()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            Player acceptingPlayer = PlayerWith(2, (ResourceId.Ore, 0));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2)),
                Resources((ResourceId.Ore, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, acceptingPlayer);

            Assert.False(result.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 2);
            AssertAmount(offeringPlayer, ResourceId.Ore, 0);
            AssertAmount(acceptingPlayer, ResourceId.Wood, 0);
            AssertAmount(acceptingPlayer, ResourceId.Ore, 0);
        }

        [Fact]
        public void Execute_WhenOfferingPlayerNoLongerHasOfferedResources_FailsWithoutChangingInventories()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            Player acceptingPlayer = PlayerWith(2, (ResourceId.Ore, 1));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2)),
                Resources((ResourceId.Ore, 1)));
            offeringPlayer.Inventory.Resources.Remove(ResourceId.Wood, 2);
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, acceptingPlayer);

            Assert.False(result.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 0);
            AssertAmount(offeringPlayer, ResourceId.Ore, 0);
            AssertAmount(acceptingPlayer, ResourceId.Wood, 0);
            AssertAmount(acceptingPlayer, ResourceId.Ore, 1);
        }

        [Fact]
        public void Execute_WithValidAcceptance_MovesResourcesBothWays()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2), (ResourceId.Brick, 1));
            Player acceptingPlayer = PlayerWith(2, (ResourceId.Ore, 1), (ResourceId.Wheat, 1));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2), (ResourceId.Brick, 1)),
                Resources((ResourceId.Ore, 1), (ResourceId.Wheat, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult result = service.Execute(offer, acceptingPlayer);

            Assert.True(result.Success, result.Message);
            AssertAmount(offeringPlayer, ResourceId.Wood, 0);
            AssertAmount(offeringPlayer, ResourceId.Brick, 0);
            AssertAmount(offeringPlayer, ResourceId.Ore, 1);
            AssertAmount(offeringPlayer, ResourceId.Wheat, 1);
            AssertAmount(acceptingPlayer, ResourceId.Wood, 2);
            AssertAmount(acceptingPlayer, ResourceId.Brick, 1);
            AssertAmount(acceptingPlayer, ResourceId.Ore, 0);
            AssertAmount(acceptingPlayer, ResourceId.Wheat, 0);
            Assert.False(offer.IsOpen);
        }

        [Fact]
        public void Execute_AfterSuccessfulAcceptance_FailsWithoutChangingInventories()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 2));
            Player firstAcceptingPlayer = PlayerWith(2, (ResourceId.Ore, 1));
            Player secondAcceptingPlayer = PlayerWith(3, (ResourceId.Ore, 1));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Ore, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult firstResult = service.Execute(offer, firstAcceptingPlayer);
            PlayerTradeResult secondResult = service.Execute(offer, secondAcceptingPlayer);

            Assert.True(firstResult.Success, firstResult.Message);
            Assert.False(secondResult.Success);
            AssertAmount(offeringPlayer, ResourceId.Wood, 1);
            AssertAmount(offeringPlayer, ResourceId.Ore, 1);
            AssertAmount(firstAcceptingPlayer, ResourceId.Wood, 1);
            AssertAmount(firstAcceptingPlayer, ResourceId.Ore, 0);
            AssertAmount(secondAcceptingPlayer, ResourceId.Wood, 0);
            AssertAmount(secondAcceptingPlayer, ResourceId.Ore, 1);
            Assert.False(offer.IsOpen);
        }

        [Fact]
        public void Execute_WhenOneAcceptanceFails_StillAllowsAnotherPlayerToAccept()
        {
            Player offeringPlayer = PlayerWith(1, (ResourceId.Wood, 1));
            Player playerWithoutOre = PlayerWith(2);
            Player playerWithOre = PlayerWith(3, (ResourceId.Ore, 1));
            PlayerTradeOffer offer = CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Ore, 1)));
            var service = new PlayerTradeService();

            PlayerTradeResult failedResult = service.Execute(offer, playerWithoutOre);
            PlayerTradeResult successfulResult = service.Execute(offer, playerWithOre);

            Assert.False(failedResult.Success);
            Assert.True(successfulResult.Success, successfulResult.Message);
            AssertAmount(offeringPlayer, ResourceId.Wood, 0);
            AssertAmount(offeringPlayer, ResourceId.Ore, 1);
            AssertAmount(playerWithoutOre, ResourceId.Wood, 0);
            AssertAmount(playerWithoutOre, ResourceId.Ore, 0);
            AssertAmount(playerWithOre, ResourceId.Wood, 1);
            AssertAmount(playerWithOre, ResourceId.Ore, 0);
        }

        private static PlayerTradeOffer CreateOffer(
            Player offeringPlayer,
            IReadOnlyDictionary<ResourceId, int> offeredResources,
            IReadOnlyDictionary<ResourceId, int> requestedResources)
        {
            var service = new PlayerTradeService();
            PlayerTradeResult result = service.CreateOffer(
                offeringPlayer,
                offeredResources,
                requestedResources,
                out PlayerTradeOffer? offer);

            Assert.True(result.Success, result.Message);
            Assert.NotNull(offer);
            return offer!;
        }

        private static Dictionary<ResourceId, int> Resources(params (ResourceId Resource, int Amount)[] entries)
        {
            return entries.ToDictionary(entry => entry.Resource, entry => entry.Amount);
        }

        private static Player PlayerWith(int number, params (ResourceId Resource, int Amount)[] entries)
        {
            var player = new Player(number);
            foreach (var entry in entries)
            {
                player.Inventory.Resources.Add(entry.Resource, entry.Amount);
            }

            return player;
        }

        private static void AssertAmount(Player player, ResourceId resource, int expectedAmount)
        {
            Assert.Equal(expectedAmount, player.Inventory.Resources.GetAmount(resource));
        }
    }
}
