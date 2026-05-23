using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using System.Collections.Generic;
using Catan.Source.Game.Resources;

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

        private static readonly Dictionary<TileType, ResourceId> _tileResourceId = new()
        {
            [TileType.Forest] = ResourceId.Wood,
            [TileType.Sheep] = ResourceId.Wool,
            [TileType.Brick] = ResourceId.Brick,
            [TileType.Mountain] = ResourceId.Ore,
            [TileType.Farm] = ResourceId.Wheat,
        };

        public static readonly Dictionary<TileType, Vector2> _tileDiceNumberOffset = new()
        {
            [TileType.Forest] = new(48, 73),
            [TileType.Sheep] = new(48, 48),
            [TileType.Brick] = new(48, 48),
            [TileType.Mountain] = new(48, 73),
            [TileType.Desert] = new(48, 48),
            [TileType.Farm] = new(48, 48),
        };

        private readonly Atlas atlas;
        private readonly TileType tileType;
        private readonly int diceNumber;
        private readonly TileVertex[] vertices;

        public TileType TileType => tileType;
        public int DiceNumber => diceNumber;
        public IReadOnlyList<TileVertex> Vertices => vertices;
        public ResourceId? ProducedResource =>
            _tileResourceId.TryGetValue(tileType, out ResourceId resource) ? resource : null;

        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber, TileVertex[] vertices)
            : base(x, y)
        {
            this.atlas = atlas;
            this.tileType = tileType;
            this.diceNumber = diceNumber;
            this.vertices = vertices;
        }
        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber)
            : this(x, y, atlas, tileType, diceNumber, []) {}

        public IEnumerable<Building> GetAdjacentBuildings()
        {
            foreach (TileVertex vertex in vertices)
            {
                if (vertex.HasBuilding)
                {
                    yield return vertex.Building;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(_tileSpriteId[this.tileType]),
                Color.White);

            _tileDiceNumberOffset.TryGetValue(this.tileType, out Vector2 offset);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y) + offset,
                Atlas.GetRectangle(Atlas.GetTileDiceNumberSprite(this.diceNumber)),
                Color.White);
        }
        public override void Update(GameTime gameTime) { }
    }
}


