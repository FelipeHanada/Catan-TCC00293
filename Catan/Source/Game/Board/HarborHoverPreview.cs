using Catan.Source.Content;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HarborModel = Catan.Source.Game.Harbor.Harbor;

namespace Catan.Source.Game.Board
{
    public class HarborHoverPreview : GameObject
    {
        private const int HoverSize = 72;
        private const int PreviewOffsetX = 16;
        private const int PreviewOffsetY = 16;

        private readonly Board _board;
        private readonly Atlas _atlas;
        private HarborModel _hoveredHarbor;
        private Point _mousePosition;

        public HarborHoverPreview(Board board, Atlas atlas)
        {
            _board = board;
            _atlas = atlas;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            _mousePosition = mouseState.Position;
            _hoveredHarbor = null;

            foreach (HarborModel harbor in _board.Harbors)
            {
                if (GetHoverBounds(harbor).Contains(_mousePosition))
                {
                    _hoveredHarbor = harbor;
                    return;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_hoveredHarbor == null)
            {
                return;
            }

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(_mousePosition.X + PreviewOffsetX, _mousePosition.Y + PreviewOffsetY),
                Atlas.GetRectangle(GetSpriteId(_hoveredHarbor)),
                Color.White);
        }

        private static Rectangle GetHoverBounds(HarborModel harbor)
        {
            float centerX = 0;
            float centerY = 0;

            foreach (TileVertex vertex in harbor.Vertices)
            {
                centerX += vertex.X;
                centerY += vertex.Y;
            }

            centerX /= harbor.Vertices.Count;
            centerY /= harbor.Vertices.Count;

            return new Rectangle(
                (int)centerX - HoverSize / 2,
                (int)centerY - HoverSize / 2,
                HoverSize,
                HoverSize);
        }

        private static AtlasSpriteId GetSpriteId(HarborModel harbor)
        {
            if (harbor.Type == HarborType.Generic)
            {
                return AtlasSpriteId.UniversalTradePopup;
            }

            return harbor.Resource switch
            {
                ResourceId.Wool => AtlasSpriteId.WoolTradePopup,
                ResourceId.Wheat => AtlasSpriteId.WheatTradePopup,
                ResourceId.Ore => AtlasSpriteId.OreTradePopup,
                ResourceId.Brick => AtlasSpriteId.BrickTradePopup,
                ResourceId.Wood => AtlasSpriteId.WoodTradePopup,
                _ => AtlasSpriteId.UniversalTradePopup,
            };
        }
    }
}
