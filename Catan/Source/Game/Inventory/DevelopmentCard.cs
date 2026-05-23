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

    public class DevelopmentCard
    {
        public DevelopmentCardType Type { get; }

        public DevelopmentCard(DevelopmentCardType type)
        {
            Type = type;
        }
    }
}
