using System;
using Catan.Source.Game.DevelopmentCards;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class KnightGameState : PlayerTurnGameState
    {
        private enum KnightStep
        {
            MoveRobber,
            StealResource,
            ConfirmUse,
            Done
        }

        private readonly DevelopmentCardActivationService _activationService;
        private readonly bool _exitDevelopmentCardStateWhenDone;
        private KnightStep _currentStep;

        public KnightGameState(GameScene gameScene, Player player, bool exitDevelopmentCardStateWhenDone = true)
            : base(gameScene, player)
        {
            _activationService = new DevelopmentCardActivationService();
            _exitDevelopmentCardStateWhenDone = exitDevelopmentCardStateWhenDone;
            _currentStep = KnightStep.MoveRobber;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_currentStep)
            {
                case KnightStep.MoveRobber:
                    _currentStep = KnightStep.StealResource;
                    _gameScene.AppendState(new MoveRobberGameState(_gameScene, Player));
                    break;
                case KnightStep.StealResource:
                    _currentStep = KnightStep.ConfirmUse;
                    _gameScene.AppendState(new StealResourceGameState(_gameScene, Player));
                    break;
                case KnightStep.ConfirmUse:
                    ConfirmKnightUse();
                    break;
                case KnightStep.Done:
                    _gameScene.ExitState();
                    if (_exitDevelopmentCardStateWhenDone)
                    {
                        _gameScene.ExitState();
                    }
                    break;
            }
        }

        private void ConfirmKnightUse()
        {
            _currentStep = KnightStep.Done;
            DevelopmentCardActivationResult result = _activationService.ConfirmKnightUse(
                Player,
                _gameScene.HasUsedDevelopmentCardThisTurn);

            Console.WriteLine(result.Message);
            if (result.Success)
            {
                _gameScene.MarkDevelopmentCardUsed();
            }
        }
    }
}
