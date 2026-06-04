using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using System.Collections.Generic;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;

namespace Catan.Source.Game.Board
{
    public enum TileType
    {
        Forest, Sheep, Brick, Mountain, Desert, Farm
    }

    public class Tile : GameObject
    {
        private const int TileWidth = 128;
        private const int TileHeight = 128;

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
        private MouseState _previousMouseState;
        public TileType TileType => tileType;
        public int DiceNumber => diceNumber;
        public IReadOnlyList<TileVertex> Vertices => vertices;
        public ResourceId? ProducedResource =>
            _tileResourceId.TryGetValue(tileType, out ResourceId resource) ? resource : null;

        private readonly GameScene gameScene;

        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber, TileVertex[] vertices, GameScene gameScene)
            : base(x, y)
        {
            this.atlas = atlas;
            this.tileType = tileType;
            this.diceNumber = diceNumber;
            this.vertices = vertices;
            this.gameScene = gameScene;
        }
        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber, GameScene gameScene)
            : this(x, y, atlas, tileType, diceNumber, [], gameScene) {}

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
        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (gameScene.GetCurrentStateGame() is MoveRobberGameState gameState)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released
                    && IsHovering(currentMouseState)
                )
                {
                    if (gameScene.Board.CanMoveRobberTo(this))
                    {
                        gameScene.Board.MoveRobberTo(this);
                    }

                    gameScene.ExitState();
                }
            }

            _previousMouseState = currentMouseState;
        }

        private bool IsHovering(MouseState mouseState)
        {
            float px = mouseState.X - X;
            float py = mouseState.Y - Y;

            if (px < 0 || py < 0 || px > TileWidth || py > TileHeight)
            {
                return false;
            }

            if (py <= TileHeight / 2)
            {
                float leftBound = 0.5f * (TileHeight / 2 - py);
                float rightBound = TileWidth - leftBound;
                return px >= leftBound && px <= rightBound;
            }
            else
            {
                float bottomY = py - TileHeight / 2;
                float leftBound = 0.5f * bottomY;
                float rightBound = TileWidth - leftBound;
                return px >= leftBound && px <= rightBound;
            }
        }
    }
}
