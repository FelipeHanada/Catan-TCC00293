using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

public class DevelopmentCardState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
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

        if (type == DevelopmentCardType.YearOfPlenty)
        {
            gameScene.AppendState(new YearOfPlentySelectionGameState(gameScene, player));
            return;
        }

        if (type == DevelopmentCardType.Monopoly)
        {
            gameScene.AppendState(new MonopolySelectionGameState(gameScene, player));
            return;
        }

        if (type == DevelopmentCardType.Knight)
        {
            gameScene.AppendState(new KnightGameState(gameScene, player));
            return;
        }

        Console.WriteLine($"{GetDisplayName(type)}: efeito ainda pendente.");
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
    }

    public void OnPlayerBuildButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new BuildingGameState(_gameScene, Player));
    }

    public void OnPlayerTradeButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new TradingGameState(_gameScene, Player));
    }

    public void OnPlayerDevelopmentCardButtonClicked()
    {
        _gameScene.ExitState();
    }
}

