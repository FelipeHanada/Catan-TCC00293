using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Resources
{
    public class ResourceProductionEntry
    {
        public GamePlayer Player { get; }
        public ResourceId Resource { get; }
        public int Amount { get; }

        public ResourceProductionEntry(GamePlayer player, ResourceId resource, int amount)
        {
            Player = player;
            Resource = resource;
            Amount = amount;
        }
    }
}
