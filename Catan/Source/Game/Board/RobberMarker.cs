using Catan;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
// Só para ter algo que represente o ladrão por agora
namespace Catan.Source.Game.Board
{
    public class RobberMarker : GameObject
    {
        private readonly Board _board;
        private readonly SpriteFont _font;

        public RobberMarker(Board board)
        {
            _board = board;
            _font = Game1.ContentManager.Load<SpriteFont>("defaultFont");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Tile robberTile = _board.RobberTile;
            if (robberTile == null)
            {
                return;
            }

            const string marker = "L";
            Vector2 markerSize = _font.MeasureString(marker);
            Vector2 tileCenter = new(robberTile.X + 64, robberTile.Y + 64);
            Vector2 markerPosition = tileCenter - markerSize / 2;

            spriteBatch.DrawString(_font, marker, markerPosition + new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(_font, marker, markerPosition, Color.Red);
        }
    }
}
