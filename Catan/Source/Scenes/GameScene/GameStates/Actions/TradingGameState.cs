using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Harbor;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Catan.Source.Scenes.Game
{
    public class TradingGameState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    private enum TradeMode
    {
        Players,
        Bank
    }

    public UISlate UISlate { get; private set; }

    private readonly TradeMode _mode;

    public TradingGameState(GameScene gameScene, Player player) : this(gameScene, player, TradeMode.Players)
    {
    }

    private TradingGameState(GameScene gameScene, Player player, TradeMode mode) : base(gameScene, player)
    {
        _mode = mode;
        UISlate = BuildUISlate(gameScene.Atlas, player);
        AddChild(UISlate);
    }

    public void OnPlayerBuildButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new BuildingGameState(_gameScene, Player));
    }

    public void OnPlayerTradeButtonClicked()
    {
        _gameScene.ExitState();
    }

    public void OnPlayerDevelopmentCardButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new DevelopmentCardState(_gameScene, Player));
    }

    private UISlate BuildUISlate(Atlas atlas, Player player)
    {
        Action doNothing = () => { };
        UISlate tradeSlate = new UISlate(600, 200, atlas, Color.Gray, 600, 370, "Trocar");

        tradeSlate.AddChild(new ButtonAction(
            970,
            205,
            atlas,
            () => ChangeMode(TradeMode.Players),
            "Jogadores",
            _mode != TradeMode.Players));

        tradeSlate.AddChild(new ButtonAction(
            1080, 
            205,
            atlas,
            () => ChangeMode(TradeMode.Bank),
            "Banco/Porto",
            _mode != TradeMode.Bank));

        ResourceDisplay offeredResourceDisplay = new ResourceDisplay(630, 250, atlas);
        tradeSlate.AddChild(offeredResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 220, atlas, doNothing, "Voce oferece: ", false));

        ResourceDisplay requestedResourceDisplay = new ResourceDisplay(630, 400, atlas);
        tradeSlate.AddChild(requestedResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 370, atlas, doNothing, "Voce recebe: ", false));

        if (_mode == TradeMode.Players)
        {
            for (int i = 0; i < 4; i++)
            {
                int acceptingPlayerNumber = i;
                bool enabled = player.PlayerNumber != acceptingPlayerNumber;
                tradeSlate.AddChild(new ButtonAction(
                    660 + i * 90,
                    520,
                    atlas,
                    () => TryAcceptTrade(player, acceptingPlayerNumber, offeredResourceDisplay, requestedResourceDisplay),
                    "  Aceitar\n(player " + i + ")",
                    enabled));
            }
        }
        else
        {
            tradeSlate.AddChild(new ButtonAction(
                660,
                520,
                atlas,
                () => TryAcceptBankTrade(player, offeredResourceDisplay, requestedResourceDisplay),
                "Confirmar banco"));
        }

        return tradeSlate;
    }

    private void ChangeMode(TradeMode mode)
    {
        if (_mode == mode)
        {
            return;
        }

        _gameScene.ExitState();
        _gameScene.AppendState(new TradingGameState(_gameScene, Player, mode));
    }

    private void TryAcceptTrade(
        Player offeringPlayer,
        int acceptingPlayerNumber,
        ResourceDisplay offeredResourceDisplay,
        ResourceDisplay requestedResourceDisplay)
    {
        PlayerTradeService service = new();
        Dictionary<ResourceId, int> offeredResources = offeredResourceDisplay.GetSelectedResources();
        Dictionary<ResourceId, int> requestedResources = requestedResourceDisplay.GetSelectedResources();

        PlayerTradeResult createResult = service.CreateOffer(
            offeringPlayer,
            offeredResources,
            requestedResources,
            out PlayerTradeOffer offer);

        if (!createResult.Success)
        {
            LogTradeResult(createResult);
            return;
        }

        Player acceptingPlayer = _gameScene.GetPlayer(acceptingPlayerNumber);
        PlayerTradeResult executeResult = service.Execute(offer, acceptingPlayer);
        LogTradeResult(executeResult);

        if (executeResult.Success)
        {
            offeredResourceDisplay.Clear();
            requestedResourceDisplay.Clear();
        }
    }

    private void TryAcceptBankTrade(
        Player player,
        ResourceDisplay offeredResourceDisplay,
        ResourceDisplay requestedResourceDisplay)
    {
        Dictionary<ResourceId, int> offeredResources = offeredResourceDisplay.GetSelectedResources();
        Dictionary<ResourceId, int> requestedResources = requestedResourceDisplay.GetSelectedResources();

        if (!TryGetSinglePaidResource(offeredResources, out ResourceId paidResource))
        {
            LogBankTradeResult("Selecione exatamente um recurso para pagar ao banco.");
            return;
        }

        HarborService harborService = new(_gameScene.Board);
        int rate = harborService.GetBestTradeRate(player, paidResource);

        if (!BankTradeSelection.TryCreate(
            offeredResources,
            requestedResources,
            rate,
            out BankTradeSelection selection,
            out string message))
        {
            LogBankTradeResult(message);
            return;
        }

        if (!_gameScene.Bank.CanTrade(
            player.Inventory.Resources,
            selection.PaidResource,
            rate,
            selection.ReceivedResource,
            1))
        {
            LogBankTradeResult("Troca com banco inválida: recursos insuficientes do jogador ou do banco.");
            return;
        }

        _gameScene.Bank.Trade(
            player.Inventory.Resources,
            selection.PaidResource,
            rate,
            selection.ReceivedResource,
            1);

        offeredResourceDisplay.Clear();
        requestedResourceDisplay.Clear();
        LogBankTradeResult("Troca com banco realizada.");
    }

    private static bool TryGetSinglePaidResource(Dictionary<ResourceId, int> resources, out ResourceId paidResource)
    {
        paidResource = default;

        if (resources.Count != 1)
        {
            return false;
        }

        foreach (var resource in resources)
        {
            if (resource.Value <= 0)
            {
                return false;
            }

            paidResource = resource.Key;
            return true;
        }

        return false;
    }

    private static void LogTradeResult(PlayerTradeResult result)
    {
#if DEBUG
        Console.WriteLine($"Troca entre jogadores: {result.Message}");
#endif
    }

    private static void LogBankTradeResult(string message)
    {
#if DEBUG
        Console.WriteLine($"Troca com banco/porto: {message}");
#endif
    }
}
}
