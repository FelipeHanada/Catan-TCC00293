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
        private Tile _robberTile;
        public List<Tile> Tiles { get; set; }
        public List<TileVertex> Vertices { get; set; }
        public List<TileEdge> Edges { get; set; }
        public Tile RobberTile => _robberTile;

        public Board(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
            Tiles = [];
            Vertices = [];
            Edges = [];
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

            scene.Subscribe(new RobberMarker(this));
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
}
