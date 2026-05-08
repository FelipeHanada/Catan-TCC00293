using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game.Board;


namespace Catan.Source.Scenes
{
    internal class GameScene : Scene
    {
        private SpriteBatch _spriteBatch;
#if DEBUG
        private SpriteFont _font;
        private Texture2D _pixel;
#endif
        private Atlas _atlas;

        private Board _board;
#if DEBUG
        private KeyboardState _previousKeyboardState;

        private readonly (Keys PrimaryKey, Keys AlternativeKey, SfxId SoundId, string Label)[] _soundHotkeys =
        {
            (Keys.D1, Keys.NumPad1, SfxId.ConstrucaoEstrada, "1 - Estrada"),
            (Keys.D2, Keys.NumPad2, SfxId.ConstrucaoCasa, "2 - Casa"),
            (Keys.D3, Keys.NumPad3, SfxId.LadraoDado7, "3 - Ladrao"),
            (Keys.D4, Keys.NumPad4, SfxId.Ovelha, "4 - Ovelha"),
            (Keys.D5, Keys.NumPad5, SfxId.Pedra, "5 - Pedra"),
            (Keys.D6, Keys.NumPad6, SfxId.Planta, "6 - Planta"),
            (Keys.D7, Keys.NumPad7, SfxId.Tijolo, "7 - Tijolo"),
            (Keys.D8, Keys.NumPad8, SfxId.TijoloCaindo, "8 - Tijolo caindo"),
        };
#endif

        public GameScene()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
#if DEBUG
            _font = Game1.ContentManager.Load<SpriteFont>("DefaultFont");

            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });
#endif

            _atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(_atlas, 0, 0);
            _board = factory.CreateBoard();
#if DEBUG
            _previousKeyboardState = Keyboard.GetState();
#endif

            base.LoadContent();
        }

        public override void UnloadContent()
        {
#if DEBUG
            _pixel?.Dispose();
            _pixel = null;
            _font = null;
#endif

            _spriteBatch?.Dispose();
            _spriteBatch = null;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F1))//atalho temporario para tela de fim
            {
                Game1.ChangeScene(new EndGameScene());
                return;
            }

#if DEBUG
            var keyboardState = Keyboard.GetState();

            foreach (var hotkey in _soundHotkeys)
            {
                if (IsJustPressed(keyboardState, hotkey.PrimaryKey) ||
                    IsJustPressed(keyboardState, hotkey.AlternativeKey))
                {
                    SoundManager.Instance.Play(hotkey.SoundId);
                    break;
                }
            }

            _previousKeyboardState = keyboardState;
#endif

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _board.Draw(gameTime, _spriteBatch);
#if DEBUG
            DrawSoundHotkeys();
#endif

            _spriteBatch.End();

            base.Draw(gameTime);
        }

#if DEBUG
        private void DrawSoundHotkeys()
        {
            const int x = 880;
            const int y = 24;
            const int width = 360;
            const int padding = 16;
            const int lineHeight = 26;

            var height = padding * 2 + lineHeight * (_soundHotkeys.Length + 1);
            var panel = new Rectangle(x, y, width, height);

            _spriteBatch.Draw(_pixel, panel, new Color(20, 26, 34, 210));
            _spriteBatch.DrawString(_font, "Teste de som", new Vector2(x + padding, y + padding), Color.White);

            for (var i = 0; i < _soundHotkeys.Length; i++)
            {
                var position = new Vector2(x + padding, y + padding + lineHeight * (i + 1));
                _spriteBatch.DrawString(_font, _soundHotkeys[i].Label, position, Color.LightGray);
            }
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
#endif
    }
}
