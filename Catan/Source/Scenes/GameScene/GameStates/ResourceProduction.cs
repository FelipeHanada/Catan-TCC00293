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
        private Player _player;
        private DiceRollControl _diceRollControl;

        public ResourceProduction(GameScene gameScene, Player player, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _player = player;
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

            ResourceProductionCalculator calculator = new(_gameScene.Board);
            var productions = calculator.CalculateExpectedProductions(roll.Total);

            ResourceBankPlaceholder bank = new();
            bank.DistributeResources(productions);
        }
    }
}
