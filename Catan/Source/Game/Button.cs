using Catan.Source.Content;
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
    public interface ICommand // Interface para o comando do botão, seguindo o padrão Command
    {
        void Execute();
    }

    public class Button : GameObject
    {
        private readonly Atlas atlas;
        private int width, height;
        private bool isDynamicSize = false;
        private string label = "";
        private SpriteFont _font;
        private float fontScale = 0.1f;
        private int padding = 2;

        private bool hovered;
        private bool pressed = false;
        private ICommand buttonCommand;
        private MouseState previousMouseState;

        private int cornerHeight = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Height,
                cornerWidth = Atlas.GetRectangle(AtlasSpriteId.ButtonBotLeft).Width;
        public Button(float x, float y, Atlas atlas, int width, int height, ICommand buttonCommand, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            
            this.atlas = atlas;
            this.width = width;
            this.height = height;
            this.isDynamicSize = false;
            this.buttonCommand = buttonCommand;
            this.label = label;
        }
        public Button(float x, float y, Atlas atlas, ICommand buttonCommand, string label) : base(x, y)
        {
            _font = Game1.ContentManager.Load<SpriteFont>("bigFont");
            this.atlas = atlas;
            this.width = 0;
            this.height = 0;
            this.isDynamicSize = true;
            this.buttonCommand = buttonCommand;
            this.label = label;
        }
        public void setFontScale(float scale)
        {
            this.fontScale = scale;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 size = _font.MeasureString(label);
            if (isDynamicSize)
            {
                width =  (int)(size.X*fontScale) + padding*cornerWidth;
                height = (int)(size.Y*fontScale) + padding*cornerHeight/2;
            }
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
            
//            spriteBatch.DrawString(_font, label, new Vector2(this.X + padding * cornerWidth + 1, this.Y + padding * cornerHeight + 1), new Color(0, 0, 0, 120), 0.0f, new Vector2(0, 0), fontScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(_font, label, new Vector2(this.X + cornerWidth + width/2, this.Y + cornerHeight + height/2), hovered ? Color.SaddleBrown : Color.Brown, 0.0f, new Vector2(size.X/2, size.Y/2.5f), fontScale, SpriteEffects.None, 0.0f);
        }
        public override void Update(GameTime gameTime) {
            MouseState mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;
            if (mousePos.X > this.X && mousePos.X < this.X + width + 2 * cornerWidth && mousePos.Y > this.Y && mousePos.Y < this.Y + height + 2 * cornerHeight)
            {
                if (hovered == false) hovered = true;
                else
                {
                    if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed) buttonCommand.Execute();
                }
            }
            else {
                hovered = false;
            };

            previousMouseState = mouseState;
        }
    }

    public class ButtonAction : Button
    {
        private class ActionICommand(Action action) : ICommand
        {
            public Action Action { get; private set; } = action;

            public void Execute()
            {
                Action.Invoke();
            }            
        }

        public ButtonAction(float x, float y, Atlas atlas, int width, int height, Action action, string label)
            : base(x, y, atlas, width, height, new ActionICommand(action), label) {}

        public ButtonAction(float x, float y, Atlas atlas, Action action, string label)
            : base(x, y, atlas, new ActionICommand(action), label) {}
    }
}
