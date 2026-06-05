using System;
using Catan.Source.Content;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Catan.Source.Game.Dice
{
    public enum DiceRollControlState
    {
        Idle,
        Rolling,
        Settled,
    }

    public class DiceRollControl : GameObject
    {
        public const int FaceSize = 64;
        public const int FaceSpacing = 10;
        private const double RollDuration = 1;
        private const double FaceChangeInterval = 0.08;
        private const double BlinkDuration = 0.6;
        private const byte BlinkMinIntensity = 120;
        private const byte BlinkMaxIntensity = 255;

        private readonly Atlas _atlas;

        private DiceRollControlState _state;
        private DiceRoll _result;
        private double _rollElapsed;
        private double _faceChangeElapsed;
        private double _blinkElapsed;
        private int _visibleFirst;
        private int _visibleSecond;
        private MouseState _previousMouseState;

        public bool IsEnabled { get; set; }
        public bool IsRolling => _state == DiceRollControlState.Rolling;
        public bool HasSettledResult => _state == DiceRollControlState.Settled;
        public DiceRoll Result => _result;

        private Rectangle Bounds => new(
            (int)X,
            (int)Y,
            (FaceSize * 2) + FaceSpacing,
            FaceSize);

        private GameScene _gameScene;

        public DiceRollControl(float x, float y, Atlas atlas, GameScene gameScene)
            : base(x, y)
        {
            _atlas = atlas;
            _state = DiceRollControlState.Idle;
            _result = new DiceRoll(1, 1);
            _visibleFirst = _result.First;
            _visibleSecond = _result.Second;
            IsEnabled = true;
            _gameScene = gameScene;
            _previousMouseState = Mouse.GetState();
        }

        public DiceRollControl(Atlas atlas, GameScene gameScene)
            : this(
                Game1.GraphicsDeviceInstance.Viewport.Width - ((DiceRollControl.FaceSize * 2) + DiceRollControl.FaceSpacing) - 32,
                Game1.GraphicsDeviceInstance.Viewport.Height - DiceRollControl.FaceSize - 32,
                atlas,
                gameScene
            ) {}

        public void StartRoll(DiceRoll result)
        {
            if (_state == DiceRollControlState.Rolling)
            {
                return;
            }

            _result = result;
            _state = DiceRollControlState.Rolling;
            _rollElapsed = 0;
            _faceChangeElapsed = FaceChangeInterval;
        }

        public DiceRoll ConsumeSettledResult()
        {
            var roll = _result;
            _state = DiceRollControlState.Idle;
            return roll;
        }

        public override void Update(GameTime gameTime)
        {
            IsEnabled = _gameScene.GetCurrentState() is WaitingForDiceRollGameState;

            MouseState currentMouseState = Mouse.GetState();
            if (IsEnabled && _state == DiceRollControlState.Idle
                && currentMouseState.LeftButton == ButtonState.Pressed
                && _previousMouseState.LeftButton == ButtonState.Released
                && Bounds.Contains(currentMouseState.Position))
            {
                StartRoll(new DiceRoll(Random.Shared.Next(1, 7), Random.Shared.Next(1, 7)));
            }

            _previousMouseState = currentMouseState;

            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            _blinkElapsed += elapsedSeconds;
            if (_blinkElapsed >= BlinkDuration)
            {
                _blinkElapsed -= BlinkDuration;
            }

            if (_state != DiceRollControlState.Rolling)
            {
                return;
            }

            _rollElapsed += elapsedSeconds;
            _faceChangeElapsed += elapsedSeconds;

            if (_faceChangeElapsed >= FaceChangeInterval)
            {
                _faceChangeElapsed = 0;
                _visibleFirst = Random.Shared.Next(1, 7);
                _visibleSecond = Random.Shared.Next(1, 7);
            }

            if (_rollElapsed >= RollDuration)
            {
                _visibleFirst = _result.First;
                _visibleSecond = _result.Second;
                _state = DiceRollControlState.Settled;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            bool shouldBlink = IsEnabled && _state == DiceRollControlState.Idle;
            Color tint;
            int yOffset = IsEnabled && _state != DiceRollControlState.Rolling ? -3 : 0;

            if (shouldBlink)
            {
                float blink = (float)((Math.Sin((_blinkElapsed / BlinkDuration) * Math.PI * 2) + 1) / 2);
                byte intensity = (byte)(BlinkMinIntensity + (BlinkMaxIntensity - BlinkMinIntensity) * blink);
                tint = new Color(intensity, intensity, intensity);

                if (blink > 0.5f)
                {
                    yOffset -= 2;
                }
            }
            else
            {
                tint = IsEnabled || _state != DiceRollControlState.Idle
                    ? Color.White
                    : new Color(170, 170, 170);
            }

            DrawDice(spriteBatch, _visibleFirst, GetFaceRectangle(0, yOffset), tint);
            DrawDice(spriteBatch, _visibleSecond, GetFaceRectangle(1, yOffset), tint);
        }

        private Rectangle GetFaceRectangle(int index, int yOffset)
        {
            return new Rectangle(
                (int)X + index * (FaceSize + FaceSpacing),
                (int)Y + yOffset,
                FaceSize,
                FaceSize);
        }

        private void DrawDice(SpriteBatch spriteBatch, int face, Rectangle destination, Color tint)
        {
            spriteBatch.Draw(
                _atlas.Texture,
                destination,
                Atlas.GetRectangle(Atlas.GetDiceFaceSprite(face)),
                tint);
        }
    }
}
