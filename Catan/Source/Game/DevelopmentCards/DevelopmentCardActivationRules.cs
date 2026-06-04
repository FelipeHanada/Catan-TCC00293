using System;
using Catan.Source.Game.Inventory;

namespace Catan.Source.Game.DevelopmentCards
{
    public static class DevelopmentCardActivationRules
    {
        public static DevelopmentCardType[] ActivatableTypes { get; } =
        {
            DevelopmentCardType.Knight,
            DevelopmentCardType.RoadBuilding,
            DevelopmentCardType.YearOfPlenty,
            DevelopmentCardType.Monopoly,
        };

        public static bool CanActivate(
            DevelopmentCardType type,
            DevelopmentCardInventory inventory,
            bool hasUsedDevelopmentCardThisTurn)
        {
            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            return IsActivatableType(type)
                && !hasUsedDevelopmentCardThisTurn
                && inventory.CountPlayableByType(type) > 0;
        }

        public static bool IsActivatableType(DevelopmentCardType type)
        {
            foreach (DevelopmentCardType activatableType in ActivatableTypes)
            {
                if (activatableType == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
