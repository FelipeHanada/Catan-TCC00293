using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class ResourceProduction : GameState
    {
        private readonly int _diceNumber;

        public ResourceProduction(GameScene gameScene, int diceNumber)
            : base(gameScene)
        {
            _diceNumber = diceNumber;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ResourceProductionCalculator calculator = new(_gameScene.Board);
            var productions = calculator.CalculateExpectedProductions(_diceNumber);

            ResourceBankPlaceholder bank = new();
            bank.DistributeResources(productions);

            _gameScene.ExitState();
        }
    }
}
