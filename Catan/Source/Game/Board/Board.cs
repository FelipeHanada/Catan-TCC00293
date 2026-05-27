using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;
using HarborModel = Catan.Source.Game.Harbor.Harbor;

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
                throw new InvalidOperationException("Both vertices of the edge must already belong to the graph.");

            if (Edges.Add(edge))
            {
                Incident[edge.VertexA].Add(edge);
                Incident[edge.VertexB].Add(edge);
            }
        }
    }

    public class Board : GameObject
    {
        public List<Tile> Tiles { get; private set; }
        public BoardGraph Graph { get; private set; }
        public List<HarborModel> Harbors { get; set; }

        public Board(float x, float y, List<Tile> tiles, List<HarborModel> harbors, BoardGraph graph)
            : base(x, y)
        {
            Tiles = tiles;
            Graph = graph;
            Harbors = harbors;
        }

        public override void OnSubscribe(Scene scene)
        {
            base.OnSubscribe(scene);

            foreach (Tile tile in Tiles) {
                scene.Subscribe(tile);
            }

            scene.Subscribe(Graph);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }
        }

        public IEnumerable<Tile> GetProducingTiles(int diceNumber)
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.DiceNumber == diceNumber && tile.ProducedResource != null)
                {
                    yield return tile;
                }
            }
        }

        public override void Update(GameTime gameTime) { }
    }
}
