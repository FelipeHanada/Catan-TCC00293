using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionSettlementGameState : GameState
    {
        public Player Player { get; private set; }
        public BuildingType BuildingType { get; private set; }
        public PositionSettlementGameState(GameScene gameScene, Player player, BuildingType buildingType)
            : base(gameScene)
        {
            Player = player;
            BuildingType = buildingType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool CanPlaceBuilding(TileVertex tileVertex)
        {
            BoardGraph graph = _gameScene.Board.Graph;

            foreach (TileEdge edge in graph.Incident[tileVertex])
            {
                TileVertex a = edge.VertexA, b = edge.VertexB;
                if (a != tileVertex && a.HasBuilding) return false;
                if (b != tileVertex && b.HasBuilding) return false;
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

        public virtual void OnPlaceBuilding(TileVertex vertex)
        {
            _gameScene.ExitState();
        }
    }

    public class SetupPositionSettlementGameState : PositionSettlementGameState
    {
        public bool Produce { get; } 

        public SetupPositionSettlementGameState(
            GameScene gameScene,
            Player player,
            BuildingType buildingType,
            bool produce = false
        ) : base(gameScene, player, buildingType)
        {
            Produce = produce;
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
}
