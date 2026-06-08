using System.Collections.Generic;
using System.Linq;
using Catan.Source.Game.Bank;
using Catan.Source.Game.Board;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Catan.Source.Game.Trading;
using Xunit;
using GameBank = Catan.Source.Game.Bank.Bank;
using BoardModel = Catan.Source.Game.Board.Board;

namespace Catan.Tests.Integration
{
    public class DomainIntegrationTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void ResourceProduction_CalculatesAndDistributesResourcesThroughBank()
        {
            Player player = new(0);
            GameBank bank = new();
            Tile producingTile = Tile(
                TileType.Forest,
                6,
                VertexWithBuilding(player, BuildingType.Settlement));
            BoardModel board = Board(Tile(TileType.Desert, 0), producingTile);
            ResourceProductionCalculator calculator = new(board);

            List<ResourceProductionEntry> productions = calculator.CalculateExpectedProductions(6);
            IReadOnlyList<ResourceDistributionRequest> deliveries = bank.DistributeProduction(
                productions.Select(ToDistributionRequest));

            Assert.Single(deliveries);
            Assert.Equal(1, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(GameBank.DefaultCardsPerResource - 1, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ResourceProduction_WhenRobberBlocksTile_DoesNotDistributeResources()
        {
            Player player = new(0);
            GameBank bank = new();
            Tile producingTile = Tile(
                TileType.Forest,
                6,
                VertexWithBuilding(player, BuildingType.Settlement));
            BoardModel board = Board(Tile(TileType.Desert, 0), producingTile);
            board.MoveRobberTo(producingTile);
            ResourceProductionCalculator calculator = new(board);

            List<ResourceProductionEntry> productions = calculator.CalculateExpectedProductions(6);
            IReadOnlyList<ResourceDistributionRequest> deliveries = bank.DistributeProduction(
                productions.Select(ToDistributionRequest));

            Assert.Empty(productions);
            Assert.Empty(deliveries);
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(GameBank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wood));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void PlayerTrade_CreateOfferAndAccept_MovesResourcesBetweenPlayers()
        {
            Player offeringPlayer = PlayerWithResources(0, (ResourceId.Wood, 2));
            Player acceptingPlayer = PlayerWithResources(1, (ResourceId.Brick, 1));
            PlayerTradeService service = new();

            PlayerTradeResult createResult = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 2)),
                Resources((ResourceId.Brick, 1)),
                out PlayerTradeOffer offer);
            PlayerTradeResult executeResult = service.Execute(offer, acceptingPlayer);

