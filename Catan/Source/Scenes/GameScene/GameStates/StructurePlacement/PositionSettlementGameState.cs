using Catan.Source.Game;
using Catan.Source.Game.Board;
using Catan.Source.Content;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Catan.Source.Scenes.Game
{
    public class PositionSettlementGameState : PlayerTurnGameState
    {
        public BuildingType BuildingType { get; private set; }
        public PositionSettlementGameState(GameScene gameScene, Player player, BuildingType buildingType)
            : base(gameScene, player)
        {
            BuildingType = buildingType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual bool CanPlaceBuilding(TileVertex tileVertex)
        {
            BoardGraph graph = _gameScene.Board.Graph;

            foreach (TileEdge edge in graph.Incident[tileVertex])
            {
                TileVertex vertexA = edge.VertexA, vertexB = edge.VertexB;
                if (vertexA != tileVertex && vertexA.HasBuilding) return false;
                if (vertexB != tileVertex && vertexB.HasBuilding) return false;
            }

            if (BuildingType == BuildingType.Settlement)
            {
                return !tileVertex.HasBuilding;
            } else if (BuildingType == BuildingType.City)
            {
                return tileVertex.HasBuilding
                    && tileVertex.Building.Type == BuildingType.Settlement
                    && tileVertex.Building.Owner == Player;
            }

            return false;
        }

        public bool TryPlaceBuilding(TileVertex vertex)
        {
            if (!CanPlaceBuilding(vertex))
            {
                return false;
            }

            vertex.PlaceBuilding(new Building(Player, BuildingType));
            OnPlaceBuilding(vertex);
            SoundManager.Instance.Play(SfxId.ConstrucaoCasa);
            return true;
        }

        public virtual void OnPlaceBuilding(TileVertex vertex)
        {
            _gameScene.ExitState();
        }
    }

    public class SetupPositionSettlementGameState : PositionSettlementGameState
    {
        private const double AiActionDelaySeconds = 0.35;
        private double _aiElapsedSeconds;
        private bool _aiActed;
        public bool Produce { get; } 

        public SetupPositionSettlementGameState(
            GameScene gameScene,
            Player player,
            BuildingType buildingType,
            bool produce = false
        ) : base(gameScene, player, buildingType)
        {
            Produce = produce;
            _aiElapsedSeconds = 0;
            _aiActed = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Player.IsAi || _aiActed)
            {
                return;
            }

            _aiElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (_aiElapsedSeconds < AiActionDelaySeconds)
            {
                return;
            }

            _aiActed = true;

            List<TileVertex> validVertices = _gameScene.Board.Graph.Vertices
                .Where(CanPlaceBuilding)
                .ToList();

            if (validVertices.Count == 0)
            {
                _gameScene.ExitState();
                return;
            }

            TileVertex chosenVertex = _gameScene.AiStrategy.ChooseSetupSettlement(_gameScene.Board, validVertices);
            TryPlaceBuilding(chosenVertex);
        }

        public override void OnPlaceBuilding(TileVertex vertex)
        {
            base.OnPlaceBuilding(vertex);

            if (Produce)
            {
                ProduceInitialResources(vertex);
            }

            _gameScene.AppendState(new SetupPositionRoadGameState(_gameScene, Player, vertex));
        }

        private void ProduceInitialResources(TileVertex vertex)
        {
            foreach (Tile tile in _gameScene.Board.Tiles)
            {
                if (!IsAdjacentTo(tile, vertex) ||
                    tile.ProducedResource is not ResourceId resource)
                {
                    continue;
                }

                _gameScene.Bank.Give(Player.Inventory.Resources, resource, 1);
            }
        }

        private static bool IsAdjacentTo(Tile tile, TileVertex vertex)
        {
            foreach (TileVertex tileVertex in tile.Vertices)
            {
                if (ReferenceEquals(tileVertex, vertex))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class BuildPositionSettlementGameState : PositionSettlementGameState
    {
        public IReadOnlyDictionary<ResourceId, int> BuildingCost { get; private set; }

        public BuildPositionSettlementGameState(GameScene gameScene, Player player, BuildingType buildingType, IReadOnlyDictionary<ResourceId, int> buildingCost = null)
            : base(gameScene, player, buildingType)
        {
            BuildingCost = buildingCost ?? new Dictionary<ResourceId, int>();

            AddChild(new ButtonAction(10, 10, gameScene.Atlas, () => { gameScene.ExitState(); }, "Cancelar"));
        }

        public override bool CanPlaceBuilding(TileVertex tileVertex)
        {
            // Check if player has enough resources for the building cost
            if (!Player.Inventory.Resources.HasEnough(BuildingCost))
            {
                return false;
            }

            if (!base.CanPlaceBuilding(tileVertex)) return false;

            BoardGraph graph = _gameScene.Board.Graph;

            foreach (TileEdge edge in graph.Incident[tileVertex])
            {
                if (edge.RoadOwner == Player) {
                    return true;
                }
            }

            return false;
        }

        public override void OnPlaceBuilding(TileVertex vertex)
        {
            // Send building cost resources to the bank
            if (BuildingCost.Count > 0)
            {
                _gameScene.Bank.Receive(Player.Inventory.Resources, BuildingCost);
            }            

            base.OnPlaceBuilding(vertex);
        }
    }
}
