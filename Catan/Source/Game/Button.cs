using Catan.Source.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catan.Source.Game
{
    public class Button : GameObject
    {
        private readonly Atlas atlas;
        private int width, height;
        private bool hovered;

        private int cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height,
                cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;
        public Button(float x, float y, Atlas atlas, int width, int height) : base(x, y)
        {
            this.atlas = atlas;
            this.width = width;
            this.height = height;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpLeft),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X + width + cornerWidth, this.Y),
                Atlas.GetRectangle(AtlasSpriteId.ButtonUpRight),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X, this.Y + height + cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Vector2(this.X + width + cornerWidth, this.Y + height + cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonBotRight),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X, (int) this.Y + cornerHeight, cornerWidth, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeLeft),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + width + cornerWidth, (int) this.Y + cornerHeight, cornerWidth, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeRight),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + cornerWidth, (int) this.Y, width, cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeTop),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int) this.X + cornerWidth, (int) this.Y + height + cornerHeight, width, cornerHeight),
                Atlas.GetRectangle(AtlasSpriteId.ButtonEdgeBot),
                Color.White);

            spriteBatch.Draw(
                atlas.Texture,
                new Rectangle((int)this.X + cornerWidth, (int)this.Y + cornerHeight, width, height),
                Atlas.GetRectangle(AtlasSpriteId.ButtonFill),
                hovered ? Color.LightGray : Color.White);
        }
        public override void Update(GameTime gameTime) {
            Point mousePos = Mouse.GetState().Position;
            if (mousePos.X > this.X && mousePos.X < this.X + width + 2 * cornerWidth && mousePos.Y > this.Y && mousePos.Y < this.Y + height + 2 * cornerHeight)
            {
                hovered = true;
            }
            else hovered = false;
        }
    }
}
