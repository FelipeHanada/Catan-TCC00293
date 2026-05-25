using System;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Catan.Source.Scenes.Game
{
    public class MaritimeTradeSelectionGameState : GameState
    {
        private const int ReceiveAmount = 1;

        private readonly Player _player;
        private readonly ResourceId[] _resources =
        {
            ResourceId.Wood,
            ResourceId.Brick,
            ResourceId.Wheat,
            ResourceId.Wool,
            ResourceId.Ore,
        };

        private int _giveResourceIndex;
        private int _receiveResourceIndex;
        private KeyboardState _previousKeyboardState;

        public MaritimeTradeSelectionGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            _player = player;
            _giveResourceIndex = 0;
            _receiveResourceIndex = 1;
            _previousKeyboardState = Keyboard.GetState();

            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Troca maritima: Jogador {_player.PlayerNumber} abriu selecao. Paga {SelectedGiveResource}, recebe {SelectedReceiveResource}."
            );
            #endif
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsJustPressed(keyboardState, Keys.Escape))
            {
                #if DEBUG
                DiagnosticsDebug.WriteLine($"Troca maritima: Jogador {_player.PlayerNumber} cancelou selecao.");
                #endif

                _gameScene.ExitState();
                return;
            }

            if (IsJustPressed(keyboardState, Keys.A))
            {
                _giveResourceIndex = PreviousIndex(_giveResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.D))
            {
                _giveResourceIndex = NextIndex(_giveResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.Left))
            {
                _receiveResourceIndex = PreviousIndex(_receiveResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.Right))
            {
                _receiveResourceIndex = NextIndex(_receiveResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.Enter))
            {
                HarborService harborService = new(_gameScene.Board);
                int giveAmount = harborService.GetBestTradeRate(_player, SelectedGiveResource);

                #if DEBUG
                DiagnosticsDebug.WriteLine(
                    $"Troca maritima: Jogador {_player.PlayerNumber} confirmou selecao. Taxa {giveAmount}:1, paga {SelectedGiveResource}, recebe {SelectedReceiveResource}."
                );
                #endif

                _gameScene.AppendState(new MaritimeTradeExecutionGameState(
                    _gameScene,
                    _player,
                    SelectedGiveResource,
                    giveAmount,
                    SelectedReceiveResource,
                    ReceiveAmount));
            }

            _previousKeyboardState = keyboardState;
        }

        private ResourceId SelectedGiveResource => _resources[_giveResourceIndex];
        private ResourceId SelectedReceiveResource => _resources[_receiveResourceIndex];

        private int NextIndex(int index)
        {
            return (index + 1) % _resources.Length;
        }

        private int PreviousIndex(int index)
        {
            return (index - 1 + _resources.Length) % _resources.Length;
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        private void LogSelection()
        {
            #if DEBUG
            HarborService harborService = new(_gameScene.Board);
            int giveAmount = harborService.GetBestTradeRate(_player, SelectedGiveResource);

            DiagnosticsDebug.WriteLine(
                $"Troca maritima: Jogador {_player.PlayerNumber} selecionou taxa {giveAmount}:1, paga {SelectedGiveResource}, recebe {SelectedReceiveResource}."
            );
            #endif
        }
    }

    public class MaritimeTradeExecutionGameState : GameState
    {
        private readonly Player _player;
        private readonly ResourceId _giveToBank;
        private readonly int _giveAmount;
        private readonly ResourceId _receiveFromBank;
        private readonly int _receiveAmount;

        public MaritimeTradeExecutionGameState(
            GameScene gameScene,
            Player player,
            ResourceId giveToBank,
            int giveAmount,
            ResourceId receiveFromBank,
            int receiveAmount)
            : base(gameScene)
        {
            _player = player;
            _giveToBank = giveToBank;
            _giveAmount = giveAmount;
            _receiveFromBank = receiveFromBank;
            _receiveAmount = receiveAmount;
        }

        public override void Update(GameTime gameTime)
        {
            try
            {
                if (!IsTradeRequestValid())
                {
                    LogInvalidTrade();
                    _gameScene.ExitState();
                    return;
                }

                if (!_gameScene.Bank.CanTrade(
                    _player.Inventory.Resources,
                    _giveToBank,
                    _giveAmount,
                    _receiveFromBank,
                    _receiveAmount))
                {
                    LogInvalidTrade();
                    _gameScene.ExitState();
                    return;
                }

                _gameScene.Bank.Trade(
                    _player.Inventory.Resources,
                    _giveToBank,
                    _giveAmount,
                    _receiveFromBank,
                    _receiveAmount);

                #if DEBUG
                DiagnosticsDebug.WriteLine(
                    $"Troca maritima: Jogador {_player.PlayerNumber} entregou {_giveAmount} {_giveToBank} e recebeu {_receiveAmount} {_receiveFromBank}."
                );
                #endif

                _gameScene.ExitState();
                _gameScene.ExitState();
            }
            catch (ArgumentException)
            {
                _gameScene.ExitState();
            }
            catch (InvalidOperationException)
            {
                _gameScene.ExitState();
            }
        }

        private bool IsTradeRequestValid()
        {
            return _giveAmount > 0
                && _receiveAmount > 0
                && _giveToBank != _receiveFromBank;
        }

        private void LogInvalidTrade()
        {
            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Troca maritima: Jogador {_player.PlayerNumber} tentou troca invalida. Paga {_giveAmount} {_giveToBank}, recebe {_receiveAmount} {_receiveFromBank}."
            );
            #endif
        }
    }
}
