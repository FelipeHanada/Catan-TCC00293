using System.Collections.Generic;
using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Game.Logging
{
    public class GameLogPanel : GameObject
    {
        private const int VisibleMessageCount = 6;
        private readonly GameLog _log;
        private readonly Atlas _atlas;
        private readonly int _width;
        private readonly int _height;
        private readonly SpriteFont _font;
        private readonly int _cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height;
        private readonly int _cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;

        public GameLogPanel(float x, float y, Atlas atlas, GameLog log, int width = 270, int height = 135)
            : base(x, y)
        {
            _atlas = atlas;
            _log = log;
            _width = width;
            _height = height;
            _font = Game1.ContentManager.Load<SpriteFont>("mediumFont");
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawPanel(spriteBatch);
            DrawText(spriteBatch);
        }

        private void DrawPanel(SpriteBatch spriteBatch)
        {
            Color color = Color.Gray;

            spriteBatch.Draw(_atlas.Texture, new Vector2(X, Y), Atlas.GetRectangle(AtlasSpriteId.ButtonUpLeft), color);
            spriteBatch.Draw(_atlas.Texture, new Vector2(X + _width + _cornerWidth, Y), Atlas.GetRectangle(AtlasSpriteId.ButtonUpRight), color);
            spriteBatch.Draw(_atlas.Texture, new Vector2(X, Y + _height + _cornerHeight), Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft), color);
            spriteBatch.Draw(_atlas.Texture, new Vector2(X + _width + _cornerWidth, Y + _height + _cornerHeight), Atlas.GetRectangle(AtlasSpriteId.ButtonBotRight), color);

            spriteBatch.Draw(_atlas.Texture, new Rectangle((int)X, (int)Y + _cornerHeight, _cornerWidth, _height), Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeLeft), color);
            spriteBatch.Draw(_atlas.Texture, new Rectangle((int)X + _width + _cornerWidth, (int)Y + _cornerHeight, _cornerWidth, _height), Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeRight), color);
            spriteBatch.Draw(_atlas.Texture, new Rectangle((int)X + _cornerWidth, (int)Y, _width, _cornerHeight), Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeTop), color);
            spriteBatch.Draw(_atlas.Texture, new Rectangle((int)X + _cornerWidth, (int)Y + _height + _cornerHeight, _width, _cornerHeight), Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeBot), color);
            spriteBatch.Draw(_atlas.Texture, new Rectangle((int)X + _cornerWidth, (int)Y + _cornerHeight, _width, _height), Atlas.GetRectangle(AtlasSpriteId.ButtonFill), color);
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            float titleScale = 0.9f;
            float messageScale = 0.8f;
            float textX = X + _cornerWidth + 8;
            float textY = Y + _cornerHeight + 6;

            spriteBatch.DrawString(_font, "Log", new Vector2(textX, textY), Color.White, 0.0f, Vector2.Zero, titleScale, SpriteEffects.None, 0.0f);

            IReadOnlyList<string> messages = _log.Messages;
            int firstMessageIndex = System.Math.Max(0, messages.Count - VisibleMessageCount);
            float lineY = textY + 24;

            for (int i = firstMessageIndex; i < messages.Count; i++)
            {
                spriteBatch.DrawString(_font, messages[i], new Vector2(textX, lineY), Color.White, 0.0f, Vector2.Zero, messageScale, SpriteEffects.None, 0.0f);
                lineY += 17;
            }
        }
    }
}
