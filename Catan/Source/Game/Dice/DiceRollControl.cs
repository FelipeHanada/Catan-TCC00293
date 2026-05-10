using System;
using Catan.Source.Content;
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

        private readonly Atlas atlas;

        private DiceRollControlState state;
        private DiceRoll result;
        private double rollElapsed;
        private double faceChangeElapsed;
        private int visibleFirst;
        private int visibleSecond;
        private bool hasUnreadSettledResult;

        public bool IsEnabled { get; set; }
        public bool IsRolling => state == DiceRollControlState.Rolling;
        public bool HasSettledResult => hasUnreadSettledResult;
        public DiceRoll Result => result;

        private Rectangle Bounds => new(
            (int)X,
            (int)Y,
            (FaceSize * 2) + FaceSpacing,
            FaceSize);

        public DiceRollControl(float x, float y, Atlas atlas)
            : base(x, y)
        {
            this.atlas = atlas;
            state = DiceRollControlState.Idle;
            result = new DiceRoll(1, 1);
            visibleFirst = result.First;
            visibleSecond = result.Second;
            IsEnabled = true;
        }

        public bool WasClicked(MouseState currentMouse, MouseState previousMouse)
        {
            return IsEnabled
                && state != DiceRollControlState.Rolling
                && currentMouse.LeftButton == ButtonState.Pressed
                && previousMouse.LeftButton == ButtonState.Released
                && Bounds.Contains(currentMouse.Position);
        }

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
            hasUnreadSettledResult = false;
        }

        public DiceRoll ConsumeSettledResult()
        {
            hasUnreadSettledResult = false;
            return result;
        }

        public override void Update(GameTime gameTime)
        {
            if (state != DiceRollControlState.Rolling)
            {
                return;
            }

            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;
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
                hasUnreadSettledResult = true;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Color tint = IsEnabled || state != DiceRollControlState.Idle
                ? Color.White
                : new Color(170, 170, 170);

            int yOffset = IsEnabled && state != DiceRollControlState.Rolling ? -3 : 0;

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
