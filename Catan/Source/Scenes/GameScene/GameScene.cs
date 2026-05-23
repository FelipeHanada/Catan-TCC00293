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
using Catan.Source.Scenes.Game;


namespace Catan.Source.Scenes
{
    public class GameScene : Scene
    {
        public override MusicId? Music => MusicId.Partida;
        private Atlas _atlas;

        private Stack<GameState> _stateStack;

        public Board Board { get; private set; }
        public DiceRoll LastDiceRoll { get; set; }

        public GameScene()
        {
            _stateStack = new();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            #if DEBUG
            Subscribe(new SoundBoardDebug());
            #endif

            _atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(_atlas, 0, 0);
            Board = factory.CreateBoard();
            Subscribe(Board);

            DiceRollControl diceRollControl = new DiceRollControl(_atlas, this);
            Subscribe(diceRollControl);

            _stateStack.Push(new PositionSettlementGameState(this));
            _stateStack.Push(new WaitingForDiceRoll(this, diceRollControl));
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            GameState currentState = GetCurrentStateGame();
            currentState.Draw(gameTime, spriteBatch);
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
            _stateStack.Push(gameState);
            gameState.Initialize();
        }
    }
}
