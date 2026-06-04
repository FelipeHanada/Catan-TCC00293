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
            const int width = 136;
            const int padding = 8;
            const int lineHeight = 17;
            const float textScale = 0.65f;
            int height = padding * 2 + lineHeight * (_resources.Length + 1);
            var viewport = Game1.GraphicsDeviceInstance.Viewport;
            var panel = new Rectangle(12, viewport.Height - height - 12, width, height);

            spriteBatch.Draw(_pixel, panel, new Color(20, 26, 34, 210));
            DrawText(spriteBatch, "Banco", panel.X + padding, panel.Y + padding, Color.White, textScale);

            for (int i = 0; i < _resources.Length; i++)
            {
                var resource = _resources[i];
                string text = $"{resource.Label}: {_bank.GetAmount(resource.Resource)}";
                int y = panel.Y + padding + lineHeight * (i + 1);

                DrawText(spriteBatch, text, panel.X + padding, y, Color.LightGray, textScale);
            }
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(_font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