            Assert.True(createResult.Success, createResult.Message);
            Assert.True(executeResult.Success, executeResult.Message);
            Assert.False(offer.IsOpen);
            Assert.Equal(0, offeringPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(1, offeringPlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(2, acceptingPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, acceptingPlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void PlayerTrade_WhenOfferingPlayerSpendsOfferedResourceBeforeAccept_Fails()
        {
            Player offeringPlayer = PlayerWithResources(0, (ResourceId.Wood, 1));
            Player acceptingPlayer = PlayerWithResources(1, (ResourceId.Brick, 1));
            PlayerTradeService service = new();

            PlayerTradeResult createResult = service.CreateOffer(
                offeringPlayer,
                Resources((ResourceId.Wood, 1)),
                Resources((ResourceId.Brick, 1)),
                out PlayerTradeOffer offer);
            offeringPlayer.Inventory.Resources.Remove(ResourceId.Wood, 1);
            PlayerTradeResult executeResult = service.Execute(offer, acceptingPlayer);

            Assert.True(createResult.Success, createResult.Message);
            Assert.False(executeResult.Success);
            Assert.True(offer.IsOpen);
            Assert.Equal(0, offeringPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, offeringPlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
            Assert.Equal(0, acceptingPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(1, acceptingPlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void DevelopmentCardPurchase_PaysBankDrawsDeckAndStoresNewCard()
        {
            Player player = new(0);
            GameBank bank = new();
            DevelopmentCard expectedCard = new(DevelopmentCardType.Monopoly);
            DevelopmentCardDeck deck = new(new[] { expectedCard });
            bank.Give(
                player.Inventory.Resources,
                Resources(
                    (ResourceId.Wool, 1),
                    (ResourceId.Wheat, 1),
                    (ResourceId.Ore, 1)));
            DevelopmentCardPurchaseService service = new();

            DevelopmentCardPurchaseResult result = service.Purchase(player, bank, deck);

            Assert.True(result.Success, result.Message);
            Assert.Equal(expectedCard, result.Card);
            Assert.Equal(0, deck.Count);
            Assert.Equal(1, player.Inventory.DevelopmentCards.CountNewByType(DevelopmentCardType.Monopoly));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wool));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, player.Inventory.Resources.GetAmount(ResourceId.Ore));
            Assert.Equal(GameBank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wool));
            Assert.Equal(GameBank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Wheat));
            Assert.Equal(GameBank.DefaultCardsPerResource, bank.GetAmount(ResourceId.Ore));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void MonopolyActivation_CollectsChosenResourceFromOtherPlayers()
        {
            Player activePlayer = PlayerWithResources(0, (ResourceId.Wheat, 1));
            Player secondPlayer = PlayerWithResources(1, (ResourceId.Wheat, 2), (ResourceId.Wood, 1));
            Player thirdPlayer = PlayerWithResources(2, (ResourceId.Wheat, 3));
            activePlayer.Inventory.DevelopmentCards.Add(new DevelopmentCard(DevelopmentCardType.Monopoly));
            DevelopmentCardActivationService service = new();

            DevelopmentCardActivationResult result = service.UseMonopoly(
                activePlayer,
                new[] { activePlayer, secondPlayer, thirdPlayer },
                hasUsedDevelopmentCardThisTurn: false,
                ResourceId.Wheat);

            Assert.True(result.Success, result.Message);
            Assert.Equal(6, activePlayer.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, secondPlayer.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(0, thirdPlayer.Inventory.Resources.GetAmount(ResourceId.Wheat));
            Assert.Equal(1, secondPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, activePlayer.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void SevenRuleAndRobber_DiscardToBankMoveRobberAndStealFromAdjacentTarget()
        {
            Player currentPlayer = new(0);
            Player targetPlayer = PlayerWithResources(1, (ResourceId.Wood, 8));
            GameBank bank = new();
            SevenRule sevenRule = new();
            RobberRule robberRule = new();
            Tile targetTile = Tile(
                TileType.Brick,
                8,
                VertexWithBuilding(targetPlayer, BuildingType.Settlement));
            BoardModel board = Board(Tile(TileType.Desert, 0), targetTile);

            Dictionary<ResourceId, int> discarded = sevenRule.DiscardResourcesToBank(targetPlayer, bank);
            bool moved = board.MoveRobberTo(targetTile);
            List<Player> targets = robberRule.GetRobberyTargets(currentPlayer, board.RobberTile);
            bool stole = robberRule.TryStealRandomResource(currentPlayer, targetPlayer, out ResourceId stolenResource);

            Assert.Equal(4, discarded[ResourceId.Wood]);
            Assert.Equal(GameBank.DefaultCardsPerResource + 4, bank.GetAmount(ResourceId.Wood));
            Assert.True(moved);
            Assert.Same(targetTile, board.RobberTile);
            Assert.Single(targets);
            Assert.Same(targetPlayer, targets[0]);
            Assert.True(stole);
            Assert.Equal(ResourceId.Wood, stolenResource);
            Assert.Equal(1, currentPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(3, targetPlayer.Inventory.Resources.GetAmount(ResourceId.Wood));
            Assert.Equal(0, targetPlayer.Inventory.Resources.GetAmount(ResourceId.Brick));
        }

        private static ResourceDistributionRequest ToDistributionRequest(ResourceProductionEntry production)
        {
            return new ResourceDistributionRequest(
                production.Player.Inventory.Resources,
                production.Resource,
                production.Amount);
        }

        private static BoardModel Board(params Tile[] tiles)
        {
            return new BoardModel(0, 0, null!, tiles.ToList(), [], new BoardGraph());
        }

        private static Tile Tile(TileType type, int diceNumber, params TileVertex[] vertices)
        {
            return new Tile(0, 0, null!, type, diceNumber, vertices, null!);
        }

        private static TileVertex VertexWithBuilding(Player player, BuildingType buildingType)
        {
            TileVertex vertex = new(0, 0, null!, null!);
            vertex.PlaceBuilding(new Building(player, buildingType));
            return vertex;
        }

        private static Player PlayerWithResources(int playerNumber, params (ResourceId Resource, int Amount)[] resources)
        {
            Player player = new(playerNumber);
            foreach ((ResourceId resource, int amount) in resources)
            {
                player.Inventory.Resources.Add(resource, amount);
            }

            return player;
        }

        private static Dictionary<ResourceId, int> Resources(params (ResourceId Resource, int Amount)[] resources)
        {
            Dictionary<ResourceId, int> result = new();
            foreach ((ResourceId resource, int amount) in resources)
            {
                result[resource] = amount;
            }

            return result;
        }
    }
}
