using Catan.Source.Game.Resources;

namespace Catan.Source.Game.AI
{
    public class AiTradeNeed
    {
        public ResourceId Resource { get; }
        public int Weight { get; }
        public double AttemptChance { get; }
        public bool PreferBank { get; }
        public int PlayerOfferAmount { get; }

        public AiTradeNeed(ResourceId resource, int weight, double attemptChance, bool preferBank, int playerOfferAmount = 1)
        {
            Resource = resource;
            Weight = weight;
            AttemptChance = attemptChance;
            PreferBank = preferBank;
            PlayerOfferAmount = playerOfferAmount;
        }
    }
}
