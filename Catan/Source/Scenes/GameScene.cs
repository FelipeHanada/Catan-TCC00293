using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Debug;


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

    internal enum TurnPhase
    {
        WaitingForDiceRoll,
        RollingDice,
        PlayerActions,
    }

    internal class GameScene : Scene
    {
        public override MusicId? Music => MusicId.Partida;

        private Atlas _atlas;

        private Stack<GameState> _stateStack;
        private Board _board;
        private RandomDiceRoller _diceRoller;
        private DiceRollControl _diceRollControl;
        private MouseState _previousMouseState;
        private TurnPhase _turnPhase;
        private DiceRoll? _lastDiceRoll;

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

            #if DEBUG
            Subscribe(new SoundBoardDebug());
            #endif


            _atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(_atlas, 0, 0);
            _board = factory.CreateBoard();
            _diceRoller = new RandomDiceRoller();

            var viewport = Game1.GraphicsDeviceInstance.Viewport;
            int diceControlWidth = (DiceRollControl.FaceSize * 2) + DiceRollControl.FaceSpacing;
            _diceRollControl = new DiceRollControl(
                viewport.Width - diceControlWidth - 32,
                viewport.Height - DiceRollControl.FaceSize - 32,
                _atlas);

            _previousMouseState = Mouse.GetState();
            _turnPhase = TurnPhase.WaitingForDiceRoll;
            Subscribe(_board);
        }

        public override void UnloadContent()
        {
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

            MouseState mouseState = Mouse.GetState();

            _diceRollControl.IsEnabled = _turnPhase == TurnPhase.WaitingForDiceRoll;

            if (_turnPhase == TurnPhase.WaitingForDiceRoll
                && _diceRollControl.WasClicked(mouseState, _previousMouseState))
            {
                _diceRollControl.StartRoll(_diceRoller.Roll());
                _turnPhase = TurnPhase.RollingDice;
            }

            _diceRollControl.Update(gameTime);

            if (_turnPhase == TurnPhase.RollingDice && _diceRollControl.HasSettledResult)
            {
                DiceRoll roll = _diceRollControl.ConsumeSettledResult();
                _lastDiceRoll = roll;
                ResolveDiceRoll(roll);
                _turnPhase = TurnPhase.PlayerActions;
            }

            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            GameState currentState = GetCurrentStateGame();
            currentState.Draw(gameTime);

            _diceRollControl.Draw(gameTime, spriteBatch);
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

        private void ResolveDiceRoll(DiceRoll roll)
        {
            if (roll.Total == 7)
            {
                // Logica do ladrao.
                return;
            }

            // logica de producao acho 9?).
        }
    }
}
