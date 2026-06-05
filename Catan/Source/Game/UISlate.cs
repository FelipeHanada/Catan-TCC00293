using Catan.Source.Content;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Catan.Source.Game
{
    public class UISlate : GameObject
    {
        private readonly Atlas _atlas;
        private int _width, _height;
        private string _label = "";
        private SpriteFont _font;
        private float _fontScale = 0.1f;
        private Color _color;

        private bool _enabled = true;

        private int _cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height,
                _cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;

        public UISlate(float x, float y, Atlas atlas, Color color, int width, int height, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            
            _atlas = atlas;
            _width = width;
            _height = height;
            _label = label;
            _color = color;
        }
        public UISlate(float x, float y, Atlas atlas, Color color, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            _atlas = atlas;
            _width = 0;
            _height = 0;
            _label = label;
            _color = color;
        }
        public void SetFontScale(float scale)
        {
            _fontScale = scale;
        }
        public void SetEnabled(bool isEnabled)
        {
            _enabled = isEnabled;
        }
        public Boolean GetEnabled()
        {
            return _enabled;
        }

        public void AddButton(Button button, float x, float y)
        {
            button.X = this.X + x + _cornerWidth;
            button.Y = this.Y + y + _font.MeasureString(_label).Y*_fontScale;

            AddChild(button);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!_enabled) return;
            Vector2 size = _font.MeasureString(_label);
            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpLeft),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X + _width + _cornerWidth, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpRight),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X, this.Y + _height + _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X + _width + _cornerWidth, this.Y + _height + _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotRight),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X, (int) this.Y + _cornerHeight, _cornerWidth, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeLeft),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _width + _cornerWidth, (int) this.Y + _cornerHeight, _cornerWidth, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeRight),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _cornerWidth, (int) this.Y, _width, _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeTop),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _cornerWidth, (int) this.Y + _height + _cornerHeight, _width, _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeBot),
                _color);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int)this.X + _cornerWidth, (int)this.Y + _cornerHeight, _width, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonFill),
                _color);
            
            spriteBatch.DrawString(_font, _label, new Vector2(this.X + _cornerWidth, this.Y + _cornerWidth), Color.White, 0.0f, new Vector2(0, 0), _fontScale, SpriteEffects.None, 0.0f);
        }
        public override void Update(GameTime gameTime) {
        }

    }
}
