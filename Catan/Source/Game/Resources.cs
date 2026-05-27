using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Resources
{
    public enum ResourceId
    {
        Wood, Wool, Brick, Ore, Wheat
    }

    public class ResourceUtils
    {
        public static readonly ResourceId[] ResourceIds = [
            ResourceId.Wood, 
            ResourceId.Wool, 
            ResourceId.Brick, 
            ResourceId.Ore, 
            ResourceId.Wheat ];

        public static readonly Dictionary<ResourceId, string> ResourceName = new(){
            [ ResourceId.Wood ] = "wood",
            [ ResourceId.Wool ] = "wool",
            [ ResourceId.Brick ] = "brick",
            [ ResourceId.Ore ] = "ore",
            [ ResourceId.Wheat ] = "wheat",
        };
    }
}