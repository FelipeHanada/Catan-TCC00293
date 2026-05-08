using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using System.Linq;


namespace Catan.Source.Scenes
{
    public abstract class GameState : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public GameState()
        {
        }
        ~GameState() => Dispose(false);
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

    public class PositionSettlementGameState : GameState
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

    internal class GameScene : Scene
    {
        private SpriteBatch _spriteBatch;
        private Atlas _atlas;

        private Stack<GameState> _stateStack;
        private Board _board;

        public GameScene()
        {
            _stateStack = new();
        }

        public override void Initialize()
        {
            base.Initialize();

            _stateStack.Push(new PositionSettlementGameState());
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
            _atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(_atlas, 0, 0);
            _board = factory.CreateBoard();
        }

        public override void UnloadContent()
        {
            _spriteBatch?.Dispose();
            _spriteBatch = null;

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F1))//atalho temporario para tela de fim
            {
                Game1.ChangeScene(new EndGameScene());
                return;
            }

            GameState currentState = GetCurrentStateGame();
            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _board.Draw(gameTime, _spriteBatch);

            GameState currentState = GetCurrentStateGame();
            currentState.Draw(gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public GameState GetCurrentStateGame() => _stateStack.First();
        public void ExitState()
        {
            GameState currentState = GetCurrentStateGame();
            currentState.Dispose();
            _stateStack.Pop();
        }

        public void AppendState(GameState gameState)
        {
            _stateStack.Append(gameState);
            gameState.Initialize();
        }
    }
}
