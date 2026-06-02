using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

public class BuildingGameState : PlayerTurnGameState
{
    public Button BuildButton { get; private set; }
    public Button TradeButton { get; private set; }
    static UISlate BuildUISlate(Atlas atlas)
	{
		UISlate buildSlate = new UISlate(600, 0, atlas, Color.Brown, 200, 200, "Contruir");
		buildSlate.setEnabled(false);

		return buildSlate;
	}

	public UISlate UISlate { get; private set; }
	public BuildingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
		UISlate = BuildUISlate(gameScene.Atlas);
		AddChild(UISlate);

        Action EnableBuildSlate = () =>
        {
            gameScene.ExitState();
        };

        Action EnableTradeSlate = () => {
            gameScene.ExitState();
            gameScene.AppendState(new TradingGameState(gameScene, player));
        };

        BuildButton = new ButtonAction(700, 650, gameScene.Atlas, EnableBuildSlate, "Construir");
        TradeButton = new ButtonAction(600, 650, gameScene.Atlas, EnableTradeSlate, "Trocar");

        AddChild(BuildButton);
        AddChild(TradeButton);
    }
}
