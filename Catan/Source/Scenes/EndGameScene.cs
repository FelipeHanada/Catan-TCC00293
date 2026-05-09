using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Scenes
{
    internal class EndGameScene : Scene
    {
        public override MusicId? Music => MusicId.FimDeJogo;

        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _pixel;

        private MouseState _previousMouseState;

        private Rectangle _backToMenuButton;
        private Rectangle _exitButton;

        public EndGameScene()
        {
            _spriteBatch = null!;
            _font = null!;
            _pixel = null!;
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
            _font = Game1.ContentManager.Load<SpriteFont>("defaultFont");

            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _backToMenuButton = new Rectangle(420, 240, 240, 60);
            _exitButton = new Rectangle(420, 320, 240, 60);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _pixel?.Dispose();
            _pixel = null!;

            _spriteBatch?.Dispose();
            _spriteBatch = null!;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (IsLeftMouseJustClicked(mouse, _previousMouseState))
            {
                HandleClick(mouse.Position);
            }

            _previousMouseState = mouse;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(_font, "Fim de Jogo", new Vector2(40, 60), Color.White);

            DrawButton(_backToMenuButton, "Voltar ao Menu");
            DrawButton(_exitButton, "Sair");

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleClick(Point mousePosition)
        {
            if (_backToMenuButton.Contains(mousePosition))
            {
                Game1.ChangeScene(new MainMenuScene());
                return;
            }

            if (_exitButton.Contains(mousePosition))
            {
                Game1.Instance().Exit();
            }
        }

        private void DrawButton(Rectangle rectangle, string text)
        {
            _spriteBatch.Draw(_pixel, rectangle, Color.DimGray);

            var textSize = _font.MeasureString(text);
            var textPosition = new Vector2(
                rectangle.X + (rectangle.Width - textSize.X) / 2f,
                rectangle.Y + (rectangle.Height - textSize.Y) / 2f
            );

            _spriteBatch.DrawString(_font, text, textPosition, Color.White);
        }

        private static bool IsLeftMouseJustClicked(MouseState current, MouseState previous)
        {
            return current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released;
        }
    }
}
