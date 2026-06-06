using Catan.Source.Game;
using Catan.Source.Game.AI;
using Catan.Source.Game.Board;
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
        private const int AiBankTradeRate = 4;
        private double _aiElapsedSeconds;
        private bool _aiActed;
        private bool _aiWaitingForBuildAction;

        public PlayerActionsGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _aiElapsedSeconds = 0;
            _aiActed = false;
            _aiWaitingForBuildAction = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Player.IsAi || _aiActed)
            {
                return;
            }

            if (_aiWaitingForBuildAction)
            {
                _aiActed = true;
                OnPlayerEndTurnButtonClicked();
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

            if (TryExecuteAiTradeAction())
            {
                _aiActed = true;
                OnPlayerEndTurnButtonClicked();
                return;
            }

            _aiActed = true;
            OnPlayerEndTurnButtonClicked();
        }

        private bool TryStartAiBuildAction()
        {
            bool hasSettlementPosition = HasValidAiSettlementPosition();
            bool canAffordSettlement = Player.Inventory.Resources.HasEnough(BuildingGameState.SettlementCost);

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

        private bool HasValidAiSettlementPosition()
        {
            BuildPositionSettlementGameState settlementState = new BuildPositionSettlementGameState(
                _gameScene,
                Player,
                BuildingType.Settlement);

            return _gameScene.Board.Graph.Vertices.Any(settlementState.CanPlaceBuilding);
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
            ResourceId? offeredResource = _gameScene.AiStrategy.ChooseTradeOfferedResource(
                Player,
                tradeNeed.Resource,
                BuildingGameState.SettlementCost,
                BuildingGameState.RoadCost,
                AiBankTradeRate);

            if (offeredResource is not ResourceId paidResource)
            {
                return false;
            }

            if (!_gameScene.Bank.CanTrade(
                Player.Inventory.Resources,
                paidResource,
                AiBankTradeRate,
                tradeNeed.Resource,
                1))
            {
                return false;
            }

            _gameScene.Bank.Trade(
                Player.Inventory.Resources,
                paidResource,
                AiBankTradeRate,
                tradeNeed.Resource,
                1);

            _gameScene.Log.Add($"{Player.DisplayName} trocou com o banco");
            return true;
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
