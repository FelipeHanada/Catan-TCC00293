using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionSettlementGameState : GameState
    {
        private GameScene gameScene;
        public Player Player { get; private set; }
        public BuildingType BuildingType { get; private set; }
        public PositionSettlementGameState(GameScene gameScene, Player player, BuildingType buildingType)
            : base(gameScene)
        {
            this.gameScene = gameScene;
            Player = player;
            BuildingType = buildingType;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool CanPlaceBuilding(TileVertex tileVertex)
        {
            BoardGraph graph = gameScene.Board.Graph;

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
    }
}
