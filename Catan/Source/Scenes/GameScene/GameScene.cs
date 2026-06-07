using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.AI;
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
        public AiTurnMode AiTurnMode { get; private set; }
        public RandomAiStrategy AiStrategy { get; private set; }
        public Board Board { get; private set; }
        public HarborHoverPreview HarborHoverPreview { get; private set; }
        public ScoreManager ScoreManager { get; private set; }
        public int TargetScore { get; }

        public ButtonAction TradeButton { get; private set; }
        public ButtonAction BuildButton { get; private set; }
        public ButtonAction DevelopmentCardButton { get; private set; }
        public ButtonAction EndTurnButton { get; private set; }
        public ButtonAction AiAutoModeButton { get; private set; }
        public ButtonAction AiManualModeButton { get; private set; }

        public BoardBackground Background { get; private set; }
        public DiceRoll LastDiceRoll { get; set; }
        public IReadOnlyList<Player> Players => _players;

        public GameScene() : this(CreateDefaultSettings())
        {
        }

        internal GameScene(MatchSettings settings)
        {
            _stateStack = new();
            Bank = new GameBank();
            DevelopmentCardDeck = new DevelopmentCardDeck();
            AiTurnMode = AiTurnMode.Auto;
            AiStrategy = new RandomAiStrategy();
            Log = new GameLog();
            _players = [];
            TargetScore = settings.TargetScore;

            for (int i = 0; i < settings.Players.Count; i++)
            {
                MatchPlayer matchPlayer = settings.Players[i];
                _players.Add(new Player(i, matchPlayer.Name, matchPlayer.IsAi));
            }
        }

        private static MatchSettings CreateDefaultSettings()
        {
            MatchSettings settings = new()
            {
                TargetScore = 10,
            };

            for (int i = 0; i < 4; i++)
            {
                settings.Players.Add(new MatchPlayer
                {
                    Name = $"Jogador {i + 1}",
                    IsAi = false,
                });
            }

            return settings;
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
            AiAutoModeButton = new ButtonAction(10, 70, Atlas, 75, 24, () => SetAiTurnMode(AiTurnMode.Auto), "IA Auto");
            AiManualModeButton = new ButtonAction(100, 70, Atlas, 90, 24, () => SetAiTurnMode(AiTurnMode.Manual), "IA Manual");
            Subscribe(BuildButton);
            Subscribe(TradeButton);
            Subscribe(DevelopmentCardButton);
            Subscribe(EndTurnButton);
            Subscribe(AiAutoModeButton);
            Subscribe(AiManualModeButton);

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

        public void SetAiTurnMode(AiTurnMode mode)
        {
            AiTurnMode = mode;
            UpdateAiModeButtons();
        }

        private void OnTradeButtonClicked()
        {
            if (CanCurrentPlayerUseManualActionButtons() &&
                GetCurrentState() is PlayerTradeButtonCallback callback)
            {
                callback.OnPlayerTradeButtonClicked();
            }
        }

        private void OnBuildButtonClicked()
        {
            if (CanCurrentPlayerUseManualActionButtons() &&
                GetCurrentState() is PlayerBuildButtonCallback callback)
            {
                callback.OnPlayerBuildButtonClicked();
            }
        }

        private void OnDevelopmentCardButtonClicked()
        {
            if (CanCurrentPlayerOpenDevelopmentCards() &&
                GetCurrentState() is PlayerDevelopmentCardButtonCallback callback)
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
            bool canUseManualActionButtons = CanCurrentPlayerUseManualActionButtons();
            TradeButton?.SetEnabled(canUseManualActionButtons && currentState is PlayerTradeButtonCallback);
            BuildButton?.SetEnabled(canUseManualActionButtons && currentState is PlayerBuildButtonCallback);
            DevelopmentCardButton?.SetEnabled(CanCurrentPlayerOpenDevelopmentCards() && currentState is PlayerDevelopmentCardButtonCallback);
            EndTurnButton?.SetEnabled(CanCurrentPlayerEndTurn() && currentState is PlayerEndTurnButtonCallback);
            UpdateAiModeButtons();
        }

        private bool CanCurrentPlayerUseManualActionButtons()
        {
            return AiTurnInputPolicy.CanUseManualActionButtons(IsAiManualControlBlockedState());
        }

        private bool CanCurrentPlayerOpenDevelopmentCards()
        {
            if (IsAiDevelopmentCardState())
            {
                return true;
            }

            return AiTurnInputPolicy.CanOpenDevelopmentCards(IsAiPlayerActionsState(), AiTurnMode);
        }

        private bool CanCurrentPlayerEndTurn()
        {
            return AiTurnInputPolicy.CanEndTurn(IsAiPlayerActionsState(), AiTurnMode);
        }

        private bool IsAiManualControlBlockedState()
        {
            return IsAiPlayerActionsState() || IsAiDevelopmentCardState();
        }

        private bool IsAiPlayerActionsState()
        {
            return GetCurrentState() is PlayerActionsGameState { Player.IsAi: true };
        }

        private bool IsAiDevelopmentCardState()
        {
            return GetCurrentState() is DevelopmentCardState { Player.IsAi: true };
        }

        private void UpdateAiModeButtons()
        {
            AiAutoModeButton?.SetEnabled(AiTurnMode != AiTurnMode.Auto);
            AiManualModeButton?.SetEnabled(AiTurnMode != AiTurnMode.Manual);
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
