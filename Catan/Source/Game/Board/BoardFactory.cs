using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Catan.Source.Content;
using Catan.Source.Game.Resources;
using HarborModel = Catan.Source.Game.Harbor.Harbor;
using Catan.Source.Scenes;

namespace Catan.Source.Game.Board
{
    public interface IBoardFactory
    {
        Board CreateBoard(GameScene gameScene);
    }

    public class RandomBoardFactory : IBoardFactory
    {
        private readonly Atlas _atlas;
        private readonly float _startX;
        private readonly float _startY;

        public RandomBoardFactory(Atlas atlas, float startX, float startY)
        {
            _atlas = atlas;
            _startX = startX;
            _startY = startY;
        }

        public Board CreateBoard(GameScene gameScene)
        {
            Random rand = new Random();
            List<Tile> tiles = [];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3 + 2 - Math.Abs(i - 2); j++)
                {
                    float tileX = _startX + 64 * Math.Abs(i - 2) + 128 * j;
                    float tileY = _startY + 96 * i;
                    TileType randomType = (TileType)rand.Next(0, 6);
                    int diceNum = randomType == TileType.Desert ? 7 : rand.Next(2, 13);
                    tiles.Add(new Tile(tileX, tileY, _atlas, randomType, diceNum, gameScene));
                }
            }

