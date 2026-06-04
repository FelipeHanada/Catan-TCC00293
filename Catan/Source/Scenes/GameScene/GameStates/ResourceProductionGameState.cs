using System.Collections.Generic;
using Catan.Source.Game.Bank;
using Catan.Source.Scenes;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Microsoft.Xna.Framework;


namespace Catan.Source.Scenes.Game
{
    public class ResourceProductionGameState : GameState
    {
        private bool _rolled;
        private bool _waitingForPlayerActions;
        private readonly List<Player> _players;
        private int _currentPlayerIndex;
        private readonly DiceRollControl _diceRollControl;

        public ResourceProductionGameState(GameScene gameScene, List<Player> players, DiceRollControl diceRollControl)
            : base(gameScene)
        {
            _players = players;
            _diceRollControl = diceRollControl;

            _rolled = false;
            _waitingForPlayerActions = false;
            _currentPlayerIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_waitingForPlayerActions)
            {
                AdvanceTurn();
                _waitingForPlayerActions = false;
            }

            if (!_rolled) {
                _rolled = true;
                _gameScene.AppendState(new WaitingForDiceRollGameState(_gameScene, _diceRollControl));
                return;
            }

            DiceRoll roll = _gameScene.LastDiceRoll;

            if (roll.Total == 7)
            {
                StartSevenRuleFlow();
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
            StartPlayerActions();
        }

        private void StartSevenRuleFlow()
        {
            _rolled = false;
            StartPlayerActions();
            _gameScene.AppendState(new SevenRuleGameState(_gameScene, CurrentPlayer, _players));
        }

        private void StartPlayerActions()
        {
            _waitingForPlayerActions = true;
            _gameScene.AppendState(new PlayerActionsGameState(_gameScene, CurrentPlayer));
        }

        private void AdvanceTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        private Player CurrentPlayer => _players[_currentPlayerIndex];
    }
}
