using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using Catan.Source.Scenes;
using System.Diagnostics.Contracts;


namespace Catan.Source.Game.Board
{
    // public interface IBoardState
    // {
    //     public void EnterState();
    //     public void ExitState();
    //     public void Update(GameTime gameTime);
    //     public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    // }

    // public abstract class BoardState : IBoardState
    // {
    //     protected Board _board;
    //     public BoardState(Board board) 
    //     {
    //         _board = board;
    //     }
    //     public virtual void EnterState() {}
    //     public virtual void ExitState() {}
    //     public virtual void Update(GameTime gameTime) {}
    //     public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {}
    // }

    // public class PlaceBuildingBoardState : BoardState
    // {
    //     private Player.Player _player;
    //     private BuildingType _buildingType;
    //     public PlaceBuildingBoardState(Board board)
    //         : base(board) {}
    //     public override void EnterState()
    //     {
    //         base.EnterState();

    //     }
    // }
    
    public class Board : GameObject
    {
        public List<Tile> Tiles { get; set; }
        public List<TileVertex> Vertices { get; set; }
        public List<TileEdge> Edges { get; set; }

        public Board(float x, float y)
            : base(x, y)
        {
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
