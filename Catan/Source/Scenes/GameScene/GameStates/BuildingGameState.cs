using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;
using System.Numerics;

public class BuildingGameState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    static UISlate BuildUISlate(Atlas atlas, Player player)
	{
        Action doNothing = () => { };
        int posX = 760;
        int posY = 350;
        UISlate buildSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 220, "Construir");    

        int i = 0;
        ButtonAction settlementButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Construir Assentamento");
        settlementButton.setEnabled(
            player.Inventory.Resources.GetAmount(ResourceId.Brick) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wood) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wool) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wheat) > 0
        );
        buildSlate.AddChild(settlementButton);
        i++;
        ButtonAction cityButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Construir Cidade");
        cityButton.setEnabled(
            player.Inventory.Resources.GetAmount(ResourceId.Ore) >= 3 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wheat) >= 2
        );
        buildSlate.AddChild(cityButton);
        i++;
        ButtonAction roadButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Construir Estrada");
        roadButton.setEnabled(
            player.Inventory.Resources.GetAmount(ResourceId.Brick) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wood) > 0
        );
        buildSlate.AddChild(roadButton);
        i++;
        ButtonAction developmentCardButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => { }, "Construir Carta de desenvolvimento");
        developmentCardButton.setEnabled(
            player.Inventory.Resources.GetAmount(ResourceId.Wood) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Ore) > 0 &&
            player.Inventory.Resources.GetAmount(ResourceId.Wheat) > 0
        );
        buildSlate.AddChild(developmentCardButton);

        return buildSlate;
    }

	public UISlate UISlate { get; private set; }
	public BuildingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
        UISlate = BuildUISlate(gameScene.Atlas, player);
        AddChild(UISlate);
    }

    public void OnPlayerTradeButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new TradingGameState(_gameScene, Player));
    }

    public void OnPlayerBuildButtonClicked()
    {
        _gameScene.ExitState();
    }

    public void OnPlayerDevelopmentCardButtonClicked()
    {
        _gameScene.ExitState();
        _gameScene.AppendState(new DevelopmentCardState(_gameScene, Player));
    }
}
