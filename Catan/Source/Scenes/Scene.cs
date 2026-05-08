using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Game;

namespace Catan.Source.Scenes
{
    public abstract class Scene : IDisposable
    {
        public bool IsDisposed { get; private set; }

        protected List<GameObject> GameObjects { get; } = new List<GameObject>();

        public Scene()
        {
        }

        ~Scene() => Dispose(false);

        public virtual void Initialize()
        {
            LoadContent();
        }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var obj in GameObjects)
            {
                obj.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var obj in GameObjects)
            {
                obj.Draw(gameTime, spriteBatch);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                UnloadContent();
            }

            IsDisposed = true;
        }
    }
}
