using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Scenes;
using System.Collections.Generic;

namespace Catan.Source.Game
{
    public class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public List<GameObject> Children { get; private set; }

        public GameObject(float x, float y)
        {
            X = x;
            Y = y;
            Children = [];
        }

        public GameObject() : this(0, 0) {}

        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {}
        public virtual void Update(GameTime gameTime) { }

        public virtual void OnSubscribe(Scene scene)
        {
            foreach (GameObject child in Children)
            {
                scene.Subscribe(child);
            }
        }
        public virtual void OnUnsubscribe(Scene scene)
        {
            foreach (GameObject child in Children)
            {
                scene.Unsubscribe(child);
            }
        }
    }
}
