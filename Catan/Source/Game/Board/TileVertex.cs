using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Game.Board
{
    public class TileVertex : GameObject
    {
        private static Texture2D? _circleTexture;
        private const int CircleDiameter = 10;

        public TileVertex(float x, float y)
            : base(x, y)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_circleTexture == null)
            {
                _circleTexture = new Texture2D(Game1.GraphicsDeviceInstance, CircleDiameter, CircleDiameter);
                var pixels = new Color[CircleDiameter * CircleDiameter];
                var radius = CircleDiameter / 2f;
                var radiusSq = radius * radius;
                var center = new Vector2(radius, radius);

                for (int y = 0; y < CircleDiameter; y++)
                {
                    for (int x = 0; x < CircleDiameter; x++)
                    {
                        var offset = new Vector2(x, y) - center;
                        var distanceSq = offset.LengthSquared();
                        pixels[y * CircleDiameter + x] = distanceSq <= radiusSq
                            ? Color.Red
                            : Color.Transparent;
                    }
                }

                _circleTexture.SetData(pixels);
            }

            var origin = new Vector2(CircleDiameter / 2f);
            spriteBatch.Draw(_circleTexture, new Vector2(X, Y), null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
