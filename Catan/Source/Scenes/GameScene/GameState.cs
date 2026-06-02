using System;
using System.Collections.Generic;
using Catan.Source.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catan.Source.Scenes.Game
{
    public abstract class GameState : IDisposable
    {
        public bool IsDisposed { get; private set; }
        protected GameScene _gameScene;
        public List<GameObject> Children { get; private set; }
        public GameState(GameScene gameScene)
        {
            _gameScene = gameScene;
            Children = [];
        }
        public void AddChild(GameObject child)
        {
            Children.Add(child);
        }
        ~GameState() => Dispose(false);
        public virtual void Initialize()
        { // quando entra pela primeira vez na stack
            LoadContent();
            foreach (GameObject obj in Children)
            {
                _gameScene.Subscribe(obj);
            }
        }
        public virtual void Uninitialize()
        { // quando não é mais o topo da stack = quando botam coisa em cima
        // quando sai da stack roda dispose
            foreach (GameObject obj in Children)
            {
                _gameScene.Unsubscribe(obj);
            }
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
                Uninitialize();
            }

            IsDisposed = true;
        }
    }
}
