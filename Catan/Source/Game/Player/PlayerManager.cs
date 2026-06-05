using System.Collections.Generic;
using Catan.Source.Scenes;


namespace Catan.Source.Game.Player
{
    public class PlayerManager : GameObject
    {
        public List<Player> Players { get; private set; }

        public PlayerManager(int numberOfPlayers) : base()
        {
            Players = new();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                Players.Add(new Player(i));
            }
        }

        public override void OnSubscribe(Scene scene)
        {
            foreach (Player player in Players)
            {
                scene.Subscribe(player);
            }
        }
    }
}
