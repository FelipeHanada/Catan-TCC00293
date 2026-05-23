namespace Catan.Source.Game.Inventory
{
    public class PlayerInventory
    {
        public ResourceInventory Resources { get; }
        public DevelopmentCardInventory DevelopmentCards { get; }
        public int TotalResourceCards => Resources.TotalCards();
        public int TotalDevelopmentCards => DevelopmentCards.Count;

        public PlayerInventory()
        {
            Resources = new ResourceInventory();
            DevelopmentCards = new DevelopmentCardInventory();
        }
    }
}
