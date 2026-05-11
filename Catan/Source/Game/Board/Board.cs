using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using System.Collections.Generic;
using Catan.Source.Scenes;

namespace Catan.Source.Game.Board
{
    public class Board : GameObject
    {
        private readonly Atlas atlas;
        public List<Tile> Tiles { get; set; }
        public List<TileVertex> Vertices { get; set; }
        public List<TileEdge> Edges { get; set; }

        public Board(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
            Tiles = [];
            Vertices = [];
            Edges = [];
        }

        public override void OnSubscribe(Scene scene)
        {
            base.OnSubscribe(scene);

            foreach (Tile tile in Tiles) {
                scene.Subscribe(tile);
            }

            foreach (TileVertex vertex in Vertices)
            {
                scene.Subscribe(vertex);
            }

            foreach (TileEdge edge in Edges)
            {
                scene.Subscribe(edge);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }
        }
        public override void Update(GameTime gameTime) { }
    }
}
