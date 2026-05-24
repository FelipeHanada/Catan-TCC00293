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
                players.Add(new Player(i + 1));
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
        private readonly PlayerInventory inventory;
        public int PlayerNumber { get; }
        public PlayerInventory Inventory => inventory;
        
        public Player(int playerNumber = 1) : base()
        {
            PlayerNumber = playerNumber;
            inventory = new();
        }

        public override void OnSubscribe(Scene scene)
        {
            scene.Subscribe(inventory);
        }
    }

    public class PlayerInventory : GameObject
    {
        // Temporário pra descarte automatico enquanto não temos a escolha do jogador.
        private static readonly ResourceId[] _discardOrder = [
            ResourceId.Wood,
            ResourceId.Wool,
            ResourceId.Brick,
            ResourceId.Ore,
            ResourceId.Wheat,
        ];

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

        public void AddResource(ResourceId resource, int amount)
        {
            resources[resource] += amount;
        }

        public int GetTotalResources()
        {
            int total = 0;

            foreach (int amount in resources.Values)
            {
                total += amount;
            }

            return total;
        }

        public bool HasResources()
        {
            return GetTotalResources() > 0;
        }

        public int RemoveResource(ResourceId resource, int amount)
        {
            int removedAmount = Math.Min(resources[resource], amount);
            resources[resource] -= removedAmount;
            return removedAmount;
        }

        public Dictionary<ResourceId, int> DiscardResources(int amount)
        {
            Dictionary<ResourceId, int> discardedResources = new();
            int remainingAmount = amount;

            foreach (ResourceId resource in _discardOrder)
            {
                if (remainingAmount <= 0)
                {
                    break;
                }

                int removedAmount = RemoveResource(resource, remainingAmount);
                if (removedAmount > 0)
                {
                    discardedResources[resource] = removedAmount;
                    remainingAmount -= removedAmount;
                }
            }

            return discardedResources;
        }

        public bool TryRemoveRandomResource(out ResourceId resource)
        {
            int totalResources = GetTotalResources();
            if (totalResources == 0)
            {
                resource = default;
                return false;
            }

            int resourceIndex = Random.Shared.Next(totalResources);
            foreach (KeyValuePair<ResourceId, int> resourceAmount in resources)
            {
                if (resourceIndex < resourceAmount.Value)
                {
                    resource = resourceAmount.Key;
                    RemoveResource(resource, 1);
                    return true;
                }

                resourceIndex -= resourceAmount.Value;
            }

            resource = default;
            return false;
        }
    }
}
