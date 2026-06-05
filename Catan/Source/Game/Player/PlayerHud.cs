using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace Catan.Source.Game.Player
{
    public class PlayerHud : UISlate
    {
        private Player _player;
        private GameScene _gameScene;
        private static SpriteFont _font = null;

        public PlayerHud(GameScene gameScene, Player player, float x, float y, Color color, int width, int height)
            : base(x, y, gameScene.Atlas, color, width, height, "PlayerNumber: " + player.PlayerNumber)
        {
            _gameScene = gameScene;
            _player = player;
            _font ??= Game1.ContentManager.Load<SpriteFont>("bigFont");
        }

        public static Vector2 DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Vector2 delta = default)
        {
            spriteBatch.DrawString(
                _font,
                text,
                position,
                Color.White, 0,
                new Vector2(0, 0),
                new Vector2(0.1f, 0.1f),
                SpriteEffects.None,
                0);

            position += delta;
            return position;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            
            Vector2 delta = new(0, 16);
            Vector2 position = new(X+16, Y+24);

            foreach (ResourceId resourceId in ResourceUtils.ResourceIds)
            {
                int amount = _player.Inventory.Resources.GetAmount(resourceId);
                position = DrawString(spriteBatch, ResourceUtils.ResourceName[resourceId].ToUpper() + ": " + amount, position, delta);
            }

            position += new Vector2(0, 8);
            DrawString(spriteBatch, "Pontuacao: " + _gameScene.ScoreManager.GetScore(_player), position);
        }
    }
}
