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

public class TradingGameState : PlayerTurnGameState
{
    public Button BuildButton { get; private set; }
    public Button TradeButton { get; private set; }
    public Button EndTurnButton { get; private set; }
    public Button DevelopmentCardButton { get; private set; }
    public UISlate UISlate { get; private set; }

    public TradingGameState(GameScene gameScene, Player player) : base(gameScene, player)
    {
        UISlate = BuildUISlate(gameScene.Atlas, player);
        AddChild(UISlate);

        Action EnableBuildSlate = () =>
        {
            gameScene.ExitState();
            gameScene.AppendState(new BuildingGameState(gameScene, player));
        };

        Action EnableTradeSlate = () =>
        {
            gameScene.ExitState();
        };

        Action EnableDevelopmentCardState = () =>
        {
            gameScene.ExitState();
            gameScene.AppendState(new DevelopmentCardState(gameScene, player));
        };

        TradeButton = new ButtonAction(840, 620, gameScene.Atlas, 75, 30, EnableTradeSlate, "Trocar");
        BuildButton = new ButtonAction(930, 620, gameScene.Atlas, 75, 30, EnableBuildSlate, "Construir");
        DevelopmentCardButton = new ButtonAction(1020, 620, gameScene.Atlas, 75, 30, EnableDevelopmentCardState, "Usar");
        EndTurnButton = new ButtonAction(880, 660, gameScene.Atlas, 175, 30, gameScene.EndCurrentPlayerActions, "Terminar turno");

        AddChild(BuildButton);
        AddChild(TradeButton);
        AddChild(EndTurnButton);
        AddChild(DevelopmentCardButton);
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
