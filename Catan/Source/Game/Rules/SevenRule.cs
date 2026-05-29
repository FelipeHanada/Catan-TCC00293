using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Rules
{
    public class SevenRule
    {
        private const int MinimumResourcesToDiscard = 8;

        public bool ShouldDiscard(GamePlayer player)
        {
            return player.Inventory.GetTotalResources() >= MinimumResourcesToDiscard;
        }

        public int GetDiscardAmount(GamePlayer player)
        {
            return player.Inventory.GetTotalResources() / 2;
        }
    }
}
