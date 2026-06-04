using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
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
    static UISlate BuildUISlate(Atlas atlas, Player player)
    {
        Action doNothing = () => { };
        int posX = 760;
        int posY = 350;
        UISlate buildSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 220, "Construir");

        int i = 0;
        ButtonAction settlementButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Monopoly");
        settlementButton.setEnabled(
            player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly) > 0
        );
        buildSlate.AddChild(settlementButton);
        i++;
        ButtonAction cityButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Knight");
        cityButton.setEnabled(
            player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Knight) > 0
        );
        buildSlate.AddChild(cityButton);
        i++;
        ButtonAction roadButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Year of plenty");
        roadButton.setEnabled(
            player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty) > 0
        );
        buildSlate.AddChild(roadButton);
        i++;
        ButtonAction developmentCardButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Road building");
        developmentCardButton.setEnabled(
            player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.RoadBuilding) > 0
        );
        buildSlate.AddChild(developmentCardButton);

        return buildSlate;
    }

    public UISlate UISlate { get; private set; }
    public DevelopmentCardState(GameScene gameScene, Player player) : base(gameScene, player)
    {
        UISlate = BuildUISlate(gameScene.Atlas, player);
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

