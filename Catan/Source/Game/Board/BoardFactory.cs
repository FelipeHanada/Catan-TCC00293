using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Catan.Source.Content;
using System.Linq;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Game.Board
{
    public interface IBoardFactory
    {
        Board CreateBoard(GameScene gameScene);
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

        public Board CreateBoard(GameScene gameScene)
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
                    tiles.Add(new Tile(tileX, tileY, atlas, randomType, diceNum, gameScene));
                }
            }

            StandardTilePositionIterator positionIterator = new(startX, startY, atlas, gameScene);
            BoardGraph graph = new();
            foreach (TileVertex vertex in positionIterator.Vertices)
            {
                graph.AddVertex(vertex);
            }
            foreach (TileEdge edge in positionIterator.Edges)
            {
                graph.AddEdge(edge);
            }

            Board board = new Board(startX, startY, tiles, graph);
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

        public Board CreateBoard(GameScene gameScene)
        {
            List<int> diceNumbers = [2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12];
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(diceNumbers));

            List<TileType> resources = [
                TileType.Forest, TileType.Forest, TileType.Forest, TileType.Forest,
                TileType.Sheep, TileType.Sheep, TileType.Sheep, TileType.Sheep,
                TileType.Farm, TileType.Farm, TileType.Farm, TileType.Farm,
                TileType.Brick, TileType.Brick, TileType.Brick,
                TileType.Mountain, TileType.Mountain, TileType.Mountain,
            ];
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(resources));

            List<KeyValuePair<TileType, int>> tilesConfig = [ new(TileType.Desert, 7) ];
            for (int i=0; i<18; i++) tilesConfig.Add(new(resources[i], diceNumbers[i]));
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(tilesConfig));

            List<Tile> tiles = [];
            int tile_idx = 0;
            StandardTilePositionIterator positionIterator = new(startX, startY, atlas, gameScene);
            foreach (Tuple<Vector2, TileVertex[]> info in positionIterator)
            {
                Vector2 position = info.Item1;
                TileVertex[] vertices = info.Item2;
                tiles.Add(new(
                    position.X, position.Y, atlas,
                    tilesConfig[tile_idx].Key, tilesConfig[tile_idx].Value,
                    vertices, gameScene
                ));
                tile_idx++;
            }

            BoardGraph graph = new();
            foreach (TileVertex vertex in positionIterator.Vertices)
            {
                graph.AddVertex(vertex);
            }
            foreach (TileEdge edge in positionIterator.Edges)
            {
                graph.AddEdge(edge);
            }

            Board board = new(startX, startY, tiles, graph);
            return board;
        }
    }

    public class StandardTilePositionIterator : IEnumerable<Tuple<Vector2, TileVertex[]>>
    {
        private readonly float startX;
        private readonly float startY;
        private readonly float width;
        private readonly float height;
        private readonly float h;
        public List<TileVertex> Vertices { get; set; }
        public List<TileEdge> Edges { get; set; }

        private List<List<TileVertex>> vertexTableA;
        private List<List<TileVertex>> vertexTableB;

        public StandardTilePositionIterator(float startX, float startY, Atlas atlas, GameScene gameScene, float width = 128, float height = 128, float h = 32)
        {
            this.startX = startX;
            this.startY = startY;

            this.width = width;
            this.height = height;
            this.h = h;

            Vertices = [];
            Edges = [];

            vertexTableA = [];
            for (int i=0; i<6; i++)
            {
                List<TileVertex> row = [];
                for (int j=0; j<6 - Math.Abs(3 - i); j++)
                {
                    row.Add(new(
                        Math.Abs(3 - i) * width/2 + width * j,
                        (height - h) * i,
                        atlas,
                        gameScene
                    ));
                }
                vertexTableA.Add(row);
                Vertices.AddRange(row);
            }
            vertexTableB = [];
            for (int i=0; i<6; i++)
            {
                List<TileVertex> row = [];
                for (int j=0; j<6 - Math.Abs(2 - i); j++)
                {
                    row.Add(new(
                        Math.Abs(2 - i) * width/2 + width * j,
                        (height - h) * i + h,
                        atlas,
                        gameScene
                    ));
                }
                vertexTableB.Add(row);
                Vertices.AddRange(row);
            }

            for (int i=0; i<vertexTableB.Count-1; i++)
            {
                for (int j=0; j<vertexTableB[i].Count; j++)
                {
                    Edges.Add(new(vertexTableA[i+1][j], vertexTableB[i][j], gameScene));
                }
            }

            for (int i=0; i<vertexTableA.Count; i++)
            {
                if (vertexTableA[i].Count < vertexTableB[i].Count)
                {
                    for (int j=0; j<vertexTableA[i].Count; j++)
                    {
                        Edges.Add(new(vertexTableA[i][j], vertexTableB[i][j], gameScene));
                        Edges.Add(new(vertexTableA[i][j], vertexTableB[i][j+1], gameScene));
                    }
                } else
                {
                    for (int j=0; j<vertexTableB[i].Count; j++)
                    {
                        Edges.Add(new(vertexTableA[i][j], vertexTableB[i][j], gameScene));
                        Edges.Add(new(vertexTableA[i][j+1], vertexTableB[i][j], gameScene));
                    }
                }
            }
        }

        public IEnumerator<Tuple<Vector2, TileVertex[]>> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<Tuple<Vector2, TileVertex[]>> Enumerate()
        {
            for (int i = 0; i < 5; i++)
            {
                int rowCount = 3 + 2 - Math.Abs(i - 2);
                for (int j = 0; j < rowCount; j++)
                {
                    float x = startX + width/2 * Math.Abs(i - 2) + width * j;
                    float y = startY + (height - h) * i;
                    yield return new(new Vector2(x, y), GetTileVertices(i, j));
                }
            }
        }

        private TileVertex[] GetTileVertices(int row, int column)
        {
            int topOffset = row > 2 ? 1 : 0;
            int bottomOffset = row < 2 ? 1 : 0;

            return [
                vertexTableA[row][column + topOffset],
                vertexTableB[row][column],
                vertexTableB[row][column + 1],
                vertexTableA[row + 1][column],
                vertexTableA[row + 1][column + 1],
                vertexTableB[row + 1][column + bottomOffset],
            ];
        }
    }
}
