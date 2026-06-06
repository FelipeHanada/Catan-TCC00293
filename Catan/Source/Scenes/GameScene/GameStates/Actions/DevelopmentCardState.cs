using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

namespace Catan.Source.Scenes.Game
{
    public class DevelopmentCardState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    static UISlate BuildUISlate(GameScene gameScene, Player player)
    {
        Atlas atlas = gameScene.Atlas;
        int posX = 760;
        int posY = 350;
        UISlate useSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 270, "Usar carta");
        DevelopmentCardType[] displayTypes =
        {
            DevelopmentCardType.Knight,
            DevelopmentCardType.RoadBuilding,
            DevelopmentCardType.Invention,
            DevelopmentCardType.Monopoly,
            DevelopmentCardType.VictoryPoint,
        };

        int buttonIndex = 0;
        foreach (DevelopmentCardType type in displayTypes)
        {
            bool isPassive = type == DevelopmentCardType.VictoryPoint;
            int count = isPassive
                ? player.Inventory.DevelopmentCards.CountByType(type)
                : player.Inventory.DevelopmentCards.CountPlayableByType(type);
            string label = isPassive
                ? $"{GetDisplayName(type)} ({count}) [Passiva]"
                : $"{GetDisplayName(type)} ({count})";
            ButtonAction button = new ButtonAction(
                posX + 30,
                posY + 30 + buttonIndex * 45,
                atlas,
                300,
                30,
                isPassive ? () => { } : () => ActivateDevelopmentCard(gameScene, player, type),
                label);

            button.SetEnabled(!isPassive && DevelopmentCardActivationRules.CanActivate(
                    type,
                    player.Inventory.DevelopmentCards,
                    gameScene.HasUsedDevelopmentCardThisTurn));

            useSlate.AddChild(button);
            buttonIndex++;
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
            gameScene.Log.Add($"{GetDisplayName(type)} nao pode ser usada agora");
            return;
        }

        if (type == DevelopmentCardType.Invention)
        {
            gameScene.Log.Add($"Jogador {player.PlayerNumber} usou Invention");
            gameScene.AppendState(new InventionSelectionGameState(gameScene, player));
            return;
        }

        if (type == DevelopmentCardType.Monopoly)
        {
            gameScene.Log.Add($"Jogador {player.PlayerNumber} usou Monopoly");
            gameScene.AppendState(new MonopolySelectionGameState(gameScene, player));
            return;
        }

        if (type == DevelopmentCardType.Knight)
        {
            gameScene.Log.Add($"Jogador {player.PlayerNumber} usou Knight");
            gameScene.AppendState(new KnightGameState(gameScene, player));
            return;
        }

        gameScene.Log.Add($"{GetDisplayName(type)}: efeito pendente");
    }

    private static string GetDisplayName(DevelopmentCardType type)
    {
        return type switch
        {
            DevelopmentCardType.Knight => "Knight",
            DevelopmentCardType.RoadBuilding => "Road building",
            DevelopmentCardType.Invention => "Invention",
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
}

