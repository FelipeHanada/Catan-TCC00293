using System.Collections.Generic;
using DiagnosticsDebug = System.Diagnostics.Debug;
using Catan.Source.Game.Board;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class StealResourceGameState : GameState
    {
        private readonly Player _currentPlayer;
        private readonly RobberRule _robberRule;

        public StealResourceGameState(GameScene gameScene, Player currentPlayer)
            : base(gameScene)
        {
            _currentPlayer = currentPlayer;
            _robberRule = new RobberRule();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Tile robberTile = _gameScene.Board.RobberTile;
            if (robberTile == null)
            {
                _gameScene.ExitState();
                return;
            }

            List<Player> targets = _robberRule.GetRobberyTargets(_currentPlayer, robberTile);
            if (targets.Count > 0)
            {
                // Futuramente, o jogador deve escolher qual alvo roubar.
                StealFromTarget(targets[0]);
            }

            _gameScene.ExitState();
        }

        private void StealFromTarget(Player target)
        {
            if (!_robberRule.TryStealRandomResource(_currentPlayer, target, out ResourceId stolenResource))
            {
                return;
            }

            #if DEBUG
            DiagnosticsDebug.WriteLine(
                $"Ladrão: Jogador {_currentPlayer.PlayerNumber} roubou {stolenResource} do jogador {target.PlayerNumber}."
            );
            #endif
        }
    }
}
