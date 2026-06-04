using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class PositionRoadGameState : PlayerTurnGameState
    {
        public PositionRoadGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual bool CanPlaceRoad(TileEdge edge)
        {
            if (edge.RoadOwner != null) return false;

            if (edge.VertexA.HasBuilding && edge.VertexA.Building.Owner == Player) return true;
            if (edge.VertexB.HasBuilding && edge.VertexB.Building.Owner == Player) return true;

            BoardGraph graph = _gameScene.Board.Graph;
            foreach (TileEdge e in graph.Incident[edge.VertexA])
            {
                if (e.RoadOwner == Player) return true;
            }
            foreach (TileEdge e in graph.Incident[edge.VertexB])
            {
                if (e.RoadOwner == Player) return true;
            }

            return false;
        }

        public virtual void OnPlaceRoad(TileEdge edge)
        {
            _gameScene.ExitState();
        }
    }

    public class SetupPositionRoadGameState : PositionRoadGameState
    {
        public TileVertex Vertex { get; private set; }
        public SetupPositionRoadGameState(GameScene gameScene, Player player, TileVertex vertex)
            : base(gameScene, player)
        {
            Vertex = vertex;
        }

        public override bool CanPlaceRoad(TileEdge edge)
        {
            if (!base.CanPlaceRoad(edge)) return false;
            
            if (edge.VertexA == Vertex) return true;
            if (edge.VertexB == Vertex) return true;

            return false;
        }
    }
}
