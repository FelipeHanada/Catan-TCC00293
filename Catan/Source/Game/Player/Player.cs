using Catan.Source.Game.Inventory;


namespace Catan.Source.Game.Player
{
    public class Player : GameObject
    {
        public PlayerInventory Inventory { get; }
        public int PlayerNumber { get; }
        
        public Player(int playerNumber = 1) : base()
        {
            PlayerNumber = playerNumber;
            Inventory = new PlayerInventory();
        }
    }
}
