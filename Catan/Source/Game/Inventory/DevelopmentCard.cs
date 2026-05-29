using System;

namespace Catan.Source.Game.Inventory
{
    public enum DevelopmentCardType
    {
        Knight,
        VictoryPoint,
        RoadBuilding,
        YearOfPlenty,
        Monopoly,
    }

    public class DevelopmentCard : IEquatable<DevelopmentCard>
    {
        public DevelopmentCardType Type { get; }

        public DevelopmentCard(DevelopmentCardType type)
        {
            Type = type;
        }

        public bool Equals(DevelopmentCard other)
        {
            return other != null && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DevelopmentCard);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }
    }
}
