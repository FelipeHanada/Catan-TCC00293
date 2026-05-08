using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Game.Board
{
    public class TileEdge : GameObject
    {
        public TileVertex VertexA { get; }
        public TileVertex VertexB { get; }

        public TileEdge(float x, float y, TileVertex vertexA, TileVertex vertexB)
            : base(x, y)
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
