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

        private readonly Atlas atlas;

        private DiceRollControlState state;
        private DiceRoll result;
        private double rollElapsed;
        private double faceChangeElapsed;
        private double blinkElapsed;
        private int visibleFirst;
        private int visibleSecond;
        private MouseState _previousMouseState;

        public bool IsEnabled { get; set; }
        public bool IsRolling => state == DiceRollControlState.Rolling;
        public bool HasSettledResult => state == DiceRollControlState.Settled;
        public DiceRoll Result => result;

        private Rectangle Bounds => new(
            (int)X,
            (int)Y,
            (FaceSize * 2) + FaceSpacing,
            FaceSize);

        private GameScene _gameScene;

        public DiceRollControl(float x, float y, Atlas atlas, GameScene gameScene)
            : base(x, y)
        {
            this.atlas = atlas;
            state = DiceRollControlState.Idle;
            result = new DiceRoll(1, 1);
            visibleFirst = result.First;
            visibleSecond = result.Second;
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
            if (state == DiceRollControlState.Rolling)
            {
                return;
            }

            this.result = result;
            state = DiceRollControlState.Rolling;
            rollElapsed = 0;
            faceChangeElapsed = FaceChangeInterval;
        }

        public DiceRoll ConsumeSettledResult()
        {
            var roll = result;
            state = DiceRollControlState.Idle;
            return roll;
        }

        public override void Update(GameTime gameTime)
        {
            IsEnabled = _gameScene.GetCurrentStateGame() is WaitingForDiceRollGameState;

            MouseState currentMouseState = Mouse.GetState();
            if (IsEnabled && state == DiceRollControlState.Idle
                && currentMouseState.LeftButton == ButtonState.Pressed
                && _previousMouseState.LeftButton == ButtonState.Released
                && Bounds.Contains(currentMouseState.Position))
            {
                StartRoll(new DiceRoll(Random.Shared.Next(1, 7), Random.Shared.Next(1, 7)));
            }

            _previousMouseState = currentMouseState;

            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            blinkElapsed += elapsedSeconds;
            if (blinkElapsed >= BlinkDuration)
            {
                blinkElapsed -= BlinkDuration;
            }

            if (state != DiceRollControlState.Rolling)
            {
                return;
            }

            rollElapsed += elapsedSeconds;
            faceChangeElapsed += elapsedSeconds;

            if (faceChangeElapsed >= FaceChangeInterval)
            {
                faceChangeElapsed = 0;
                visibleFirst = Random.Shared.Next(1, 7);
                visibleSecond = Random.Shared.Next(1, 7);
            }

            if (rollElapsed >= RollDuration)
            {
                visibleFirst = result.First;
                visibleSecond = result.Second;
                state = DiceRollControlState.Settled;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            bool shouldBlink = IsEnabled && state == DiceRollControlState.Idle;
            Color tint;
            int yOffset = IsEnabled && state != DiceRollControlState.Rolling ? -3 : 0;

            if (shouldBlink)
            {
                float blink = (float)((Math.Sin((blinkElapsed / BlinkDuration) * Math.PI * 2) + 1) / 2);
                byte intensity = (byte)(BlinkMinIntensity + (BlinkMaxIntensity - BlinkMinIntensity) * blink);
                tint = new Color(intensity, intensity, intensity);

                if (blink > 0.5f)
                {
                    yOffset -= 2;
                }
            }
            else
            {
                tint = IsEnabled || state != DiceRollControlState.Idle
                    ? Color.White
                    : new Color(170, 170, 170);
            }

            DrawDice(spriteBatch, visibleFirst, GetFaceRectangle(0, yOffset), tint);
            DrawDice(spriteBatch, visibleSecond, GetFaceRectangle(1, yOffset), tint);
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
                atlas.Texture,
                destination,
                Atlas.GetRectangle(Atlas.GetDiceFaceSprite(face)),
                tint);
        }
    }
}
