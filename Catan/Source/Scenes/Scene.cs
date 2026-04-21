using System;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes
{
    public abstract class Scene : IDisposable
    {
        public bool IsDisposed { get; private set; }

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

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

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
