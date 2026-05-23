using System.Collections.Generic;
using Catan.Source.Game;
using Catan.Source.Game.Inventory;
using Catan.Source.Scenes;


namespace Catan.Source.Game.Player
{
    public class PlayerManager : GameObject
    {
        public List<Player> players { get; private set; }

        public PlayerManager(int numberOfPlayers) : base()
        {
            players = new();
            for (int i=0; i<numberOfPlayers; i++)
            {
                players.Add(new Player());
            }
        }

        public override void OnSubscribe(Scene scene)
        {
            foreach (Player player in players)
            {
                scene.Subscribe(player);
            }
        }
    }

    public class Player : GameObject
    {
        public PlayerInventory Inventory { get; }
        
        public Player() : base()
        {
            Inventory = new PlayerInventory();
        }
    }
}
