using System;
using Catan.Source.Content;
using Catan.Source.Game;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class YearOfPlentySelectionGameState : PlayerTurnGameState
    {
        private readonly Player _player;
        private ResourceId? _firstResource;
        private readonly DevelopmentCardActivationService _activationService;

        public UISlate UISlate { get; private set; }

        public YearOfPlentySelectionGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _player = player;
            _activationService = new DevelopmentCardActivationService();
            UISlate = BuildUISlate(gameScene.Atlas);
            AddChild(UISlate);
        }

        private UISlate BuildUISlate(Atlas atlas)
        {
            int posX = 760;
            int posY = 350;
            UISlate slate = new UISlate(posX, posY, atlas, Color.Gray, 360, 310, "Invenção");

            int buttonIndex = 0;
            foreach (ResourceId resource in Enum.GetValues<ResourceId>())
            {
                ResourceId selectedResource = resource;
                ButtonAction button = new ButtonAction(
                    posX + 30,
                    posY + 30 + buttonIndex * 40,
                    atlas,
                    300,
                    30,
                    () => SelectResource(selectedResource),
                    GetDisplayName(selectedResource));

                slate.AddChild(button);
                buttonIndex++;
            }

            ButtonAction cancelButton = new ButtonAction(
                posX + 30,
                posY + 30 + buttonIndex * 40,
                atlas,
                300,
                30,
                () => _gameScene.ExitState(),
                "Cancelar");
            slate.AddChild(cancelButton);

            return slate;
        }

        private void SelectResource(ResourceId resource)
        {
            if (!_firstResource.HasValue)
            {
                _firstResource = resource;
                Console.WriteLine($"Invenção: primeiro recurso escolhido: {GetDisplayName(resource)}.");
                return;
            }

            DevelopmentCardActivationResult result = _activationService.UseYearOfPlenty(
                _player,
                _gameScene.Bank,
                _gameScene.HasUsedDevelopmentCardThisTurn,
                _firstResource.Value,
                resource);

            Console.WriteLine(result.Message);
            if (!result.Success)
            {
                _firstResource = null;
                return;
            }

            _gameScene.MarkDevelopmentCardUsed();
            _gameScene.ExitState();
            _gameScene.ExitState();
        }

        private static string GetDisplayName(ResourceId resource)
        {
            return resource switch
            {
                ResourceId.Wood => "Wood",
                ResourceId.Wool => "Wool",
                ResourceId.Brick => "Brick",
                ResourceId.Ore => "Ore",
                ResourceId.Wheat => "Wheat",
                _ => resource.ToString(),
            };
        }
    }
}