            StandardTilePositionIterator positionIterator = new(_startX, _startY, _atlas, gameScene);
            BoardGraph graph = CreateGraph(positionIterator);
            Board board = new(_startX, _startY, _atlas, tiles, positionIterator.CreateHarbors(), graph);
            return board;
        }

        private static BoardGraph CreateGraph(StandardTilePositionIterator positionIterator)
        {
            BoardGraph graph = new();
            foreach (TileVertex vertex in positionIterator.Vertices)
            {
                graph.AddVertex(vertex);
            }

            foreach (TileEdge edge in positionIterator.Edges)
            {
                graph.AddEdge(edge);
            }

            return graph;
        }
    }

    public class StandardRandomBoardFactory : IBoardFactory
    {
        private readonly Atlas _atlas;
        private readonly float _startX;
        private readonly float _startY;

        public StandardRandomBoardFactory(Atlas atlas, float startX, float startY)
        {
            _atlas = atlas;
            _startX = startX;
            _startY = startY;
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

            List<KeyValuePair<TileType, int>> tilesConfig = [new(TileType.Desert, 7)];
            for (int i = 0; i < 18; i++)
            {
                tilesConfig.Add(new(resources[i], diceNumbers[i]));
            }
            Random.Shared.Shuffle(CollectionsMarshal.AsSpan(tilesConfig));

            List<Tile> tiles = [];
            int tileIndex = 0;
            StandardTilePositionIterator positionIterator = new(_startX, _startY, _atlas, gameScene);
            foreach (Tuple<Vector2, TileVertex[]> info in positionIterator)
            {
                Vector2 tilePosition = info.Item1;
                TileVertex[] vertices = info.Item2;
                tiles.Add(new(
                    tilePosition.X,
                    tilePosition.Y,
                    _atlas,
                    tilesConfig[tileIndex].Key,
                    tilesConfig[tileIndex].Value,
                    vertices,
                    gameScene
                ));
                tileIndex++;
            }

            BoardGraph graph = CreateGraph(positionIterator);
            Board board = new(_startX, _startY, _atlas, tiles, positionIterator.CreateHarbors(), graph);
            return board;
        }

        private static BoardGraph CreateGraph(StandardTilePositionIterator positionIterator)
        {
            BoardGraph graph = new();
            foreach (TileVertex vertex in positionIterator.Vertices)
            {
                graph.AddVertex(vertex);
            }

            foreach (TileEdge edge in positionIterator.Edges)
            {
                graph.AddEdge(edge);
            }

            return graph;
        }
    }

    public class StandardTilePositionIterator : IEnumerable<Tuple<Vector2, TileVertex[]>>
    {
        private readonly float _startX;
        private readonly float _startY;
        private readonly float _width;
        private readonly float _height;
        private readonly float _h;
        public List<TileVertex> Vertices { get; set; }
        public List<TileEdge> Edges { get; set; }

        private List<List<TileVertex>> _vertexTableA;
        private List<List<TileVertex>> _vertexTableB;

        public StandardTilePositionIterator(float startX, float startY, Atlas atlas, GameScene gameScene, float width = 128, float height = 128, float h = 32)
        {
            _startX = startX;
            _startY = startY;

            _width = width;
            _height = height;
            _h = h;

            Vertices = [];
            Edges = [];

            _vertexTableA = [];
            for (int i = 0; i < 6; i++)
            {
                List<TileVertex> row = [];
                for (int j = 0; j < 6 - Math.Abs(3 - i); j++)
                {
                    row.Add(new(
                        startX + Math.Abs(3 - i) * _width / 2 + _width * j,
                        startY + (_height - _h) * i,
                        atlas,
                        gameScene
                    ));
                }
                _vertexTableA.Add(row);
                Vertices.AddRange(row);
            }
            _vertexTableB = [];
            for (int i = 0; i < 6; i++)
            {
                List<TileVertex> row = [];
                for (int j = 0; j < 6 - Math.Abs(2 - i); j++)
                {
                    row.Add(new(
                        startX + Math.Abs(2 - i) * _width / 2 + _width * j,
                        startY + (_height - _h) * i + _h,
                        atlas,
                        gameScene
                    ));
                }
                _vertexTableB.Add(row);
                Vertices.AddRange(row);
            }

            for (int i = 0; i < _vertexTableB.Count - 1; i++)
            {
                for (int j = 0; j < _vertexTableB[i].Count; j++)
                {
                    Edges.Add(new(_vertexTableA[i + 1][j], _vertexTableB[i][j], atlas, gameScene));
                }
            }

            for (int i = 0; i < _vertexTableA.Count; i++)
            {
                if (_vertexTableA[i].Count < _vertexTableB[i].Count)
                {
                    for (int j = 0; j < _vertexTableA[i].Count; j++)
                    {
                        Edges.Add(new(_vertexTableA[i][j], _vertexTableB[i][j], atlas, gameScene));
                        Edges.Add(new(_vertexTableA[i][j], _vertexTableB[i][j + 1], atlas, gameScene));
                    }
                }
                else
                {
                    for (int j = 0; j < _vertexTableB[i].Count; j++)
                    {
                        Edges.Add(new(_vertexTableA[i][j], _vertexTableB[i][j], atlas, gameScene));
                        Edges.Add(new(_vertexTableA[i][j + 1], _vertexTableB[i][j], atlas, gameScene));
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
                    float x = _startX + _width / 2 * Math.Abs(i - 2) + _width * j;
                    float y = _startY + (_height - _h) * i;
                    yield return new(new Vector2(x, y), GetTileVertices(i, j));
                }
            }
        }

        private TileVertex[] GetTileVertices(int row, int column)
        {
            int topOffset = row > 2 ? 1 : 0;
            int bottomOffset = row < 2 ? 1 : 0;

            return [
                _vertexTableA[row][column + topOffset],
                _vertexTableB[row][column],
                _vertexTableB[row][column + 1],
                _vertexTableA[row + 1][column],
                _vertexTableA[row + 1][column + 1],
                _vertexTableB[row + 1][column + bottomOffset],
            ];
        }

        public List<HarborModel> CreateHarbors()
        {
            return [
                HarborModel.CreateGeneric([_vertexTableA[0][0], _vertexTableA[0][1]]),
                HarborModel.CreateSpecific(ResourceId.Ore, [_vertexTableB[0][0], _vertexTableA[1][0]]),
                HarborModel.CreateSpecific(ResourceId.Wood, [_vertexTableA[0][2], _vertexTableB[0][3]]),
                HarborModel.CreateGeneric([_vertexTableB[1][4], _vertexTableA[2][4]]),
                HarborModel.CreateSpecific(ResourceId.Brick, [_vertexTableA[3][5], _vertexTableB[3][4]]),
                HarborModel.CreateGeneric([_vertexTableA[5][3], _vertexTableB[5][2]]),
                HarborModel.CreateSpecific(ResourceId.Wheat, [_vertexTableB[5][0], _vertexTableB[5][1]]),
                HarborModel.CreateSpecific(ResourceId.Wool, [_vertexTableA[5][0], _vertexTableB[4][0]]),
                HarborModel.CreateGeneric([_vertexTableB[2][0], _vertexTableA[3][0]]),
            ];
        }
    }
}
