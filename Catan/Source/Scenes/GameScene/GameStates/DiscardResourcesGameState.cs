using System.Collections.Generic;
using DiagnosticsDebug = System.Diagnostics.Debug;
using Catan.Source.Game.Player;
using Catan.Source.Game.Resources;
using Catan.Source.Game.Rules;
using Microsoft.Xna.Framework;

namespace Catan.Source.Scenes.Game
{
    public class DiscardResourcesGameState : GameState
    {
        private readonly List<Player> _players;
        private readonly SevenRule _sevenRule;

        public DiscardResourcesGameState(GameScene gameScene, List<Player> players)
            : base(gameScene)
        {
            _players = players;
            _sevenRule = new SevenRule();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Player player in _players)
            {
                if (!_sevenRule.ShouldDiscard(player))
                {
                    continue;
                }

                int amountToDiscard = _sevenRule.GetDiscardAmount(player);
                // Futuramente, o jogador deve escolher quais cartas descartar.
                Dictionary<ResourceId, int> discardedResources = player.Inventory.DiscardResources(amountToDiscard);

                #if DEBUG
                foreach (KeyValuePair<ResourceId, int> discardedResource in discardedResources)
                {
                    DiagnosticsDebug.WriteLine(
                        $"Regra do 7: Jogador {player.PlayerNumber} descartou {discardedResource.Value} {discardedResource.Key}."
                    );
                }
                #endif
            }

            _gameScene.ExitState();
        }
    }
}
