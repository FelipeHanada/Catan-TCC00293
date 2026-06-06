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

        public bool TryPlaceRoad(TileEdge edge)
        {
            if (!CanPlaceRoad(edge))
            {
                return false;
            }

            edge.PlaceRoad(Player);
            OnPlaceRoad(edge);
            SoundManager.Instance.Play(SfxId.ConstrucaoEstrada);
            return true;
        }

        public virtual void OnPlaceRoad(TileEdge edge)
        {
            _gameScene.Log.Add($"{Player.DisplayName} construiu estrada");
            _gameScene.ExitState();
        }
    }

    public class SetupPositionRoadGameState : PositionRoadGameState
    {
        private const double AiActionDelaySeconds = 0.35;
        private double _aiElapsedSeconds;
        private bool _aiActed;
        public TileVertex Vertex { get; private set; }
        public SetupPositionRoadGameState(GameScene gameScene, Player player, TileVertex vertex)
            : base(gameScene, player)
        {
            Vertex = vertex;
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

            List<TileEdge> validEdges = _gameScene.Board.Graph.Edges
                .Where(CanPlaceRoad)
                .ToList();

            if (validEdges.Count == 0)
            {
                _gameScene.ExitState();
                return;
            }

            TileEdge chosenEdge = _gameScene.AiStrategy.ChooseRoad(validEdges);
            TryPlaceRoad(chosenEdge);
        }

        public override bool CanPlaceRoad(TileEdge edge)
        {
            if (!base.CanPlaceRoad(edge)) return false;
            
            if (edge.VertexA == Vertex) return true;
            if (edge.VertexB == Vertex) return true;

            return false;
        }
    }

    public class BuildPositionRoadGameState : PositionRoadGameState
    {
        public IReadOnlyDictionary<ResourceId, int> RoadCost { get; private set; }

        public BuildPositionRoadGameState(GameScene gameScene, Player player, IReadOnlyDictionary<ResourceId, int> roadCost = null)
            : base(gameScene, player)
        {
            RoadCost = roadCost ?? new Dictionary<ResourceId, int>();

            AddChild(new ButtonAction(10, 10, gameScene.Atlas, () => { gameScene.ExitState(); }, "Cancelar"));
        }

        public override bool CanPlaceRoad(TileEdge edge)
        {
            // Check if player has enough resources for the road cost
            if (!Player.Inventory.Resources.HasEnough(RoadCost))
            {
                return false;
            }

            return base.CanPlaceRoad(edge);
        }

        public override void OnPlaceRoad(TileEdge edge)
        {
            // Send road cost resources to the bank
            if (RoadCost.Count > 0)
            {
                _gameScene.Bank.Receive(Player.Inventory.Resources, RoadCost);
            }

            base.OnPlaceRoad(edge);
        }
    }
}
