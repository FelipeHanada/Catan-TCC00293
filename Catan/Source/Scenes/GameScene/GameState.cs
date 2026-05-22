using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Scenes.Game
{
    public abstract class GameState : IDisposable
    {
        public bool IsDisposed { get; private set; }
        protected GameScene _gameScene;
        public GameState(GameScene gameScene)
        {
            _gameScene = gameScene;
        }
        ~GameState() => Dispose(false);
        public virtual void Initialize()
        {
            LoadContent();
        }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
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
