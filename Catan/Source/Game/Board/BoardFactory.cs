using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Catan.Source.Content;

namespace Catan.Source.Game.Board
{
    public interface IBoardFactory
    {
        Board CreateBoard();
    }

    public class RandomBoardFactory : IBoardFactory
    {
        private readonly Atlas atlas;
        private readonly float startX;
        private readonly float startY;

        public RandomBoardFactory(Atlas atlas, float startX, float startY)
        {
            this.atlas = atlas;
            this.startX = startX;
            this.startY = startY;
        }

        public Board CreateBoard()
        {
            Random rand = new Random();
            List<Tile> tiles = [];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3 + 2 - Math.Abs(i - 2); j++)
                {
                    float tileX = startX + 64 * Math.Abs(i - 2) + 128 * j;
                    float tileY = startY + 96 * i;
                    TileType randomType = (TileType)rand.Next(0, 6);
                    int diceNum = randomType == TileType.Desert ? 7 : rand.Next(2, 13);
                    tiles.Add(new Tile(tileX, tileY, atlas, randomType, diceNum));
                }
            }

            Board board = new Board(startX, startY, atlas)
            {
                Tiles = tiles
            };
            return board;
        }
    }

    public class StandardRandomBoardFactory : IBoardFactory
    {
        private readonly Atlas atlas;
        private readonly float startX;
        private readonly float startY;

        public StandardRandomBoardFactory(Atlas atlas, float startX, float startY)
        {
            this.atlas = atlas;
            this.startX = startX;
            this.startY = startY;
        }

        public Board CreateBoard()
        {
            List<int> diceNumbers = [2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12];
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(diceNumbers));

            List<TileType> resources = [ TileType.Forest, TileType.Sheep, TileType.Brick, TileType.Mountain, TileType.Farm ];
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(resources));
            for (int i=0; i<3; i++) for(int j=0; j<3; j++) resources.Add(resources[i]);
            for (int i=3; i<5; i++) for(int j=0; j<2; j++) resources.Add(resources[i]);
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(resources));

            List<KeyValuePair<TileType, int>> tilesConfig = [ new(TileType.Desert, 7) ];
            for (int i=0; i<18; i++) tilesConfig.Add(new(resources[i], diceNumbers[i]));
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(tilesConfig));

            List<Tile> tiles = [];
            int tile_idx = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3 + 2 - Math.Abs(i - 2); j++)
                {
                    float tileX = startX + 64 * Math.Abs(i - 2) + 128 * j;
                    float tileY = startY + 96 * i;
                    tiles.Add(new Tile(tileX, tileY, atlas, tilesConfig[tile_idx].Key, tilesConfig[tile_idx].Value));
                    tile_idx++;
                }
            }

            Board board = new(startX, startY, atlas)
            {
                Tiles = tiles
            };
            return board;
        }
    }
}
