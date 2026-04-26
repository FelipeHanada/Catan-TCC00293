using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using System.Collections.Generic;

namespace Catan.Source.Game.Board
{
    public class Board : GameObject
    {
        private readonly Atlas atlas;

        private List<Tile> _tiles;

        public Board(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
            _tiles = new()
            {
                new Tile(0, 0, atlas, TileType.Forest, 1),
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Tile tile in _tiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }
        }
        public override void Update(GameTime gameTime) { }
    }
}
