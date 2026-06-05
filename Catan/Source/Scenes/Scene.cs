using System;
using Catan.Source.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Game;

namespace Catan.Source.Scenes
{
    public abstract class Scene : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public virtual MusicId? Music => null;

        protected List<GameObject> GameObjects { get; } = [];
        private readonly Queue<GameObject> _toSubscribe = [];
        private readonly Queue<GameObject> _toUnsubscribe = [];

        public Scene()
        {
        }

        public void Subscribe(GameObject obj)
        {
            _toSubscribe.Enqueue(obj);

        }

        public void Unsubscribe(GameObject obj)
        {
            _toUnsubscribe.Enqueue(obj);
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

            while (_toSubscribe.Count > 0)
            {
                var obj = _toSubscribe.Dequeue();
                if (!GameObjects.Contains(obj))
                {
                    GameObjects.Add(obj);
                    obj.OnSubscribe(this);
                }
            }

            while (_toUnsubscribe.Count > 0)
            {
                var obj = _toUnsubscribe.Dequeue();
                if (GameObjects.Remove(obj))
                {
                    obj.OnUnsubscribe(this);
                }
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
                foreach (var obj in GameObjects)
                {
                    obj.OnUnsubscribe(this);
                }
                GameObjects.Clear();
                UnloadContent();
            }

            IsDisposed = true;
        }
    }
}
