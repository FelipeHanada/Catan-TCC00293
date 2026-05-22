using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using System.Linq;
using Catan.Source.Game.Dice;


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

#if DEBUG
        private SpriteFont _font;
        private Texture2D _pixel;
#endif
        private Atlas _atlas;

        private Stack<GameState> _stateStack;
        private Board _board;
        private RandomDiceRoller _diceRoller;
        private DiceRollControl _diceRollControl;
        private MouseState _previousMouseState;
        private TurnPhase _turnPhase;
        private DiceRoll? _lastDiceRoll;
#if DEBUG
        private KeyboardState _previousKeyboardState;

        private readonly (Keys PrimaryKey, Keys AlternativeKey, SfxId SoundId, string Label)[] _soundHotkeys =
        {
            (Keys.D1, Keys.NumPad1, SfxId.ConstrucaoEstrada, "1 - Estrada"),
            (Keys.D2, Keys.NumPad2, SfxId.ConstrucaoCasa, "2 - Casa"),
            (Keys.D3, Keys.NumPad3, SfxId.LadraoDado7, "3 - Ladrao"),
            (Keys.D4, Keys.NumPad4, SfxId.Ovelha, "4 - Ovelha"),
            (Keys.D5, Keys.NumPad5, SfxId.Pedra, "5 - Pedra"),
            (Keys.D6, Keys.NumPad6, SfxId.Planta, "6 - Planta"),
            (Keys.D7, Keys.NumPad7, SfxId.Tijolo, "7 - Tijolo"),
            (Keys.D8, Keys.NumPad8, SfxId.TijoloCaindo, "8 - Tijolo caindo"),
        };
#endif

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
            _font = Game1.ContentManager.Load<SpriteFont>("DefaultFont");

            _pixel = new Texture2D(Game1.GraphicsDeviceInstance, 1, 1);
            _pixel.SetData(new[] { Color.White });
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
#if DEBUG
            _previousKeyboardState = Keyboard.GetState();
#endif

            _previousMouseState = Mouse.GetState();
            _turnPhase = TurnPhase.WaitingForDiceRoll;
            Subscribe(_board);
        }

        public override void UnloadContent()
        {
#if DEBUG
            _pixel?.Dispose();
            _pixel = null;
            _font = null;
#endif

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F1))//atalho temporario para tela de fim
            {
                Game1.ChangeScene(new EndGameScene());
                return;
            }

#if DEBUG
            var keyboardState = Keyboard.GetState();

            foreach (var hotkey in _soundHotkeys)
            {
                if (IsJustPressed(keyboardState, hotkey.PrimaryKey) ||
                    IsJustPressed(keyboardState, hotkey.AlternativeKey))
                {
                    SoundManager.Instance.Play(hotkey.SoundId);
                    break;
                }
            }

            _previousKeyboardState = keyboardState;
#endif

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

#if DEBUG
            DrawSoundHotkeys(gameTime, spriteBatch);
#endif

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

#if DEBUG
        private void DrawSoundHotkeys(GameTime gameTime, SpriteBatch spriteBatch)
        {
            const int x = 880;
            const int y = 24;
            const int width = 360;
            const int padding = 16;
            const int lineHeight = 26;

            var height = padding * 2 + lineHeight * (_soundHotkeys.Length + 1);
            var panel = new Rectangle(x, y, width, height);

            spriteBatch.Draw(_pixel, panel, new Color(20, 26, 34, 210));
            spriteBatch.DrawString(_font, "Teste de som", new Vector2(x + padding, y + padding), Color.White);

            for (var i = 0; i < _soundHotkeys.Length; i++)
            {
                var position = new Vector2(x + padding, y + padding + lineHeight * (i + 1));
                spriteBatch.DrawString(_font, _soundHotkeys[i].Label, position, Color.LightGray);
            }
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
#endif

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
