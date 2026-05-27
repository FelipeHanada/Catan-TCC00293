using Catan.Source.Game.Resources;
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

        public static void DrawString(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            spriteBatch.DrawString(
                Font,
                text,
                position,
                Color.White, 0,
                new Vector2(0, 0),
                new Vector2(0.1f, 0.1f),
                SpriteEffects.None,
                0);   
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            float deltaY = 0;

            DrawString(spriteBatch, "PlayerNumber: " + Player.PlayerNumber, new(X, Y + deltaY));
            deltaY += 16;

            foreach (ResourceId resourceId in ResourceUtils.ResourceIds)
            {
                int amount = Player.Inventory.Resources.GetAmount(resourceId);
                DrawString(spriteBatch, ResourceUtils.ResourceName[resourceId].ToUpper() + ": " + amount, new(X, Y + deltaY));
                deltaY += 16;
            }
        }
    }
}
