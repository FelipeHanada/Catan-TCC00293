using Catan.Source.Game;
using Catan.Source.Game.AI;
using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Catan.Source.Scenes.Game
{
    public class PlayerActionsGameState : PlayerTurnGameState, PlayerTradeButtonCallback, PlayerBuildButtonCallback, PlayerDevelopmentCardButtonCallback, PlayerEndTurnButtonCallback
    {
        private const double AiActionDelaySeconds = 0.35;
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
