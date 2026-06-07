using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Player;
using Catan.Source.Scenes;
using Catan.Source.Scenes.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Catan.Source.Scenes.Game
{
    public class DevelopmentCardState : PlayerTurnGameState, PlayerBuildButtonCallback, PlayerTradeButtonCallback, PlayerDevelopmentCardButtonCallback
{
    private readonly Dictionary<ButtonAction, DevelopmentCardType> _cardButtons;
    private readonly DevelopmentCardPreview _preview;

    UISlate BuildUISlate(GameScene gameScene, Player player)
    {
        Atlas atlas = gameScene.Atlas;
        int posX = 760;
        int posY = 350;
        UISlate useSlate = new UISlate(posX, posY, atlas, Color.Gray, 360, 270, "Cartas");
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

            _cardButtons[button] = type;
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
        _cardButtons = new Dictionary<ButtonAction, DevelopmentCardType>();
        UISlate = BuildUISlate(gameScene, player);
        _preview = new DevelopmentCardPreview(613, 390, gameScene.Atlas);
        AddChild(UISlate);
        AddChild(_preview);
    }

    public override void Update(GameTime gameTime)
    {
        _preview.CardType = null;
        foreach (var entry in _cardButtons)
        {
            if (entry.Key.IsHovered)
            {
                _preview.CardType = entry.Value;
                return;
            }
        }
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

    private class DevelopmentCardPreview : GameObject
    {
        private readonly Atlas _atlas;
        public DevelopmentCardType? CardType { get; set; }

        public DevelopmentCardPreview(float x, float y, Atlas atlas) : base(x, y)
        {
            _atlas = atlas;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!CardType.HasValue)
            {
                return;
            }

            spriteBatch.Draw(
                _atlas.Texture,
                new Vector2(X, Y),
                Atlas.GetRectangle(GetSpriteId(CardType.Value)),
                Color.White);
        }

        private static AtlasSpriteId GetSpriteId(DevelopmentCardType type)
        {
            return type switch
            {
                DevelopmentCardType.Invention => AtlasSpriteId.DevCardInvention,
                DevelopmentCardType.RoadBuilding => AtlasSpriteId.DevCardRoad,
                DevelopmentCardType.Monopoly => AtlasSpriteId.DevCardMonopoly,
                DevelopmentCardType.Knight => AtlasSpriteId.DevCardKnight,
                DevelopmentCardType.VictoryPoint => AtlasSpriteId.DevCardVP,
                _ => AtlasSpriteId.DevCardVP,
            };
        }
    }
}
}

