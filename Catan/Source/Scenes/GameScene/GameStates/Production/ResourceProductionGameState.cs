using System;
using System.Collections.Generic;
using Catan.Source.Content;
using Catan.Source.Game.Bank;
using Catan.Source.Game.Inventory;
using Catan.Source.Scenes;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;


namespace Catan.Source.Scenes.Game
{
    public class ResourceProductionGameState : PlayerTurnGameState
    {
        private bool _rolled;

        public ResourceProductionGameState(GameScene gameScene, Player player)
            : base(gameScene, player)
        {
            _rolled = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_rolled) {
                _rolled = true;
                _gameScene.AppendState(new WaitingForDiceRollGameState(_gameScene, _gameScene.DiceRollControl, Player));
                return;
            }

            _gameScene.ExitState();

            DiceRoll roll = _gameScene.LastDiceRoll;
            if (roll.Total == 7)
            {
                _gameScene.Log.Add("Dado 7: ladrao ativado");
                _gameScene.AppendState(new SevenRuleGameState(_gameScene, Player));
                SoundManager.Instance.Play(SfxId.LadraoDado7);
                return;
            }

            ResourceProductionCalculator calculator = new(_gameScene.Board);
            var productions = calculator.CalculateExpectedProductions(roll.Total);
            var distributionRequests = new List<ResourceDistributionRequest>();
            var playersByInventory = new Dictionary<ResourceInventory, Player>();

            foreach (ResourceProductionEntry production in productions)
            {
                playersByInventory[production.Player.Inventory.Resources] = production.Player;
                distributionRequests.Add(new ResourceDistributionRequest(
                    production.Player.Inventory.Resources,
                    production.Resource,
                    production.Amount));
            }

            var deliveries = _gameScene.Bank.DistributeProduction(distributionRequests);

            foreach (ResourceDistributionRequest delivery in deliveries)
            {
                Player recipient = playersByInventory[delivery.RecipientInventory];
                _gameScene.Log.Add($"{recipient.DisplayName} ganhou {delivery.Amount} {GetResourceLogName(delivery.Resource)}");

                SfxId sound = GetResourceProductionSound(delivery.Resource);
                for (int i = 0; i < delivery.Amount; i++)
                {
                    SoundManager.Instance.Play(sound);
                }
            }
        }

        private static SfxId GetResourceProductionSound(ResourceId resource)
        {
            return resource switch
            {
                ResourceId.Wool => SfxId.Ovelha,
                ResourceId.Brick => SfxId.Tijolo,
                ResourceId.Ore => SfxId.Pedra,
                ResourceId.Wood => SfxId.Planta,
                ResourceId.Wheat => SfxId.Planta,
                _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, "Recurso de producao desconhecido."),
            };
        }

        private static string GetResourceLogName(ResourceId resource)
        {
            return resource switch
            {
                ResourceId.Wool => "la",
                ResourceId.Brick => "tijolo",
                ResourceId.Ore => "minerio",
                ResourceId.Wood => "madeira",
                ResourceId.Wheat => "trigo",
                _ => resource.ToString(),
            };
        }
    }
}
