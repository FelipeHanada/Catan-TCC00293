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
	static UISlate BuildUISlate(Atlas atlas, Player player)
	{
        Action doNothing = () => {};
		UISlate tradeSlate = new UISlate(600, 200, atlas, Color.Gray, 600, 370, "Trocar");

		ResourceDisplay playerResourceDisplay = new ResourceDisplay(630,250, atlas);
		tradeSlate.AddChild(playerResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 220, atlas, doNothing, "Voce oferece: ", false));

        ResourceDisplay tradeResourceDisplay = new ResourceDisplay(630, 400, atlas);
        tradeSlate.AddChild(tradeResourceDisplay);
        tradeSlate.AddChild(new ButtonAction(660, 370, atlas, doNothing, "Voce recebe: ", false));


        for(int i = 0; i < 4; i++)
        {
            tradeSlate.AddChild(new ButtonAction(660 + i*90, 520, atlas, () => { }, "  Aceitar\n(player " + i + ")", player.PlayerNumber == i ? false : true));
        }

        return tradeSlate;
	}

	public UISlate UISlate { get; private set; }
	public TradingGameState(GameScene gameScene, Player player) : base(gameScene, player)
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
        };

        BuildButton = new ButtonAction(1000, 620, gameScene.Atlas, 75, 30, EnableBuildSlate, "Construir");
        TradeButton = new ButtonAction(900, 620, gameScene.Atlas, 75, 30, EnableTradeSlate, "Trocar");

        AddChild(BuildButton);
        AddChild(TradeButton);
        AddChild(new ButtonAction(900, 660, gameScene.Atlas, 175, 30, () => { }, "Terminar turno"));
    }
}
