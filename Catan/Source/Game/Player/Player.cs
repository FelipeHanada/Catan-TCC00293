using System;
using System.Collections;
using System.Collections.Generic;
using Catan.Source.Game;
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

        void override OnSubscribe()
        {

        }
    }

    public class Player : GameObject
    {
        public Dictionary<ResourceId, int> resources { get; private set; }

        public Player() : base()
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
