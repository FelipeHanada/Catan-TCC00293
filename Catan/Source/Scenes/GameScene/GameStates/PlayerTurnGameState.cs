using System.Collections.Generic;
using Catan.Source.Game.Dice;
using Catan.Source.Game.Player;
using Microsoft.Xna.Framework;
// Estado temporário para testar melhor o fluxo com o ladrão, mas ainda não representa o turno completo do jogo.
namespace Catan.Source.Scenes.Game
{
    public class PlayerTurnGameState : GameState
    {
        private const int RobberDiceTotal = 7;

        private enum TurnStep
        {
            RollDice,
            ResolveDiceRoll,
            Done
        }

        private readonly Player _currentPlayer;
        private readonly List<Player> _players;
        private readonly DiceRollControl _diceRollControl;
        private TurnStep _currentStep;

        public PlayerTurnGameState(
            GameScene gameScene,
            Player currentPlayer,
            List<Player> players,
            DiceRollControl diceRollControl
        )
            : base(gameScene)
        {
            _currentPlayer = currentPlayer;
            _players = players;
            _diceRollControl = diceRollControl;
            _currentStep = TurnStep.RollDice;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_currentStep)
            {
                case TurnStep.RollDice:
                    _currentStep = TurnStep.ResolveDiceRoll;
                    _gameScene.AppendState(new WaitingForDiceRoll(_gameScene, _diceRollControl));
                    break;
                case TurnStep.ResolveDiceRoll:
                    ResolveDiceRoll(_gameScene.LastDiceRoll);
                    break;
                case TurnStep.Done:
                    // Temporário enquanto ainda não existe o fluxo completo de ações/fim de turno.
                    _currentStep = TurnStep.RollDice;
                    break;
            }
        }

        private void ResolveDiceRoll(DiceRoll roll)
        {
            _currentStep = TurnStep.Done;

            if (roll.Total == RobberDiceTotal)
            {
                _gameScene.AppendState(new SevenRuleGameState(_gameScene, _currentPlayer, _players));
                return;
            }

            _gameScene.AppendState(new ResourceProduction(_gameScene, roll.Total));
        }
    }
}
