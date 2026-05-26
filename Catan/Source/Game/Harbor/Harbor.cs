using System;
using System.Collections.Generic;
using Catan.Source.Game.Board;
using Catan.Source.Game.Resources;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Harbor
{
    public enum HarborType
    {
        Generic,
        Specific,
    }

    public class Harbor
    {
        private readonly List<TileVertex> _vertices;

        public HarborType Type { get; }
        public ResourceId? Resource { get; }
        public IReadOnlyList<TileVertex> Vertices => _vertices;

        private Harbor(HarborType type, ResourceId? resource, IEnumerable<TileVertex> vertices)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException(nameof(vertices), "Vértices do porto não podem ser nulos.");
            }

            Type = type;
            Resource = resource;
            _vertices = new List<TileVertex>();

            foreach (TileVertex vertex in vertices)
            {
                if (vertex == null)
                {
                    throw new ArgumentException("Porto não pode conter vértice nulo.", nameof(vertices));
                }

                _vertices.Add(vertex);
            }

            if (_vertices.Count == 0)
            {
                throw new ArgumentException("Porto deve estar conectado a pelo menos um vértice.", nameof(vertices));
            }
        }

        public static Harbor CreateGeneric(IEnumerable<TileVertex> vertices)
        {
            return new Harbor(HarborType.Generic, null, vertices);
        }

        public static Harbor CreateSpecific(ResourceId resource, IEnumerable<TileVertex> vertices)
        {
            return new Harbor(HarborType.Specific, resource, vertices);
        }

        public bool GivesAccessTo(GamePlayer player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player), "Jogador não pode ser nulo.");
            }

            foreach (TileVertex vertex in _vertices)
            {
                if (vertex.HasBuilding && vertex.Building.Owner.PlayerNumber == player.PlayerNumber)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsSpecificFor(ResourceId resource)
        {
            return Type == HarborType.Specific && Resource == resource;
        }
    }
}
