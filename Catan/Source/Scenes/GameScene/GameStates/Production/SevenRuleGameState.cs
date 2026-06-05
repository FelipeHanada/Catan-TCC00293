using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class SevenRuleGameState : PlayerTurnGameState
    {
        private enum SevenRuleStep
        {
            DiscardResources,
            MoveRobber,
            StealResource,
            Done
        }

        private SevenRuleStep _currentStep;

        public SevenRuleGameState(GameScene gameScene, Player currentPlayer)
            : base(gameScene, currentPlayer)
        {
            _currentStep = SevenRuleStep.DiscardResources;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_currentStep)
            {
                case SevenRuleStep.DiscardResources:
                    _currentStep = SevenRuleStep.MoveRobber;
                    _gameScene.AppendState(new DiscardResourcesGameState(_gameScene));
                    break;
                case SevenRuleStep.MoveRobber:
                    _currentStep = SevenRuleStep.StealResource;
                    _gameScene.AppendState(new MoveRobberGameState(_gameScene));
                    break;
                case SevenRuleStep.StealResource:
                    _currentStep = SevenRuleStep.Done;
                    _gameScene.AppendState(new StealResourceGameState(_gameScene, Player));
                    break;
                case SevenRuleStep.Done:
                    _gameScene.ExitState();
                    break;
            }
        }
    }
}
