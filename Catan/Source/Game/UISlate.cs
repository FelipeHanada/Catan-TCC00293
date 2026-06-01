using Catan.Source.Content;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Catan.Source.Game
{
    public class UISlate : GameObject
    {
        private readonly Atlas atlas;
        private int width, height;
        private string label = "";
        private SpriteFont _font;
        private float fontScale = 0.1f;
        private int padding = 2;
        private Color color;

        private bool enabled = true;
        private MouseState previousMouseState;

        private int cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height,
                cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;

        public UISlate(float x, float y, Atlas atlas, Color color, int width, int height, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            
            this.atlas = atlas;
            this.width = width;
            this.height = height;
            this.label = label;
            this.color = color;
        }
        public UISlate(float x, float y, Atlas atlas, Color color, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            this.atlas = atlas;
            this.width = 0;
            this.height = 0;
            this.label = label;
            this.color = color;
        }
        public void setFontScale(float scale)
        {
            this.fontScale = scale;
        }
        public void setEnabled(bool isEnabled)
        {
            this.enabled = isEnabled;
        }
        public Boolean getEnabled()
        {
            return this.enabled;
        }

        public void addButton(Button button, float x, float y)
        {
            button.X = this.X + x + cornerWidth;
            button.Y = this.Y + y + _font.MeasureString(label).Y*fontScale;

            AddChild(button);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!this.enabled) return;
            Vector2 size = _font.MeasureString(label);
            /*
            if (isDynamicSize)
            {
                width =  (int)(size.X*fontScale) + padding*cornerWidth;
                height = (int)(size.Y*fontScale) + padding*cornerHeight/2;
            }
            */
            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpLeft),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X + width + cornerWidth, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpRight),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y + height + cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X + width + cornerWidth, this.Y + height + cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotRight),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X, (int) this.Y + cornerHeight, cornerWidth, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeLeft),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + width + cornerWidth, (int) this.Y + cornerHeight, cornerWidth, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeRight),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + cornerWidth, (int) this.Y, width, cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeTop),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + cornerWidth, (int) this.Y + height + cornerHeight, width, cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeBot),
                color);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int)this.X + cornerWidth, (int)this.Y + cornerHeight, width, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonFill),
                color);
            
//            spriteBatch.DrawString(_font, label, new Vector2(this.X + padding * cornerWidth + 1, this.Y + padding * cornerHeight + 1), new Color(0, 0, 0, 120), 0.0f, new Vector2(0, 0), fontScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font, label, new Vector2(this.X + cornerWidth, this.Y + cornerWidth), Color.White, 0.0f, new Vector2(0, 0), fontScale, SpriteEffects.None, 0.0f);
        }
        public override void Update(GameTime gameTime) {
        }

    }
    public class PlayerInfo : UISlate
    {
        private Game.Player.Player Player = null;
        private readonly Atlas atlas;
        public PlayerInfo(float x, float y, Atlas atlas, Color color, int width, int height, string label) : base(x, y, atlas, color, width, height, label)
        {
            this.atlas = atlas;
        }

        public void setPlayer(Game.Player.Player player)
        {
            this.Player = player;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            
            if (Player == null) return;
            float deltaY = 16;


            Game.Player.PlayerHud.DrawString(spriteBatch, "PlayerNumber: " + Player.PlayerNumber, new(X+16, Y + deltaY));
            deltaY += 16;

            foreach (ResourceId resourceId in ResourceUtils.ResourceIds)
            {
                int amount = Player.Inventory.Resources.GetAmount(resourceId);
                Game.Player.PlayerHud.DrawString(spriteBatch, ResourceUtils.ResourceName[resourceId].ToUpper() + ": " + amount, new(X+16, Y + deltaY));
                deltaY += 16;
            }
            
        }
    }
}
