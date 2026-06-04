using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

public class DevelopmentCardState : PlayerTurnGameState
{
    public Button BuildButton { get; private set; }
    public Button TradeButton { get; private set; }
    public Button EndTurnButton { get; private set; }
    public Button DevelopmentCardButton { get; private set; }
    static UISlate BuildUISlate(GameScene gameScene, Player player)
    {
        Atlas atlas = gameScene.Atlas;
        int posX = 760;
        int posY = 350;
        UISlate useSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 220, "Usar carta");

        int i = 0;
        foreach (DevelopmentCardType type in DevelopmentCardActivationRules.ActivatableTypes)
        {
            int playableCount = player.Inventory.DevelopmentCards.CountPlayableByType(type);
            string label = $"{GetDisplayName(type)} ({playableCount})";
            ButtonAction button = new ButtonAction(
                posX + 30,
                posY + 30 + i * 45,
                atlas,
                300,
                30,
                () => ActivateDevelopmentCard(gameScene, player, type),
                label);

            button.setEnabled(DevelopmentCardActivationRules.CanActivate(
                type,
                player.Inventory.DevelopmentCards,
                gameScene.HasUsedDevelopmentCardThisTurn));

            useSlate.AddChild(button);
            i++;
        }

        return useSlate;
    }

    private static void ActivateDevelopmentCard(GameScene gameScene, Player player, DevelopmentCardType type)
    {
        if (!DevelopmentCardActivationRules.CanActivate(
            type,
            player.Inventory.DevelopmentCards,
            gameScene.HasUsedDevelopmentCardThisTurn))
        {
            Console.WriteLine($"{GetDisplayName(type)} nao pode ser usada agora.");
            return;
        }

        gameScene.MarkDevelopmentCardUsed();
        Console.WriteLine($"{GetDisplayName(type)}: efeito ainda pendente.");
        gameScene.ExitState();
        gameScene.AppendState(new DevelopmentCardState(gameScene, player));
    }

    private static string GetDisplayName(DevelopmentCardType type)
    {
        return type switch
        {
            DevelopmentCardType.Knight => "Knight",
            DevelopmentCardType.RoadBuilding => "Road building",
            DevelopmentCardType.YearOfPlenty => "Year of plenty",
            DevelopmentCardType.Monopoly => "Monopoly",
            DevelopmentCardType.VictoryPoint => "Victory point",
            _ => type.ToString(),
        };
    }

    public UISlate UISlate { get; private set; }
    public DevelopmentCardState(GameScene gameScene, Player player) : base(gameScene, player)
    {
        UISlate = BuildUISlate(gameScene, player);
        AddChild(UISlate);
        // UISlate.setEnabled(false);

        Action EnableBuildSlate = () =>
        {
            gameScene.ExitState();
            gameScene.AppendState(new BuildingGameState(gameScene, player));
        };

        Action EnableTradeSlate = () => {
            gameScene.ExitState();
            gameScene.AppendState(new TradingGameState(gameScene, player));
        };

        Action EnableDevelopmentCardState = () => {
            gameScene.ExitState();
        };

        TradeButton = new ButtonAction(840, 620, gameScene.Atlas, 75, 30, EnableTradeSlate, "Trocar");
        BuildButton = new ButtonAction(930, 620, gameScene.Atlas, 75, 30, EnableBuildSlate, "Construir");
        DevelopmentCardButton = new ButtonAction(1020, 620, gameScene.Atlas, 75, 30, EnableDevelopmentCardState, "Usar");
        EndTurnButton = new ButtonAction(880, 660, gameScene.Atlas, 175, 30, () => { }, "Terminar turno");

        AddChild(BuildButton);
        AddChild(TradeButton);
        AddChild(EndTurnButton);
        AddChild(DevelopmentCardButton);
    }
}

