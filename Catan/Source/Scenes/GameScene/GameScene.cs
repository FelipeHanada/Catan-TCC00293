using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Board;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Debug;
using Catan.Source.Game.Player;
using Catan.Source.Scenes.Game;
using GameBank = Catan.Source.Game.Bank.Bank;


namespace Catan.Source.Scenes
{
    public interface PlayerTradeButtonCallback { void OnPlayerTradeButtonClicked(); }
    public interface PlayerBuildButtonCallback { void OnPlayerBuildButtonClicked(); }
    public interface PlayerDevelopmentCardButtonCallback { void OnPlayerDevelopmentCardButtonClicked(); }
    public interface PlayerEndTurnButtonCallback { void OnPlayerEndTurnButtonClicked(); }

    public class GameScene : Scene
    {
        public override MusicId? Music => MusicId.Partida;
        public Atlas Atlas { get; private set; }

        private Stack<GameState> _stateStack;

        public DiceRollControl DiceRollControl { get; private set; }
        public GameBank Bank { get; private set; }
        public Board Board { get; private set; }

        public ButtonAction TradeButton { get; private set; }
        public ButtonAction BuildButton { get; private set; }
        public ButtonAction DevelopmentCardButton { get; private set; }
        public ButtonAction EndTurnButton { get; private set; }

        public BoardBackground Background;
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

            BuildButton = new ButtonAction(930, 620, Atlas, 75, 30, OnBuildButtonClicked, "Construir");
            TradeButton = new ButtonAction(840, 620, Atlas, 75, 30, OnTradeButtonClicked, "Trocar");
            DevelopmentCardButton = new ButtonAction(1020, 620, Atlas, 75, 30, OnDevelopmentCardButtonClicked, "Usar");
            EndTurnButton = new ButtonAction(880, 660, Atlas, 175, 30, OnEndTurnButtonClicked, "Terminar turno");

            Subscribe(BuildButton);
            Subscribe(TradeButton);
            Subscribe(DevelopmentCardButton);
            Subscribe(EndTurnButton);

            UpdateActionButtons();

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
            UpdateActionButtons();
        }
        public GameState GetCurrentStateGame() => _stateStack.Count > 0 ? _stateStack.Peek() : null;
        public Player GetPlayer(int playerNumber)
        {
            return _players[playerNumber];
        }

        private void OnTradeButtonClicked()
        {
            if (GetCurrentStateGame() is PlayerTradeButtonCallback callback)
            {
                callback.OnPlayerTradeButtonClicked();
            }
        }

        private void OnBuildButtonClicked()
        {
            if (GetCurrentStateGame() is PlayerBuildButtonCallback callback)
            {
                callback.OnPlayerBuildButtonClicked();
            }
        }

        private void OnDevelopmentCardButtonClicked()
        {
            if (GetCurrentStateGame() is PlayerDevelopmentCardButtonCallback callback)
            {
                callback.OnPlayerDevelopmentCardButtonClicked();
            }
        }

        private void OnEndTurnButtonClicked()
        {
            if (GetCurrentStateGame() is PlayerEndTurnButtonCallback callback)
            {
                callback.OnPlayerEndTurnButtonClicked();
            }
        }

        private void UpdateActionButtons()
        {
            GameState currentState = GetCurrentStateGame();
            TradeButton?.setEnabled(currentState is PlayerTradeButtonCallback);
            BuildButton?.setEnabled(currentState is PlayerBuildButtonCallback);
            DevelopmentCardButton?.setEnabled(currentState is PlayerDevelopmentCardButtonCallback);
            EndTurnButton?.setEnabled(currentState is PlayerEndTurnButtonCallback);
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

            UpdateActionButtons();
        }

        public void AppendState(GameState gameState)
        {
            if (_stateStack.Count > 0)
            {
                _stateStack.Peek().Uninitialize();
            }

            _stateStack.Push(gameState);
            gameState.Initialize();
            UpdateActionButtons();
        }
    }
}
