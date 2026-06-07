using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Board;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Debug;
using Catan.Source.Game.Logging;
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
        private List<Player> _players;

        public DiceRollControl DiceRollControl { get; private set; }
        public GameBank Bank { get; private set; }
        public DevelopmentCardDeck DevelopmentCardDeck { get; private set; }
        public GameLog Log { get; private set; }
        public bool HasUsedDevelopmentCardThisTurn { get; private set; }
        public Board Board { get; private set; }
        public HarborHoverPreview HarborHoverPreview { get; private set; }
        public ScoreManager ScoreManager { get; private set; }

        public ButtonAction TradeButton { get; private set; }
        public ButtonAction BuildButton { get; private set; }
        public ButtonAction DevelopmentCardButton { get; private set; }
        public ButtonAction EndTurnButton { get; private set; }

        public BoardBackground Background { get; private set; }
        public DiceRoll LastDiceRoll { get; set; }
        public IReadOnlyList<Player> Players => _players;

        public GameScene()
        {
            _stateStack = new();
            Bank = new GameBank();
            DevelopmentCardDeck = new DevelopmentCardDeck();
            Log = new GameLog();
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

            Subscribe(new GameLogPanel(1000, 345, Atlas, Log));

            StandardRandomBoardFactory factory = new(Atlas, 192, 64);
            Board = factory.CreateBoard(this);

            Background = new BoardBackground(Board.Tiles[0].X, Board.Tiles[0].Y, Atlas);
            HarborHoverPreview = new HarborHoverPreview(Board, Atlas);
            Subscribe(Background);
            Subscribe(Board);

            ScoreManager = new ScoreManager(this);
            Subscribe(ScoreManager);

            DiceRollControl = new(Atlas, this);
            Subscribe(DiceRollControl);

            BuildButton = new ButtonAction(930, 620, Atlas, 75, 30, OnBuildButtonClicked, "Construir");
            TradeButton = new ButtonAction(840, 620, Atlas, 75, 30, OnTradeButtonClicked, "Trocar");
            DevelopmentCardButton = new ButtonAction(1020, 620, Atlas, 75, 30, OnDevelopmentCardButtonClicked, "Cartas");
            EndTurnButton = new ButtonAction(880, 660, Atlas, 175, 30, OnEndTurnButtonClicked, "Terminar turno");
            Subscribe(BuildButton);
            Subscribe(TradeButton);
            Subscribe(DevelopmentCardButton);
            Subscribe(EndTurnButton);

            UpdateActionButtons();

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

            GameState currentState = GetCurrentState();

            currentState.Update(gameTime);
            UpdateActionButtons();
            HarborHoverPreview.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            HarborHoverPreview.Draw(gameTime, spriteBatch);
        }

        public GameState GetCurrentState() => _stateStack.Count > 0 ? _stateStack.Peek() : null;
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

        private void OnTradeButtonClicked()
        {
            if (GetCurrentState() is PlayerTradeButtonCallback callback)
            {
                callback.OnPlayerTradeButtonClicked();
            }
        }

        private void OnBuildButtonClicked()
        {
            if (GetCurrentState() is PlayerBuildButtonCallback callback)
            {
                callback.OnPlayerBuildButtonClicked();
            }
        }

        private void OnDevelopmentCardButtonClicked()
        {
            if (GetCurrentState() is PlayerDevelopmentCardButtonCallback callback)
            {
                callback.OnPlayerDevelopmentCardButtonClicked();
            }
        }

        private void OnEndTurnButtonClicked()
        {
            if (GetCurrentState() is PlayerEndTurnButtonCallback callback)
            {
                callback.OnPlayerEndTurnButtonClicked();
            }
        }

        private void UpdateActionButtons()
        {
            GameState currentState = GetCurrentState();
            TradeButton?.SetEnabled(currentState is PlayerTradeButtonCallback);
            BuildButton?.SetEnabled(currentState is PlayerBuildButtonCallback);
            DevelopmentCardButton?.SetEnabled(currentState is PlayerDevelopmentCardButtonCallback);
            EndTurnButton?.SetEnabled(currentState is PlayerEndTurnButtonCallback);
        }

        public void ExitState()
        {
            GameState currentState = GetCurrentState();
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
