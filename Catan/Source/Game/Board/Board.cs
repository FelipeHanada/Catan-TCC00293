using System;
using System.Collections.Generic;
using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;
using HarborModel = Catan.Source.Game.Harbor.Harbor;
using Catan.Source.Content;

namespace Catan.Source.Game.Board
{
    public class BoardGraph : GameObject
    {
        public HashSet<TileVertex> Vertices { get; private set; }
        public HashSet<TileEdge> Edges { get; private set; }
        public Dictionary<TileVertex, HashSet<TileEdge>> Incident { get; private set; }

        public BoardGraph()
        {
            Vertices = [];
            Edges = [];
            Incident = [];
        }

        public override void OnSubscribe(Scene scene)
        {
            base.OnSubscribe(scene);

            foreach (TileEdge edge in Edges)
            {
                scene.Subscribe(edge);
            }

            foreach (TileVertex vertex in Vertices)
            {
                scene.Subscribe(vertex);
            }
        }

        public void AddVertex(TileVertex vertex)
        {
            ArgumentNullException.ThrowIfNull(vertex);

            if (Vertices.Add(vertex))
            {
                Incident[vertex] = [];
            }
        }

        public void AddEdge(TileEdge edge)
        {
            ArgumentNullException.ThrowIfNull(edge);

            if (!Vertices.Contains(edge.VertexA) || !Vertices.Contains(edge.VertexB))
            {
                throw new InvalidOperationException("Both vertices of the edge must already belong to the graph.");
            }

            if (Edges.Add(edge))
            {
                Incident[edge.VertexA].Add(edge);
                Incident[edge.VertexB].Add(edge);
            }
        }
    }

    public class Board : GameObject
    {
        private readonly Atlas _atlas;
        private Tile _robberTile;

        public List<Tile> Tiles { get; private set; }
        public BoardGraph Graph { get; private set; }
        public List<HarborModel> Harbors { get; set; }
        public Tile RobberTile => _robberTile;

        public Board(float x, float y, Atlas atlas, List<Tile> tiles, List<HarborModel> harbors, BoardGraph graph)
            : base(x, y)
        {
            _atlas = atlas;
            Tiles = tiles;
            Graph = graph;
            Harbors = harbors;
            InitializeRobber();
        }

        public void InitializeRobber()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.TileType == TileType.Desert)
                {
                    _robberTile = tile;
                    return;
                }
            }

            if (Tiles.Count > 0)
            {
                _robberTile = Tiles[0];
            }
        }

        public bool IsTileBlockedByRobber(Tile tile)
        {
            return ReferenceEquals(_robberTile, tile);
        }

        public bool CanMoveRobberTo(Tile tile)
        {
            return Tiles.Contains(tile) && !IsTileBlockedByRobber(tile);
        }

        public bool MoveRobberTo(Tile tile)
        {
            if (!CanMoveRobberTo(tile))
            {
                return false;
            }

            _robberTile = tile;
            return true;
        }

        public override void OnSubscribe(Scene scene)
        {
            base.OnSubscribe(scene);

            foreach (Tile tile in Tiles)
            {
                scene.Subscribe(tile);
            }

            scene.Subscribe(Graph);
            scene.Subscribe(new RobberMarker(this, _atlas));
        }

        public IEnumerable<Tile> GetProducingTiles(int diceNumber)
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.DiceNumber == diceNumber &&
                    tile.ProducedResource != null &&
                    !IsTileBlockedByRobber(tile))
                {
                    yield return tile;
                }
            }
        }

        public override void Update(GameTime gameTime) { }
    }

    public class BoardBackground : GameObject
    {
        private Atlas atlas;

        public BoardBackground(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
        }

        public override void OnSubscribe(Scene scene)
        {
            base.OnSubscribe(scene);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.Background),
                Color.White, 0.0f,
                new Vector2(
                    Atlas.GetRectangle(AtlasSpriteId.BackgroundOrigin).X,
                    Atlas.GetRectangle(AtlasSpriteId.BackgroundOrigin).Y
                    ),
                1.0f, SpriteEffects.None, 0.0f
            );
        }
        public override void Update(GameTime gameTime) { }
    }
}
