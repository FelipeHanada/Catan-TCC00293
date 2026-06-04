using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Catan.Source.Game.Board;

public class BuildingGameState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    private static readonly Dictionary<ResourceId, int> SETTLEMENT_COST = new Dictionary<ResourceId, int>
    {
        { ResourceId.Brick, 1 },
        { ResourceId.Wood, 1 },
        { ResourceId.Wool, 1 },
        { ResourceId.Wheat, 1 }
    };

    private static readonly Dictionary<ResourceId, int> CITY_COST = new Dictionary<ResourceId, int>
    {
        { ResourceId.Ore, 3 },
        { ResourceId.Wheat, 2 }
    };

    private static readonly Dictionary<ResourceId, int> ROAD_COST = new Dictionary<ResourceId, int>
    {
        { ResourceId.Brick, 1 },
        { ResourceId.Wood, 1 }
    };

    private UISlate BuildUISlate()
	{
        Atlas atlas = _gameScene.Atlas;
        Action doNothing = () => { };
        int posX = 760;
        int posY = 350;
        UISlate buildSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 220, "Construir");    

        int i = 0;
        ButtonAction settlementButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, OnBuildSettlement, "Construir Assentamento");
        settlementButton.setEnabled(Player.Inventory.Resources.HasEnough(SETTLEMENT_COST));
        buildSlate.AddChild(settlementButton);
        i++;
        ButtonAction cityButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, OnBuildCity, "Construir Cidade");
        cityButton.setEnabled(Player.Inventory.Resources.HasEnough(CITY_COST));
        buildSlate.AddChild(cityButton);
        i++;
        ButtonAction roadButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, OnBuildRoad, "Construir Estrada");
        roadButton.setEnabled(Player.Inventory.Resources.HasEnough(ROAD_COST));
        buildSlate.AddChild(roadButton);
        i++;
        DevelopmentCardPurchaseService purchaseService = new();
        ButtonAction developmentCardButton = new ButtonAction(posX + 30, posY + 30 + i * 45, atlas, 300, 30, () => {
            DevelopmentCardPurchaseResult result = purchaseService.Purchase(Player, _gameScene.Bank, _gameScene.DevelopmentCardDeck);
            Console.WriteLine(result.Message);
            if (result.Success)
            {
                Console.WriteLine($"Carta comprada: {result.Card.Type}");
                _gameScene.ExitState();
                _gameScene.AppendState(new BuildingGameState(_gameScene, Player));
            }
        }, "Construir Carta de desenvolvimento");
        developmentCardButton.setEnabled(purchaseService.CanPurchase(Player, _gameScene.Bank, _gameScene.DevelopmentCardDeck).Success);
        buildSlate.AddChild(developmentCardButton);

        return buildSlate;
    }

	public UISlate UISlate { get; private set; }
	public BuildingGameState(GameScene gameScene, Player player) : base(gameScene, player)
	{
        UISlate = BuildUISlate();
        AddChild(UISlate);
    }

    private void OnBuildSettlement() {
        _gameScene.AppendState(new BuildPositionSettlementGameState(
            _gameScene, Player, BuildingType.Settlement, SETTLEMENT_COST
        ));
    }

    private void OnBuildCity() {
        _gameScene.AppendState(new BuildPositionSettlementGameState(
            _gameScene, Player, BuildingType.City, CITY_COST
        ));
    }

    private void OnBuildRoad() {
        _gameScene.AppendState(new BuildPositionRoadGameState(
            _gameScene, Player, ROAD_COST
        ));
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
