using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;

public class TradingGameState : PlayerTurnGameState
{
    public Button BuildButton { get; private set; }
    public Button TradeButton { get; private set; }
	static UISlate BuildUISlate(Atlas atlas)
	{
        Action doNothing = () => {};
		UISlate tradeSlate = new UISlate(600, 200, atlas, Color.Gray, 600, 300, "Trocar");

		ResourceDisplay playerResourceDisplay = new ResourceDisplay(630,250, atlas);
		tradeSlate.AddChild(playerResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(630, 220, atlas, doNothing, "Voce oferece: ", false));

        ResourceDisplay tradeResourceDisplay = new ResourceDisplay(630, 390, atlas);
        tradeSlate.AddChild(tradeResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(630, 360, atlas, doNothing, "Voce recebe: ", false));

        return tradeSlate;
	}

	public UISlate UISlate { get; private set; }
	public TradingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
		UISlate = BuildUISlate(gameScene.Atlas);
        AddChild(UISlate);
        // UISlate.setEnabled(false);

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
