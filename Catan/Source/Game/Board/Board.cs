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

            Random rand = new Random();
            _tiles = [];
            for (int i=0; i<5; i++) {
                for (int j=0; j < 3 + 2 - Math.Abs(i - 2); j++) {
                    float tileX = X + 64*Math.Abs(i - 2) + 128*j;
                    float tileY = Y + 96*i;
                    TileType randomType = (TileType)rand.Next(0, 6);
                    int diceNum = randomType == TileType.Desert ? 7 : rand.Next(2, 13);
                    _tiles.Add(new Tile(tileX, tileY, atlas, randomType, diceNum));
                }
            }
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
