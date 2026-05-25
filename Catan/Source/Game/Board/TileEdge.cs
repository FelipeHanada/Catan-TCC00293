using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Scenes.Game;
using Catan.Source.Content;

namespace Catan.Source.Game.Board
{
    public class TileEdge : GameObject
    {
        public TileVertex VertexA { get; }
        public TileVertex VertexB { get; }

        private Texture2D _lineTexture;

        private const float Radius = 4;
        private MouseState _previousMouseState;

        private readonly Atlas atlas;
        private readonly GameScene gameScene;

        public Player.Player RoadOwner { get; private set; }

        public TileEdge(TileVertex vertexA, TileVertex vertexB, Atlas atlas, GameScene gameScene)
            : base(0, 0)
        {
            this.VertexA = vertexA;
            this.VertexB = vertexB;
            this.atlas = atlas;
            this.gameScene = gameScene;
            RoadOwner = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            EnsureTextureGenerated();

            var position = new Vector2(Math.Min(VertexA.X, VertexB.X), Math.Min(VertexA.Y, VertexB.Y));
            spriteBatch.Draw(_lineTexture, position, null, Color.White, 0f, new(0, 0), 1f, SpriteEffects.None, 0f);


            if (RoadOwner is not null)
            {
                float ax = VertexA.X, ay = VertexA.Y;
                float bx = VertexB.X, by = VertexB.Y;

                if (ax < bx || (ax == bx && ay < by))
                {
                    (ax, ay, bx, by) = (bx, by, ax, ay);
                }

                AtlasPlayerSprite spriteId;
                if (ax == bx)
                {
                    // draw vertical road
                    spriteId = AtlasPlayerSprite.Road1;
                } else if (ay < by)
                {
                    // draw horizontal descending road
                    spriteId = AtlasPlayerSprite.Road1;
                } else
                {
                    // draw horizontal ascending road
                    spriteId = AtlasPlayerSprite.Road1;
                }

                Rectangle rect = Atlas.GetRectangle(spriteId, RoadOwner.PlayerNumber);
                spriteBatch.Draw(atlas.Texture, rect, Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();

            if (gameScene.GetCurrentStateGame() is PositionRoadGameState gameState)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed
                    && _previousMouseState.LeftButton == ButtonState.Released
                    && IsHovering(currentMouseState)
                )
                {
                    if (CanPlaceRoad(gameState.Player))
                    {
                        PlaceRoad(gameState.Player);
                        gameScene.ExitState();

                        SoundManager.Instance.Play(SfxId.ConstrucaoEstrada);
                    } else
                    {
                        SoundManager.Instance.Play(SfxId.TijoloCaindo);
                    }
                }
            }

            _previousMouseState = currentMouseState;
        }

        public bool CanPlaceRoad(Player.Player player)
        {
            return true;
        }

        public void PlaceRoad(Player.Player player)
        {
            RoadOwner = player;
        }

        private bool IsHovering(MouseState mouseState)
        {
            float left = Math.Min(VertexA.X, VertexB.X);
            float right = Math.Max(VertexA.X, VertexB.X);
            float top = Math.Min(VertexA.Y, VertexB.Y);
            float bottom = Math.Max(VertexA.Y, VertexB.Y);
            float mx = mouseState.X, my = mouseState.Y;

            if (mx < left || mx > right || my < top || my > bottom) return false;

            float ux = right - left, uy = bottom - top;
            float vx = mx - left, vy = my - top;

            float dot_uv = ux * vx + uy * vy;
            float dot_uu = ux * ux + uy * uy;

            float projx = ux * dot_uv / dot_uu;
            float projy = uy * dot_uv / dot_uu;

            float dx = Math.Abs(projx - vx), dy = Math.Abs(projy - vy);
            float dist2 = dx * dx + dy * dy;
            return dist2 <= Radius * Radius;
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
