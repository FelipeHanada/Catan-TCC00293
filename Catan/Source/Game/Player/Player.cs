using Catan.Source.Game.Inventory;

namespace Catan.Source.Game.Player
{
    public class Player : GameObject
    {
        public PlayerInventory Inventory { get; }
        public int PlayerNumber { get; }
        public string Name { get; }
        public bool IsAi { get; }
        public string DisplayName => IsAi ? $"IA {PlayerNumber + 1}" : Name;

        public Player(int playerNumber = 1, string name = null, bool isAi = false)
            : base()
        {
            PlayerNumber = playerNumber;
            Name = name ?? $"Jogador {playerNumber + 1}";
            IsAi = isAi;
            Inventory = new PlayerInventory();
        }
    }
}
