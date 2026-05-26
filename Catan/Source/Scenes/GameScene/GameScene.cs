using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Debug;
using Catan.Source.Game.Player;
using Catan.Source.Scenes.Game;
using GameBank = Catan.Source.Game.Bank.Bank;


namespace Catan.Source.Scenes
{
    public class GameScene : Scene
    {
        public override MusicId? Music => MusicId.Partida;
        private Atlas _atlas;

        private Stack<GameState> _stateStack;

        public GameBank Bank { get; }
        public Board Board { get; private set; }
        public DiceRoll LastDiceRoll { get; set; }

        public List<Player> _players;

        public GameScene()
        {
            _stateStack = new();
            Bank = new GameBank();
            _players = [];
            for (int i=0; i<4; i++)
            {
                _players.Add(new Player(i));
            }
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
            Subscribe(new BankDebugPanel(Bank));
            #endif

            _atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(_atlas, 0, 0);
            Board = factory.CreateBoard(this);
            Subscribe(Board);

            DiceRollControl diceRollControl = new(_atlas, this);
            Subscribe(diceRollControl);

            // _stateStack.Push(new PositionSettlementGameState(this));
            // _stateStack.Push(new WaitingForDiceRollGameState(this, diceRollControl));
            // _stateStack.Push(new ResourceProductionGameState(this, _players[0], diceRollControl));
            _stateStack.Push(new SetupGameState(this));
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.F1))//atalho temporario para tela de fim
            {
                Game1.ChangeScene(new EndGameScene());
                return;
            }

            GameState currentState = GetCurrentStateGame();

            Console.Out.WriteLine(currentState);

            currentState.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            GameState currentState = GetCurrentStateGame();
            currentState.Draw(gameTime, spriteBatch);
        }

        public GameState GetCurrentStateGame() => _stateStack.Peek();
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
