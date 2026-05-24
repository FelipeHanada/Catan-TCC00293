using System;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DiagnosticsDebug = System.Diagnostics.Debug;

namespace Catan.Source.Scenes.Game
{
    public class BankTradeSelectionGameState : GameState
    {
        private const int GiveAmount = 4;
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

        public BankTradeSelectionGameState(GameScene gameScene, Player player)
            : base(gameScene)
        {
            _player = player;
            _giveResourceIndex = 0;
            _receiveResourceIndex = 1;
            _previousKeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsJustPressed(keyboardState, Keys.Escape))
            {
                _gameScene.ExitState();
                return;
            }

            if (IsJustPressed(keyboardState, Keys.A))
            {
                _giveResourceIndex = PreviousIndex(_giveResourceIndex);
            }

            if (IsJustPressed(keyboardState, Keys.D))
            {
                _giveResourceIndex = NextIndex(_giveResourceIndex);
            }

            if (IsJustPressed(keyboardState, Keys.Left))
            {
                _receiveResourceIndex = PreviousIndex(_receiveResourceIndex);
            }

            if (IsJustPressed(keyboardState, Keys.Right))
            {
                _receiveResourceIndex = NextIndex(_receiveResourceIndex);
            }

            if (IsJustPressed(keyboardState, Keys.Enter))
            {
                _gameScene.AppendState(new BankTradeExecutionGameState(
                    _gameScene,
                    _player,
                    SelectedGiveResource,
                    GiveAmount,
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
    }

    public class BankTradeExecutionGameState : GameState
    {
        private readonly Player _player;
        private readonly ResourceId _giveToBank;
        private readonly int _giveAmount;
        private readonly ResourceId _receiveFromBank;
        private readonly int _receiveAmount;

        public BankTradeExecutionGameState(
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
                if (!_gameScene.Bank.CanTrade(
                    _player.Inventory.Resources,
                    _giveToBank,
                    _giveAmount,
                    _receiveFromBank,
                    _receiveAmount))
                {
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
                    $"Troca com banco: Jogador {_player.PlayerNumber} entregou {_giveAmount} {_giveToBank} e recebeu {_receiveAmount} {_receiveFromBank}."
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
    }
}
