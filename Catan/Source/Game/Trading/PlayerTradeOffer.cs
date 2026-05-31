using System.Collections.Generic;
using Catan.Source.Game.Resources;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Trading
{
    public class PlayerTradeOffer
    {
        private readonly Dictionary<ResourceId, int> _offeredResources;
        private readonly Dictionary<ResourceId, int> _requestedResources;

        public GamePlayer OfferingPlayer { get; }
        public IReadOnlyDictionary<ResourceId, int> OfferedResources => _offeredResources;
        public IReadOnlyDictionary<ResourceId, int> RequestedResources => _requestedResources;
        public bool IsOpen { get; private set; }

        public PlayerTradeOffer(
            GamePlayer offeringPlayer,
            IReadOnlyDictionary<ResourceId, int> offeredResources,
            IReadOnlyDictionary<ResourceId, int> requestedResources)
        {
            OfferingPlayer = offeringPlayer;
            _offeredResources = new Dictionary<ResourceId, int>(offeredResources);
            _requestedResources = new Dictionary<ResourceId, int>(requestedResources);
            IsOpen = true;
        }

        internal void Close()
        {
            IsOpen = false;
        }
    }
}
