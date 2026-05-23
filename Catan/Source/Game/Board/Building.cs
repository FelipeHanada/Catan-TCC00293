using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Board
{
    public enum BuildingType
    {
        Settlement,
        City,
    }

    public class Building
    {
        public GamePlayer Owner { get; }
        public BuildingType Type { get; private set; }

        public int ProductionAmount => Type == BuildingType.City ? 2 : 1;

        public Building(GamePlayer owner, BuildingType type)
        {
            Owner = owner;
            Type = type;
        }
    }
}
