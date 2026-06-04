using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Resources;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Rules
{
    public class RobberRule
    {
        public List<GamePlayer> GetRobberyTargets(GamePlayer currentPlayer, Tile tile)
        {
            List<GamePlayer> targets = [];

            foreach (Building building in tile.GetAdjacentBuildings())
            {
                GamePlayer target = building.Owner;
                if (ReferenceEquals(target, currentPlayer) ||
                    !target.Inventory.HasResources() ||
                    targets.Contains(target))
                {
                    continue;
                }

                targets.Add(target);
            }

            return targets;
        }

        public bool TryStealRandomResource(GamePlayer currentPlayer, GamePlayer target, out ResourceId stolenResource)
        {
            if (!target.Inventory.TryRemoveRandomResource(out stolenResource))
            {
                return false;
            }

            currentPlayer.Inventory.AddResource(stolenResource, 1);
            return true;
        }
    }
}
