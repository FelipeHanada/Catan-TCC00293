using System;
using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Scenes
{
    internal class MainMenuScene : Scene
    {
        public override MusicId? Music => MusicId.MenuPrincipal;

        // Valores temporários para o placeholder
        private const int MaxPanelWidth = 620;
        private const int PanelHeight = 380;
        private const int ButtonHeight = 72;
        private const int ButtonSpacing = 18;
        private const int PanelPadding = 28;

        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;

        private readonly string[] _options = { "Jogar", "Sair" };
        private int _selectedOptionIndex;
        private KeyboardState _previousKeyboardState;
        private MouseState _previousMouseState;

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);

            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _font = Game1.ContentManager.Load<SpriteFont>("DefaultFont");

            _selectedOptionIndex = 0;
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _pixel?.Dispose();
            _pixel = null;
            _font = null;

            _spriteBatch?.Dispose();
            _spriteBatch = null;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);

            for (int i = 0; i < _options.Length; i++)
            {
                var buttonRect = GetButtonRect(i);
                if (buttonRect.Contains(mousePoint))
                {
                    _selectedOptionIndex = i;
                    break;
                }
            }

            if (IsJustPressed(keyboardState, Keys.Up) || IsJustPressed(keyboardState, Keys.W))
            {
                _selectedOptionIndex = (_selectedOptionIndex - 1 + _options.Length) % _options.Length;
            }

            if (IsJustPressed(keyboardState, Keys.Down) || IsJustPressed(keyboardState, Keys.S))
            {
                _selectedOptionIndex = (_selectedOptionIndex + 1) % _options.Length;
            }

            if (IsJustPressed(keyboardState, Keys.Enter))
            {
                ExecuteSelectedOption();
            }

            bool isLeftClick = mouseState.LeftButton == ButtonState.Pressed &&
                               _previousMouseState.LeftButton == ButtonState.Released;

            if (isLeftClick)
            {
                for (int i = 0; i < _options.Length; i++)
                {
                    var buttonRect = GetButtonRect(i);
                    if (buttonRect.Contains(mousePoint))
                    {
                        _selectedOptionIndex = i;
                        ExecuteSelectedOption();
                        break;
                    }
                }
            }

            _previousKeyboardState = keyboardState;
            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var viewport = Game1.GraphicsDeviceInstance.Viewport;
            var panelRect = GetPanelRect();
            var titleRect = GetTitleRect(panelRect);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                _pixel,
                new Rectangle(0, 0, viewport.Width, viewport.Height),
                Color.CornflowerBlue);

            _spriteBatch.Draw(_pixel, panelRect, new Color(33, 47, 67));
            _spriteBatch.Draw(_pixel, titleRect, new Color(57, 79, 105));

            DrawCenteredText("Menu", titleRect, Color.White);

            for (int i = 0; i < _options.Length; i++)
            {
                var rect = GetButtonRect(i);

                Color color = i == _selectedOptionIndex
                    ? new Color(230, 184, 75)
                    : new Color(85, 104, 130);

                _spriteBatch.Draw(_pixel, rect, color);
                DrawCenteredText(_options[i], rect, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        private Rectangle GetPanelRect()
        {
            var viewport = Game1.GraphicsDeviceInstance.Viewport;
            int width = Math.Min(MaxPanelWidth, viewport.Width - 80);
            int x = (viewport.Width - width) / 2;
            int y = (viewport.Height - PanelHeight) / 2;

            return new Rectangle(x, y, width, PanelHeight);
        }

        private Rectangle GetTitleRect(Rectangle panelRect)
        {
            int titleWidth = panelRect.Width - (PanelPadding * 2);
            int titleHeight = 86;
            return new Rectangle(panelRect.X + PanelPadding, panelRect.Y + PanelPadding, titleWidth, titleHeight);
        }

        private Rectangle GetButtonRect(int index)
        {
            var panelRect = GetPanelRect();
            int width = panelRect.Width - (PanelPadding * 2);
            int totalButtonsHeight = (_options.Length * ButtonHeight) + ((_options.Length - 1) * ButtonSpacing);
            int startY = panelRect.Bottom - PanelPadding - totalButtonsHeight;

            return new Rectangle(
                panelRect.X + PanelPadding,
                startY + index * (ButtonHeight + ButtonSpacing),
                width,
                ButtonHeight);
        }

        private void ExecuteSelectedOption()
        {
            switch (_selectedOptionIndex)
            {
                case 0:
                    Game1.ChangeScene(new ConfigScene());
                    break;
                case 1:
                    Game1.Instance().Exit();
                    break;
            }
        }

        private void DrawCenteredText(string text, Rectangle area, Color color)
        {
            Vector2 size = _font.MeasureString(text);
            Vector2 position = new Vector2(
                area.X + (area.Width - size.X) / 2f,
                area.Y + (area.Height - size.Y) / 2f);

            _spriteBatch.DrawString(_font, text, position, color);
        }
    }
}
