using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography.X509Certificates;

public class TradingGameState : PlayerTurnGameState
{
        public Button BuildButton { get; private set; }
        public Button TradeButton { get; private set; }
	static UISlate BuildUISlate(Atlas atlas)
	{

		UISlate tradeSlate = new UISlate(600, 200, atlas, Color.Gray, 600, 300, "Trocar");

		ResourceDisplay resourceDisplay = new ResourceDisplay(600,250, atlas);
		for (int i = 0; i < 10; i++) resourceDisplay.incrementResource(ResourceId.Wool);
		for (int i = 0; i < 21; i++) resourceDisplay.incrementResource(ResourceId.Wood);
		for (int i = 0; i < 3; i++) resourceDisplay.incrementResource(ResourceId.Ore);
		for (int i = 0; i < 4; i++) resourceDisplay.incrementResource(ResourceId.Brick);
		for (int i = 0; i < 5; i++) resourceDisplay.incrementResource(ResourceId.Wheat);
		tradeSlate.AddChild(resourceDisplay);

		return tradeSlate;
	}

	public UISlate UISlate { get; private set; }
	public TradingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
		UISlate = BuildUISlate(gameScene.Atlas);
        AddChild(UISlate);
        UISlate.setEnabled(false);

        Action EnableBuildSlate = () =>
        {
            gameScene.ExitState();
            gameScene.AppendState(new BuildingGameState(gameScene, player));
        };

        Action EnableTradeSlate = () => {
                gameScene.ExitState();
        };

        BuildButton = new ButtonAction(700, 650, gameScene.Atlas, EnableBuildSlate, "Construir");
        TradeButton = new ButtonAction(600, 650, gameScene.Atlas, EnableTradeSlate, "Trocar");

        AddChild(BuildButton);
        AddChild(TradeButton);
    }
}
