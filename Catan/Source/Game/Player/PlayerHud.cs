using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Catan.Source.Game.Player
{
    public class PlayerHud : GameObject
    {
        public Player Player { get; }
        private static SpriteFont Font = null;

        public PlayerHud(float x, float y, Player player) : base(x, y)
        {
            Player = player;
            Font ??= Game1.ContentManager.Load<SpriteFont>("bigFont");
        }

        public override void OnSubscribe(Scene scene)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.DrawString(
                Font,
                "TESTE " + Player.PlayerNumber,
                new Vector2(X, Y),
                Color.Black, 0,
                new Vector2(0, 0),
                new Vector2(1, 1),
                SpriteEffects.None,
                0);
        }
    }
}
