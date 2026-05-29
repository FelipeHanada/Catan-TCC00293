using System.Collections.Generic;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class SevenRuleGameState : GameState
    {
        private enum SevenRuleStep
        {
            DiscardResources,
            MoveRobber,
            StealResource,
            Done
        }

        private readonly Player _currentPlayer;
        private readonly List<Player> _players;
        private SevenRuleStep _currentStep;

        public SevenRuleGameState(GameScene gameScene, Player currentPlayer, List<Player> players)
            : base(gameScene)
        {
            _currentPlayer = currentPlayer;
            _players = players;
            _currentStep = SevenRuleStep.DiscardResources;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_currentStep)
            {
                case SevenRuleStep.DiscardResources:
                    _currentStep = SevenRuleStep.MoveRobber;
                    _gameScene.AppendState(new DiscardResourcesGameState(_gameScene, _players));
                    break;
                case SevenRuleStep.MoveRobber:
                    _currentStep = SevenRuleStep.StealResource;
                    _gameScene.AppendState(new MoveRobberGameState(_gameScene));
                    break;
                case SevenRuleStep.StealResource:
                    _currentStep = SevenRuleStep.Done;
                    _gameScene.AppendState(new StealResourceGameState(_gameScene, _currentPlayer));
                    break;
                case SevenRuleStep.Done:
                    _gameScene.ExitState();
                    break;
            }
        }
    }
}
