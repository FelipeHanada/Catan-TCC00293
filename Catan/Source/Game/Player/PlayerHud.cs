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
    public class PlayerHud : GameObject
    {
        private Atlas _atlas;
        public Player Player { get; }
        private static SpriteFont Font = null;


        public PlayerHud(Atlas atlas, Player player) : base(0, 0)
        {
            _atlas = atlas;
            Player = player;
            Font ??= Game1.ContentManager.Load<SpriteFont>("bigFont");

            Action doNothing = () => {};

            PlayerInfo PlayerInfo = new PlayerInfo(1280 - 210, 10, _atlas, Color.White, 200, 200, "");
            PlayerInfo.setPlayer(Player);
            AddChild(PlayerInfo);

            UISlate tradeSlate = new UISlate(600, 200, _atlas, Color.Gray, 600, 300, "Trocar");
            tradeSlate.setEnabled(false);
            AddChild(tradeSlate);

            ResourceDisplay resourceDisplay = new ResourceDisplay(600,250, atlas);
            for (int i = 0; i < 10; i++) resourceDisplay.incrementResource(ResourceId.Wool);
            for (int i = 0; i < 21; i++) resourceDisplay.incrementResource(ResourceId.Wood);
            for (int i = 0; i < 3; i++) resourceDisplay.incrementResource(ResourceId.Ore);
            for (int i = 0; i < 4; i++) resourceDisplay.incrementResource(ResourceId.Brick);
            for (int i = 0; i < 5; i++) resourceDisplay.incrementResource(ResourceId.Wheat);
            AddChild(resourceDisplay);

            UISlate buildSlate = new UISlate(600, 0, _atlas, Color.Brown, 200, 200, "Contruir");
            buildSlate.setEnabled(false);
            AddChild(buildSlate);



            tradeSlate.addButton(new ButtonAction(0, 0, _atlas, doNothing, "Trocar"), 0, 0);
            
            
            
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
            /*
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
            */
        }
    }
}
