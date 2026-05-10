using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using Catan.Source.Game.Dice;


namespace Catan.Source.Scenes
{
    internal enum TurnPhase
    {
        WaitingForDiceRoll,
        RollingDice,
        PlayerActions,
    }

    internal class GameScene : Scene
    {
        private SpriteBatch _spriteBatch;
        private Atlas _atlas;

        private Board _board;
        private RandomDiceRoller _diceRoller;
        private DiceRollControl _diceRollControl;
        private MouseState _previousMouseState;
        private TurnPhase _turnPhase;
        private DiceRoll? _lastDiceRoll;

        public GameScene()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game1.GraphicsDeviceInstance);
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

            base.LoadContent();
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

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _board.Draw(gameTime, _spriteBatch);
            _diceRollControl.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
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
