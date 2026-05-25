using System;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;

namespace Catan.Source.Game.Board
{
    public class TileEdge : GameObject
    {
        public TileVertex VertexA { get; }
        public TileVertex VertexB { get; }

        private Texture2D _lineTexture;

        public TileEdge(TileVertex vertexA, TileVertex vertexB, GameScene gameScene)
            : base(0, 0)
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            EnsureTextureGenerated();

            var position = new Vector2(Math.Min(VertexA.X, VertexB.X), Math.Min(VertexA.Y, VertexB.Y));
            spriteBatch.Draw(_lineTexture, position, null, Color.White, 0f, new(0, 0), 1f, SpriteEffects.None, 0f);
        }

        public override void Update(GameTime gameTime)
        {
        }

        private void EnsureTextureGenerated()
        {
            if (_lineTexture != null) return;

            int width = (int)Math.Max(1, Math.Abs(VertexA.X - VertexB.X));
            int height = (int)Math.Max(1, Math.Abs(VertexA.Y - VertexB.Y));

            _lineTexture = new Texture2D(Game1.GraphicsDeviceInstance, width, height);
            var pixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[y * width + x] = Color.Transparent;
                }
            }

            float y1, y2;
            if (VertexA.X <= VertexB.X) { y1 = VertexA.Y; y2 = VertexB.Y; }
            else { y1 = VertexB.Y; y2 = VertexA.Y; }
            y1 -= Math.Min(VertexA.Y, VertexB.Y);
            y2 -= Math.Min(VertexA.Y, VertexB.Y);

            double step = 1/(double)Math.Max(width, height);
            for (double t = 0; t <= 1; t += step)
            {
                int x = (int)Math.Round(t * width);
                int y = (int)Math.Round(y1 + t * (y2 - y1));
                pixels[Math.Clamp(y, 0, height-1) * width + Math.Clamp(x, 0, width-1)] = Color.Red;
            }

            _lineTexture.SetData(pixels);
        }
    }
}
