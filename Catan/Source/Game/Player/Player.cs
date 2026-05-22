using System;
using System.Collections;
using System.Collections.Generic;
using Catan.Source.Game;
using Catan.Source.Scenes;
using Catan.Source.Game.Resources;


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
        PlayerInventory inventory { get; private set; }
        
        public Player() : base()
        {
            inventory = new();
        }

        public override void OnSubscribe(Scene scene)
        {
            scene.Subscribe(inventory);
        }
    }

    public class PlayerInventory : GameObject
    {
        public Dictionary<ResourceId, int> resources { get; private set; }

        public PlayerInventory()
        {
            resources = new(){
                [ResourceId.Wood] = 0,
                [ResourceId.Wool] = 0,
                [ResourceId.Brick] = 0,
                [ResourceId.Ore] = 0,
                [ResourceId.Wheat] = 0,
            };
        }
    }
}
