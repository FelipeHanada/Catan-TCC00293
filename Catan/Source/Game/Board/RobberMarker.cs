using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Game.Board
{
    public class RobberMarker : GameObject
    {
        private readonly Board _board;
        private readonly Atlas _atlas;

        public RobberMarker(Board board, Atlas atlas)
        {
            _board = board;
            _atlas = atlas;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Tile robberTile = _board.RobberTile;
            if (robberTile == null)
            {
                return;
            }

            Rectangle robberSprite = Atlas.GetRectangle(AtlasSpriteId.Robber);
            Vector2 tileCenter = new(robberTile.X + 64, robberTile.Y + 64);
            Vector2 markerPosition = tileCenter - new Vector2(robberSprite.Width, robberSprite.Height) / 2;

            spriteBatch.Draw(_atlas.Texture, markerPosition, robberSprite, Color.White);
        }
    }
}
