using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using System.Collections.Generic;

namespace Catan.Source.Game.Board
{
    public enum TileType
    {
        Forest, Sheep, Brick, Mountain, Desert, Farm
    }

    public class Tile : GameObject
    {
        private static readonly Dictionary<TileType, AtlasSpriteId> _tileSpriteId = new()
        {
            [TileType.Forest] = AtlasSpriteId.TileForest,
            [TileType.Sheep] = AtlasSpriteId.TileSheep,
            [TileType.Brick] = AtlasSpriteId.TileBrick,
            [TileType.Mountain] = AtlasSpriteId.TileMountain,
            [TileType.Desert] = AtlasSpriteId.TileDesert,
            [TileType.Farm] = AtlasSpriteId.TileFarm,
        };

        private readonly Atlas atlas;
        private readonly TileType tileType;
        private readonly int diceNumber;

        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber)
            : base(x, y)
        {
            this.atlas = atlas;
            this.tileType = tileType;
            this.diceNumber = diceNumber;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(_tileSpriteId[this.tileType]),
                Color.White);
        }
        public override void Update(GameTime gameTime) { }
    }
}


