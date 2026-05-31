using System.Collections.Generic;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using DiagnosticsDebug = System.Diagnostics.Debug;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Scenes.Game
{
    public class PlayerTradeOfferCreationGameState : GameState
    {
        private enum TradeSide
        {
            Offered,
            Requested,
        }

        private readonly GamePlayer _player;
        private readonly ResourceId[] _resources =
        {
            ResourceId.Wood,
            ResourceId.Brick,
            ResourceId.Wheat,
            ResourceId.Wool,
            ResourceId.Ore,
        };

        private readonly Dictionary<ResourceId, int> _offeredResources;
        private readonly Dictionary<ResourceId, int> _requestedResources;
        private KeyboardState _previousKeyboardState;
        private TradeSide _selectedSide;
        private int _selectedResourceIndex;

        public PlayerTradeOfferCreationGameState(GameScene gameScene, GamePlayer player)
            : base(gameScene)
        {
            _player = player;
            _offeredResources = CreateEmptySelection();
            _requestedResources = CreateEmptySelection();
            _previousKeyboardState = Keyboard.GetState();
            _selectedSide = TradeSide.Offered;
            _selectedResourceIndex = 0;

            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Troca entre jogadores: Jogador {_player.PlayerNumber} abriu criacao de oferta. Tab alterna lado, A/D muda recurso, Q/E muda quantidade, Enter publica, Esc cancela.");
            #endif
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsJustPressed(keyboardState, Keys.Escape))
            {
                #if DEBUG
                DiagnosticsDebug.WriteLine("Troca entre jogadores: oferta cancelada.");
                #endif

                _gameScene.ExitState();
                return;
            }

            if (IsJustPressed(keyboardState, Keys.Tab))
            {
                _selectedSide = _selectedSide == TradeSide.Offered
                    ? TradeSide.Requested
                    : TradeSide.Offered;
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.A))
            {
                _selectedResourceIndex = PreviousIndex(_selectedResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.D))
            {
                _selectedResourceIndex = NextIndex(_selectedResourceIndex);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.Q))
            {
                Dictionary<ResourceId, int> selectedResources = GetSelectedResources();
                ResourceId resource = SelectedResource;
                selectedResources[resource] = System.Math.Max(0, selectedResources[resource] - 1);
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.E))
            {
                Dictionary<ResourceId, int> selectedResources = GetSelectedResources();
                ResourceId resource = SelectedResource;
                selectedResources[resource] += 1;
                LogSelection();
            }

            if (IsJustPressed(keyboardState, Keys.Enter))
            {
                var service = new PlayerTradeService();
                Dictionary<ResourceId, int> offeredResources = BuildPositiveResources(_offeredResources);
                Dictionary<ResourceId, int> requestedResources = BuildPositiveResources(_requestedResources);
                PlayerTradeResult result = service.CreateOffer(
                    _player,
                    offeredResources,
                    requestedResources,
                    out PlayerTradeOffer offer);

                #if DEBUG
                DiagnosticsDebug.WriteLine(
                    $"Troca entre jogadores: Jogador {_player.PlayerNumber} tentou criar oferta: oferece {FormatResources(offeredResources)}; pede {FormatResources(requestedResources)}. {result.Message}");
                #endif

                if (result.Success)
                {
                    #if DEBUG
                    DiagnosticsDebug.WriteLine(
                        $"Troca entre jogadores: Jogador {_player.PlayerNumber} criou oferta: oferece {FormatResources(offer.OfferedResources)}; pede {FormatResources(offer.RequestedResources)}.");
                    #endif

                    _gameScene.AppendState(new PlayerTradeOfferResponseGameState(_gameScene, offer));
                }
            }

            _previousKeyboardState = keyboardState;
        }

        private ResourceId SelectedResource => _resources[_selectedResourceIndex];

        private Dictionary<ResourceId, int> GetSelectedResources()
        {
            return _selectedSide == TradeSide.Offered
                ? _offeredResources
                : _requestedResources;
        }

        private Dictionary<ResourceId, int> CreateEmptySelection()
        {
            var resources = new Dictionary<ResourceId, int>();
            foreach (ResourceId resource in _resources)
            {
                resources[resource] = 0;
            }

            return resources;
        }

        private Dictionary<ResourceId, int> BuildPositiveResources(Dictionary<ResourceId, int> resources)
        {
            var positiveResources = new Dictionary<ResourceId, int>();
            foreach (var resource in resources)
            {
                if (resource.Value > 0)
                {
                    positiveResources[resource.Key] = resource.Value;
                }
            }

            return positiveResources;
        }

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
            Dictionary<ResourceId, int> selectedResources = GetSelectedResources();
            DiagnosticsDebug.WriteLine(
                $"Troca entre jogadores: selecionando {_selectedSide}, recurso {SelectedResource}, quantidade {selectedResources[SelectedResource]}. Oferece {FormatResources(BuildPositiveResources(_offeredResources))}; pede {FormatResources(BuildPositiveResources(_requestedResources))}.");
            #endif
        }

        internal static string FormatResources(IReadOnlyDictionary<ResourceId, int> resources)
        {
            if (resources.Count == 0)
            {
                return "nada";
            }

            var parts = new List<string>();
            foreach (var resource in resources)
            {
                parts.Add($"{resource.Value} {resource.Key}");
            }

            return string.Join(" + ", parts);
        }
    }

    public class PlayerTradeOfferResponseGameState : GameState
    {
        private readonly PlayerTradeOffer _offer;
        private KeyboardState _previousKeyboardState;

        public PlayerTradeOfferResponseGameState(GameScene gameScene, PlayerTradeOffer offer)
            : base(gameScene)
        {
            _offer = offer;
            _previousKeyboardState = Keyboard.GetState();

            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Troca entre jogadores: oferta aberta do Jogador {_offer.OfferingPlayer.PlayerNumber}. Oferece {PlayerTradeOfferCreationGameState.FormatResources(_offer.OfferedResources)}; pede {PlayerTradeOfferCreationGameState.FormatResources(_offer.RequestedResources)}. Use 1/2/3/4 para tentar aceitar ou Esc para cancelar.");
            #endif
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsJustPressed(keyboardState, Keys.Escape))
            {
                #if DEBUG
                DiagnosticsDebug.WriteLine("Troca entre jogadores: oferta cancelada.");
                #endif

                _gameScene.ExitState();
                _gameScene.ExitState();
                return;
            }

            for (int index = 0; index < 4; index++)
            {
                if (IsPlayerKeyJustPressed(keyboardState, index))
                {
                    TryAccept(index);
                    break;
                }
            }

            _previousKeyboardState = keyboardState;
        }

        private void TryAccept(int playerIndex)
        {
            if (playerIndex >= _gameScene._players.Count)
            {
                #if DEBUG
                DiagnosticsDebug.WriteLine($"Troca entre jogadores: Jogador {playerIndex + 1} nao existe nesta partida.");
                #endif
                return;
            }

            GamePlayer acceptingPlayer = _gameScene._players[playerIndex];
            var service = new PlayerTradeService();
            PlayerTradeResult result = service.Execute(_offer, acceptingPlayer);

            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Troca entre jogadores: Jogador {acceptingPlayer.PlayerNumber} tentou aceitar oferta do Jogador {_offer.OfferingPlayer.PlayerNumber}. {result.Message}");
            #endif

            if (!result.Success)
            {
                return;
            }

            #if DEBUG
            DiagnosticsDebug.WriteLine("Troca entre jogadores: troca concluida com sucesso.");
            #endif

            _gameScene.ExitState();
            _gameScene.ExitState();
        }

        private bool IsPlayerKeyJustPressed(KeyboardState currentState, int playerIndex)
        {
            Keys digitKey = Keys.D1 + playerIndex;
            Keys numPadKey = Keys.NumPad1 + playerIndex;
            return IsJustPressed(currentState, digitKey) || IsJustPressed(currentState, numPadKey);
        }

        private bool IsJustPressed(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}
