using Catan.Source.Content;
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
    public interface ICommand // Interface para o comando do botão, seguindo o padrão Command
    {
        void Execute();
    }
    public class Button : GameObject
    {
        private readonly Atlas _atlas;
        private int _width, _height;
        private bool _isDynamicSize = false;
        private string _label = "";
        private SpriteFont _font;
        private float _fontScale = 0.1f;
        private int _padding = 2;

        private bool _hovered;
        private bool _enabled = true;
        private ICommand _buttonCommand;
        private MouseState _previousMouseState;
        public bool IsHovered => _hovered;

        private int _cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height,
                _cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;
        public Button(float x, float y, Atlas atlas, int width, int height, ICommand buttonCommand, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            
            _atlas = atlas;
            _width = width;
            _height = height;
            _isDynamicSize = false;
            _buttonCommand = buttonCommand;
            _label = label;
        }
        public Button(float x, float y, Atlas atlas, ICommand buttonCommand, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            _atlas = atlas;
            _width = 0;
            _height = 0;
            _isDynamicSize = true;
            _buttonCommand = buttonCommand;
            _label = label;
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color edgeColor = _enabled ? Color.White : Color.Gray;
            Color fillColor = _enabled ? (_hovered ? Color.LightGray : Color.White) : Color.DarkGray;

            Vector2 size = _font.MeasureString(_label);
            if (_isDynamicSize)
            {
                _width =  (int)(size.X*_fontScale) + _padding*_cornerWidth;
                _height = (int)(size.Y*_fontScale) + _padding*_cornerHeight/2;
            }
            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpLeft),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X + _width + _cornerWidth, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpRight),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X, this.Y + _height + _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(this.X + _width + _cornerWidth, this.Y + _height + _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotRight),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X, (int) this.Y + _cornerHeight, _cornerWidth, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeLeft),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _width + _cornerWidth, (int) this.Y + _cornerHeight, _cornerWidth, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeRight),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _cornerWidth, (int) this.Y, _width, _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeTop),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int) this.X + _cornerWidth, (int) this.Y + _height + _cornerHeight, _width, _cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeBot),
                edgeColor);

            spriteBatch.Draw(
                _atlas.Texture,
                new Rectangle((int)this.X + _cornerWidth, (int)this.Y + _cornerHeight, _width, _height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonFill),
                fillColor);
            
//            spriteBatch.DrawString(_font, _label, new Vector2(this.X + _padding * _cornerWidth + 1, this.Y + _padding * _cornerHeight + 1), new Color(0, 0, 0, 120), 0.0f, new Vector2(0, 0), _fontScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font, _label, new Vector2(this.X + _cornerWidth + _width/2, this.Y + _cornerHeight + _height/2), _enabled ? (_hovered ? Color.SaddleBrown : Color.SaddleBrown) : Color.Brown, 0.0f, new Vector2(size.X/2, size.Y/2.5f), _fontScale, SpriteEffects.None, 0.0f);
        }
        public override void Update(GameTime gameTime) {
            MouseState mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;
            if (mousePos.X > this.X && mousePos.X < this.X + _width + 2 * _cornerWidth && mousePos.Y > this.Y && mousePos.Y < this.Y + _height + 2 * _cornerHeight)
            {
                if (_hovered == false) _hovered = true;
                else
                {
                    if (!_enabled) return;
                    if (_previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed) _buttonCommand.Execute();
                }
            }
            else {
                _hovered = false;
            };

            _previousMouseState = mouseState;
        }
    }

    public class ButtonAction : Button
    {
        private class ActionICommand(Action action) : ICommand
        {
            public Action Action { get; private set; } = action;

            public void Execute()
            {
                Action.Invoke();
            }            
        }

        public ButtonAction(float x, float y, Atlas atlas, int width, int height, Action action, string label)
            : base(x, y, atlas, width, height, new ActionICommand(action), label) {}

        public ButtonAction(float x, float y, Atlas atlas, Action action, string label)
            : base(x, y, atlas, new ActionICommand(action), label) {}

        public ButtonAction(float x, float y, Atlas atlas, Action action, string label, bool enabled)
            : base(x, y, atlas, new ActionICommand(action), label) {
            this.SetEnabled(enabled);
        }
    }
}
