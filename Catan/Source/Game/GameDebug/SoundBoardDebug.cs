using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;

namespace Catan.Source.Game.Debug
{
    public class SoundBoardDebug : GameObject
    {
        private KeyboardState _previousKeyboardState;
        private SpriteFont _font;
        private Texture2D _pixel;


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

        public SoundBoardDebug() : base(0, 0)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("DefaultFont");
            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _previousKeyboardState = Keyboard.GetState();
        }

        public override void OnUnsubscribe()
        {
            base.OnUnsubscribe();
            _pixel?.Dispose();
            _pixel = null;
            _font = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawSoundHotkeys(gameTime, spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
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
        }

        private void DrawSoundHotkeys(GameTime gameTime, SpriteBatch spriteBatch)
        {
            const int x = 880;
            const int y = 24;
            const int width = 360;
            const int padding = 16;
            const int lineHeight = 26;

            var height = padding * 2 + lineHeight * (_soundHotkeys.Length + 1);
            var panel = new Rectangle(x, y, width, height);

            spriteBatch.Draw(_pixel, panel, new Color(20, 26, 34, 210));
            spriteBatch.DrawString(_font, "Teste de som", new Vector2(x + padding, y + padding), Color.White);

            for (var i = 0; i < _soundHotkeys.Length; i++)
            {
                var position = new Vector2(x + padding, y + padding + lineHeight * (i + 1));
                spriteBatch.DrawString(_font, _soundHotkeys[i].Label, position, Color.LightGray);
            }
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}
