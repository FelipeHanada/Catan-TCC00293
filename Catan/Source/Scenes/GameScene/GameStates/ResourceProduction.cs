using System.Collections.Generic;
using Catan.Source.Game.Bank;
using Catan.Source.Scenes;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;


namespace Catan.Source.Scenes.Game
{
    public class ResourceProduction : GameState
    {
        private bool _rolled;
        private DiceRollControl _diceRollControl;

        public ResourceProduction(GameScene gameScene, Player player, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _diceRollControl = diceRollControl;

            _rolled = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_rolled) {
                _rolled = true;
                _gameScene.AppendState(new WaitingForDiceRoll(_gameScene, _diceRollControl));
                return;
            }

            DiceRoll roll = _gameScene.LastDiceRoll;

            if (roll.Total == 7)
            {
                // Logica do ladrao.
                _rolled = false;
                return;
            }

            ResourceProductionCalculator calculator = new(_gameScene.Board);
            var productions = calculator.CalculateExpectedProductions(roll.Total);
            var distributionRequests = new List<ResourceDistributionRequest>();

            foreach (ResourceProductionEntry production in productions)
            {
                distributionRequests.Add(new ResourceDistributionRequest(
                    production.Player.Inventory.Resources,
                    production.Resource,
                    production.Amount));
            }

            _gameScene.Bank.DistributeProduction(distributionRequests);
            _rolled = false;
        }
    }
}
