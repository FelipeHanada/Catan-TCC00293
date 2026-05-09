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
        public List<Tile> Tiles { get; set; }

        public Board(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
            Tiles = [];
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
