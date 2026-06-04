using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

public class DevelopmentCardState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    static UISlate BuildUISlate(Atlas atlas, Player player)
    {
        Action doNothing = () => { };
        int posX = 760;
        int posY = 350;
        UISlate buildSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 220, "Construir");

        int i = 0;
        ButtonAction settlementButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Monopoly");
        settlementButton.setEnabled(
            player.Inventory.DevelopmentCards.CountByType(DevelopmentCardType.Monopoly) > 0
        );
        buildSlate.AddChild(settlementButton);
        i++;
        ButtonAction cityButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Knight");
        cityButton.setEnabled(
            player.Inventory.DevelopmentCards.CountByType(DevelopmentCardType.Knight) > 0
        );
        buildSlate.AddChild(cityButton);
        i++;
        ButtonAction roadButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Year of plenty");
        roadButton.setEnabled(
            player.Inventory.DevelopmentCards.CountByType(DevelopmentCardType.YearOfPlenty) > 0
        );
        buildSlate.AddChild(roadButton);
        i++;
        ButtonAction developmentCardButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Road building");
        developmentCardButton.setEnabled(
            player.Inventory.DevelopmentCards.CountByType(DevelopmentCardType.RoadBuilding) > 0
        );
        buildSlate.AddChild(developmentCardButton);

        return buildSlate;
    }

    public UISlate UISlate { get; private set; }
    public DevelopmentCardState(GameScene gameScene, Player player) : base(gameScene, player)
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
        _gameScene.AppendState(new TradingGameState(_gameScene, Player));
    }

    public void OnPlayerDevelopmentCardButtonClicked()
    {
        _gameScene.ExitState();
    }
}

