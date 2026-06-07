using Catan.Source.Game;
using Catan.Source.Game.AI;
using Catan.Source.Game.Board;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState, PlayerTradeButtonCallback, PlayerBuildButtonCallback, PlayerDevelopmentCardButtonCallback, PlayerEndTurnButtonCallback
    {
        private const double AiActionDelaySeconds = 0.35;
        private double _aiElapsedSeconds;
        private bool _aiActed;
        private bool _aiWaitingForBuildAction;
        private bool _aiWaitingForDevelopmentCardAction;

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _aiElapsedSeconds = 0;
            _aiActed = false;
            _aiWaitingForBuildAction = false;
            _aiWaitingForDevelopmentCardAction = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Player.IsAi || _aiActed)
            {
                return;
            }

            if (_aiWaitingForBuildAction || _aiWaitingForDevelopmentCardAction)
            {
                FinishAiAction();
                return;
            }

            _aiElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_aiElapsedSeconds < AiActionDelaySeconds)
            {
                return;
            }

            if (TryStartAiBuildAction())
            {
                _aiWaitingForBuildAction = true;
                return;
            }

            if (TryExecuteAiDevelopmentCardAction())
            {
                if (_aiWaitingForDevelopmentCardAction)
                {
                    return;
                }

                FinishAiAction();
                return;
            }

            if (TryExecuteAiDevelopmentCardPurchase())
            {
                FinishAiAction();
                return;
            }

            if (TryExecuteAiTradeAction())
            {
                FinishAiAction();
                return;
            }

            FinishAiAction();
        }

        private void FinishAiAction()
        {
            _aiActed = true;
            _aiWaitingForBuildAction = false;
            _aiWaitingForDevelopmentCardAction = false;

            if (_gameScene.AiTurnMode.ShouldAutoEndTurn())
            {
                OnPlayerEndTurnButtonClicked();
            }
        }

        private bool TryStartAiBuildAction()
        {
            bool hasCityPosition = HasValidAiCityPosition();
            bool canAffordCity = Player.Inventory.Resources.HasEnough(BuildingGameState.CityCost);
            bool hasSettlementPosition = HasValidAiSettlementPosition();
            bool canAffordSettlement = Player.Inventory.Resources.HasEnough(BuildingGameState.SettlementCost);
            int settlementCount = CountAiSettlements();

            if (hasCityPosition && canAffordCity &&
                (!hasSettlementPosition || !canAffordSettlement || _gameScene.AiStrategy.ShouldBuildCityWhenPossible(settlementCount)))
            {
                return TryStartAiCityBuild();
            }

            if (hasSettlementPosition && canAffordSettlement)
            {
                if (!_gameScene.AiStrategy.ShouldBuildSettlementWhenPossible() && TryStartAiRoadBuild())
                {
                    return true;
                }

                return TryStartAiSettlementBuild();
            }

            if (hasSettlementPosition)
            {
                int missingResources = RandomAiStrategy.CountMissingResources(
                    Player.Inventory.Resources,
                    BuildingGameState.SettlementCost);

                return _gameScene.AiStrategy.ShouldBuildRoadWhileWaitingForSettlement(missingResources)
                    && TryStartAiRoadBuild();
            }

            return TryStartAiRoadBuild();
        }

        private bool TryExecuteAiDevelopmentCardAction()
        {
            if (_gameScene.HasUsedDevelopmentCardThisTurn)
            {
                return false;
            }

            return TryExecuteAiYearOfPlenty()
                || TryExecuteAiMonopoly()
                || TryStartAiKnight();
        }

        private bool TryExecuteAiYearOfPlenty()
        {
            if (Player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty) <= 0)
            {
                return false;
            }

            IReadOnlyList<ResourceId> resources = _gameScene.AiStrategy.ChooseYearOfPlentyResources(
                Player,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost,
                BuildingGameState.CityCost);

            if (resources.Count < 2)
            {
                return false;
            }

            DevelopmentCardActivationService service = new();
            DevelopmentCardActivationResult result = service.UseYearOfPlenty(
                Player,
                _gameScene.Bank,
                _gameScene.HasUsedDevelopmentCardThisTurn,
                resources[0],
                resources[1]);

            if (!result.Success)
            {
                return false;
            }

            _gameScene.MarkDevelopmentCardUsed();
            _gameScene.Log.Add($"{Player.DisplayName} usou Year of plenty");
            return true;
        }

        private bool TryExecuteAiMonopoly()
        {
            if (Player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly) <= 0)
            {
                return false;
            }

            ResourceId resource = _gameScene.AiStrategy.ChooseMonopolyResource(
                Player,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost,
                BuildingGameState.CityCost);

            DevelopmentCardActivationService service = new();
            DevelopmentCardActivationResult result = service.UseMonopoly(
                Player,
                _gameScene.Players,
                _gameScene.HasUsedDevelopmentCardThisTurn,
                resource);

            if (!result.Success)
            {
                return false;
            }

            _gameScene.MarkDevelopmentCardUsed();
            _gameScene.Log.Add($"{Player.DisplayName} usou Monopoly em {resource}");
            return true;
        }

        private bool TryStartAiKnight()
        {
            if (Player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Knight) <= 0 ||
                !_gameScene.AiStrategy.ShouldUseKnight())
            {
                return false;
            }

            _gameScene.Log.Add($"{Player.DisplayName} usou Knight");
            _aiWaitingForDevelopmentCardAction = true;
            _gameScene.AppendState(new KnightGameState(_gameScene, Player, false));
            return true;
        }

        private bool TryExecuteAiDevelopmentCardPurchase()
        {
            DevelopmentCardPurchaseService service = new();
            if (!service.CanPurchase(Player, _gameScene.Bank, _gameScene.DevelopmentCardDeck).Success ||
                !_gameScene.AiStrategy.ShouldBuyDevelopmentCard())
            {
                return false;
            }

            DevelopmentCardPurchaseResult result = service.Purchase(
                Player,
                _gameScene.Bank,
                _gameScene.DevelopmentCardDeck);

            if (!result.Success)
            {
                return false;
            }

            _gameScene.Log.Add($"{Player.DisplayName} comprou {GetDevelopmentCardLogName(result.Card.Type)}");
            return true;
        }

        private bool HasValidAiSettlementPosition()
        {
            BuildPositionSettlementGameState settlementState = new BuildPositionSettlementGameState(
                _gameScene,
                Player,
                BuildingType.Settlement);

            return _gameScene.Board.Graph.Vertices.Any(settlementState.CanPlaceBuilding);
        }

        private bool HasValidAiCityPosition()
        {
            BuildPositionSettlementGameState cityState = new BuildPositionSettlementGameState(
                _gameScene,
                Player,
                BuildingType.City);

            return _gameScene.Board.Graph.Vertices.Any(cityState.CanPlaceBuilding);
        }

        private int CountAiSettlements()
        {
            return _gameScene.Board.Graph.Vertices.Count(vertex =>
                vertex.HasBuilding &&
                vertex.Building.Owner == Player &&
                vertex.Building.Type == BuildingType.Settlement);
        }

        private bool TryStartAiCityBuild()
        {
            if (!Player.Inventory.Resources.HasEnough(BuildingGameState.CityCost))
            {
                return false;
            }

            BuildPositionSettlementGameState cityState = new BuildPositionSettlementGameState(
                _gameScene,
                Player,
                BuildingType.City,
                BuildingGameState.CityCost);

            if (!_gameScene.Board.Graph.Vertices.Any(cityState.CanPlaceBuilding))
            {
                return false;
            }

            _gameScene.AppendState(cityState);
            return true;
        }

        private bool TryStartAiSettlementBuild()
        {
            if (!Player.Inventory.Resources.HasEnough(BuildingGameState.SettlementCost))
            {
                return false;
            }

            BuildPositionSettlementGameState settlementState = new BuildPositionSettlementGameState(
                _gameScene,
                Player,
                BuildingType.Settlement,
                BuildingGameState.SettlementCost);

            if (!_gameScene.Board.Graph.Vertices.Any(settlementState.CanPlaceBuilding))
            {
                return false;
            }

            _gameScene.AppendState(settlementState);
            return true;
        }

        private bool TryStartAiRoadBuild()
        {
            if (!Player.Inventory.Resources.HasEnough(BuildingGameState.RoadCost))
            {
                return false;
            }

            BuildPositionRoadGameState roadState = new BuildPositionRoadGameState(
                _gameScene,
                Player,
                BuildingGameState.RoadCost);

            var validEdges = _gameScene.Board.Graph.Edges
                .Where(roadState.CanPlaceRoad)
                .ToList();
            var usefulEdges = RandomAiStrategy
                .GetUsefulRoadCandidates(_gameScene.Board, _gameScene.Board.Graph, validEdges)
                .ToList();

            if (usefulEdges.Count == 0)
            {
                return false;
            }

            _gameScene.AppendState(roadState);
            return true;
        }

        private bool TryExecuteAiTradeAction()
        {
            AiTradeNeed tradeNeed = _gameScene.AiStrategy.ChooseTradeNeed(
                _gameScene.Board,
                Player,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost);

            if (tradeNeed == null)
            {
                return false;
            }

            if (tradeNeed.PreferBank && TryExecuteAiBankTrade(tradeNeed))
            {
                return true;
            }

            if (TryExecuteAiPlayerTrade(tradeNeed))
            {
                return true;
            }

            return !tradeNeed.PreferBank && TryExecuteAiBankTrade(tradeNeed);
        }

        private bool TryExecuteAiBankTrade(AiTradeNeed tradeNeed)
        {
            HarborService harborService = new(_gameScene.Board);
            Dictionary<ResourceId, int> tradeRates = GetAiBankTradeRates(harborService);

            ResourceId? offeredResource = _gameScene.AiStrategy.ChooseBankTradeOfferedResource(
                Player,
                tradeNeed.Resource,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost,
                tradeRates);

            if (offeredResource is not ResourceId paidResource)
            {
                return false;
            }

            int tradeRate = tradeRates[paidResource];
            if (!_gameScene.Bank.CanTrade(
                Player.Inventory.Resources,
                paidResource,
                tradeRate,
                tradeNeed.Resource,
                1))
            {
                return false;
            }

            _gameScene.Bank.Trade(
                Player.Inventory.Resources,
                paidResource,
                tradeRate,
                tradeNeed.Resource,
                1);

            _gameScene.Log.Add($"{Player.DisplayName} trocou com banco/porto {tradeRate}:1");
            return true;
        }

        private Dictionary<ResourceId, int> GetAiBankTradeRates(HarborService harborService)
        {
            Dictionary<ResourceId, int> tradeRates = new();

            foreach (ResourceId resource in ResourceUtils.ResourceIds)
            {
                tradeRates[resource] = harborService.GetBestTradeRate(Player, resource);
            }

            return tradeRates;
        }

        private bool TryExecuteAiPlayerTrade(AiTradeNeed tradeNeed)
        {
            ResourceId? offeredResource = _gameScene.AiStrategy.ChooseTradeOfferedResource(
                Player,
                tradeNeed.Resource,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost,
                tradeNeed.PlayerOfferAmount);

            if (offeredResource is not ResourceId paidResource)
            {
                return false;
            }

            Dictionary<ResourceId, int> offeredResources = new()
            {
                { paidResource, tradeNeed.PlayerOfferAmount },
            };
            Dictionary<ResourceId, int> requestedResources = new()
            {
                { tradeNeed.Resource, 1 },
            };

            PlayerTradeService service = new();
            PlayerTradeResult createResult = service.CreateOffer(
                Player,
                offeredResources,
                requestedResources,
                out PlayerTradeOffer offer);

            if (!createResult.Success)
            {
                return false;
            }

            foreach (Player acceptingPlayer in _gameScene.Players.Where(player => player != Player && player.IsAi))
            {
                PlayerTradeResult acceptResult = service.CanAccept(offer, acceptingPlayer);
                if (!acceptResult.Success || !_gameScene.AiStrategy.ShouldAcceptTrade())
                {
                    continue;
                }

                PlayerTradeResult executeResult = service.Execute(offer, acceptingPlayer);
                if (!executeResult.Success)
                {
                    continue;
                }

                _gameScene.Log.Add($"{acceptingPlayer.DisplayName} aceitou troca da {Player.DisplayName}");
                return true;
            }

            return false;
        }

        private static string GetDevelopmentCardLogName(DevelopmentCardType type)
        {
            return type switch
            {
                DevelopmentCardType.Knight => "Knight",
                DevelopmentCardType.RoadBuilding => "Road building",
                DevelopmentCardType.YearOfPlenty => "Year of plenty",
                DevelopmentCardType.Monopoly => "Monopoly",
                DevelopmentCardType.VictoryPoint => "Victory point",
                _ => type.ToString(),
            };
        }

        public void OnPlayerBuildButtonClicked()
        {
            if (_gameScene.GetCurrentState() is BuildingGameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new BuildingGameState(_gameScene, Player));
            }
        }

        public void OnPlayerTradeButtonClicked()
        {
            if (_gameScene.GetCurrentState() is TradingGameState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new TradingGameState(_gameScene, Player));
            }
        }

        public void OnPlayerDevelopmentCardButtonClicked()
        {
            if (_gameScene.GetCurrentState() is DevelopmentCardState)
            {
                _gameScene.ExitState();
            }
            else
            {
                _gameScene.AppendState(new DevelopmentCardState(_gameScene, Player));
            }
        }

        public void OnPlayerEndTurnButtonClicked()
        {
            Player.Inventory.DevelopmentCards.ReleaseNewCards();
            _gameScene.ResetDevelopmentCardUsageForTurn();
            _gameScene.ExitState();
        }

    }
}
