using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using GameBank = Catan.Source.Game.Bank.Bank;

namespace Catan.Source.Game.Debug
{
    public class BankDebugPanel : GameObject
    {
        private readonly GameBank _bank;
        private readonly (ResourceId Resource, string Label)[] _resources =
        {
            (ResourceId.Wood, "Mad"),
            (ResourceId.Brick, "Tij"),
            (ResourceId.Wheat, "Tri"),
            (ResourceId.Wool, "Ove"),
            (ResourceId.Ore, "Min"),
        };

        private SpriteFont _font;
        private Texture2D _pixel;

        public BankDebugPanel(GameBank bank) : base(24, 24)
        {
            if (bank == null)
            {
                throw new ArgumentNullException(nameof(bank), "Banco não pode ser nulo.");
            }

            _bank = bank;
            _font = Game1.ContentManager.Load<SpriteFont>("DefaultFont");
            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public override void OnUnsubscribe(Scene scene)
        {
            base.OnUnsubscribe(scene);
            _pixel?.Dispose();
            _pixel = null;
            _font = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            const int Width = 136;
            const int Padding = 8;
            const int LineHeight = 17;
            const float TextScale = 0.65f;
            int height = Padding * 2 + LineHeight * (_resources.Length + 1);
            var viewport = Game1.GraphicsDeviceInstance.Viewport;
            var panel = new Rectangle(12, viewport.Height - height - 12, Width, height);

            spriteBatch.Draw(_pixel, panel, new Color(20, 26, 34, 210));
            DrawText(spriteBatch, "Banco", panel.X + Padding, panel.Y + Padding, Color.White, TextScale);

            for (int i = 0; i < _resources.Length; i++)
            {
                var resource = _resources[i];
                string text = $"{resource.Label}: {_bank.GetAmount(resource.Resource)}";
                int y = panel.Y + Padding + LineHeight * (i + 1);

                DrawText(spriteBatch, text, panel.X + Padding, y, Color.LightGray, TextScale);
            }
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(_font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
