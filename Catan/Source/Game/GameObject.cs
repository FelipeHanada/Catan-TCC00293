using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;

namespace Catan.Source.Game
{
    public class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }

        public GameObject(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public GameObject() : this(0, 0) {}

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Update(GameTime gameTime) { }

        public virtual void OnSubscribe(Scene scene) { }
        public virtual void OnUnsubscribe() { }
    }
}
