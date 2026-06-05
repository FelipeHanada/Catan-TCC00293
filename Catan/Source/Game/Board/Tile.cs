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

        private static readonly Dictionary<TileType, AtlasSpriteId> _tileSpriteIds = new()
        {
            [TileType.Forest] = AtlasSpriteId.TileForest,
            [TileType.Sheep] = AtlasSpriteId.TileSheep,
            [TileType.Brick] = AtlasSpriteId.TileBrick,
            [TileType.Mountain] = AtlasSpriteId.TileMountain,
            [TileType.Desert] = AtlasSpriteId.TileDesert,
            [TileType.Farm] = AtlasSpriteId.TileFarm,
        };

        private static readonly Dictionary<TileType, ResourceId> _tileResourceIds = new()
        {
            [TileType.Forest] = ResourceId.Wood,
            [TileType.Sheep] = ResourceId.Wool,
            [TileType.Brick] = ResourceId.Brick,
            [TileType.Mountain] = ResourceId.Ore,
            [TileType.Farm] = ResourceId.Wheat,
        };

        public static readonly Dictionary<TileType, Vector2> TileDiceNumberOffsets = new()
        {
            [TileType.Forest] = new(48, 73),
            [TileType.Sheep] = new(48, 48),
            [TileType.Brick] = new(48, 48),
            [TileType.Mountain] = new(48, 73),
            [TileType.Desert] = new(48, 48),
            [TileType.Farm] = new(48, 48),
        };

        private readonly Atlas _atlas;
        private readonly TileType _tileType;
        private readonly int _diceNumber;
        private readonly TileVertex[] _vertices;
        private MouseState _previousMouseState;
        public TileType TileType => _tileType;
        public int DiceNumber => _diceNumber;
        public IReadOnlyList<TileVertex> Vertices => _vertices;
        public ResourceId? ProducedResource =>
            _tileResourceIds.TryGetValue(_tileType, out ResourceId resource) ? resource : null;

        private readonly GameScene _gameScene;

        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber, TileVertex[] vertices, GameScene gameScene)
            : base(x, y)
        {
            _atlas = atlas;
            _tileType = tileType;
            _diceNumber = diceNumber;
            _vertices = vertices;
            _gameScene = gameScene;
        }
        public Tile(float x, float y, Atlas atlas, TileType tileType, int diceNumber, GameScene gameScene)
            : this(x, y, atlas, tileType, diceNumber, [], gameScene) {}

        public IEnumerable<Building> GetAdjacentBuildings()
        {
            foreach (TileVertex vertex in _vertices)
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
                _atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(_tileSpriteIds[_tileType]),
                Color.White);

            TileDiceNumberOffsets.TryGetValue(_tileType, out Vector2 offset);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X, this.Y) + offset,
                Atlas.GetRectangle(Atlas.GetTileDiceNumberSprite(_diceNumber)),
                Color.White);
        }
        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (_gameScene.GetCurrentState() is MoveRobberGameState gameState)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released
                    && IsHovering(currentMouseState)
                )
                {
                    if (_gameScene.Board.MoveRobberTo(this))
                    {
                        gameState.OnPlaceRobber(this);
                        _gameScene.ExitState();
                    }
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
