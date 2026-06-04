using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game.Board;
using Catan.Source.Game.DevelopmentCards;
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
        public Atlas Atlas { get; private set; }

        private Stack<GameState> _stateStack;

        public DiceRollControl DiceRollControl { get; private set; }
        public GameBank Bank { get; private set; }
        public DevelopmentCardDeck DevelopmentCardDeck { get; private set; }
        public bool HasUsedDevelopmentCardThisTurn { get; private set; }
        public Board Board { get; private set; }

        public BoardBackground Background;
        public DiceRoll LastDiceRoll { get; set; }

        public List<Player> _players;

        public GameScene()
        {
            _stateStack = new();
            Bank = new GameBank();
            DevelopmentCardDeck = new DevelopmentCardDeck();
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
            //Subscribe(new SoundBoardDebug());
            Subscribe(new BankDebugPanel(Bank));
            #endif

            Atlas = new Atlas(Game1.ContentManager);

            StandardRandomBoardFactory factory = new(Atlas, 0, 0);
            Board = factory.CreateBoard(this);

            Background = new BoardBackground(Board.Tiles[0].X, Board.Tiles[0].Y, Atlas);
            Subscribe(Background);
            Subscribe(Board);

            DiceRollControl = new(Atlas, this);
            Subscribe(DiceRollControl);

            // AppendState.Push(new PositionSettlementGameState(this));
            // AppendState.Push(new WaitingForDiceRollGameState(this, diceRollControl));
            // AppendState.Push(new ResourceProductionGameState(this, _players[0], diceRollControl));

            AppendState(new PlayerTurnManagerGameState(this, _players));
            AppendState(new SetupGameState(this));
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
        public GameState GetCurrentStateGame() => _stateStack.Peek();
        public Player GetPlayer(int playerNumber)
        {
            return _players[playerNumber];
        }

        public void MarkDevelopmentCardUsed()
        {
            HasUsedDevelopmentCardThisTurn = true;
        }

        public void ResetDevelopmentCardUsageForTurn()
        {
            HasUsedDevelopmentCardThisTurn = false;
        }

        public void ExitState()
        {
            GameState currentState = GetCurrentStateGame();
            currentState.Dispose();
            _stateStack.Pop();

            if (_stateStack.Count > 0)
            {
                _stateStack.Peek().Initialize();
            }
        }

        public void AppendState(GameState gameState)
        {
            if (_stateStack.Count > 0)
            {
                _stateStack.Peek().Uninitialize();
            }

            _stateStack.Push(gameState);
            gameState.Initialize();
        }
    }
}
