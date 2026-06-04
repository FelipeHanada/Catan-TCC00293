using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Trading;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

public class TradingGameState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    public UISlate UISlate { get; private set; }

    public TradingGameState(GameScene gameScene, Player player) : base(gameScene, player)
    {
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

        ResourceDisplay offeredResourceDisplay = new ResourceDisplay(630, 250, atlas);
        tradeSlate.AddChild(offeredResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 220, atlas, doNothing, "Voce oferece: ", false));

        ResourceDisplay requestedResourceDisplay = new ResourceDisplay(630, 400, atlas);
        tradeSlate.AddChild(requestedResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 370, atlas, doNothing, "Voce recebe: ", false));

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

        return tradeSlate;
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

    private static void LogTradeResult(PlayerTradeResult result)
    {
#if DEBUG
        Console.WriteLine($"Troca entre jogadores: {result.Message}");
#endif
    }
}
