using System;
using Catan.Source.Game.Resources;
using BoardModel = Catan.Source.Game.Board.Board;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Harbor
{
    public class HarborService
    {
        public const int DefaultTradeRate = 4;
        public const int GenericHarborTradeRate = 3;
        public const int SpecificHarborTradeRate = 2;

        private readonly BoardModel _board;

        public HarborService(BoardModel board)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board), "Tabuleiro não pode ser nulo.");
        }

        public int GetBestTradeRate(GamePlayer player, ResourceId paidResource)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player), "Jogador não pode ser nulo.");
            }

            if (HasSpecificHarbor(player, paidResource))
            {
                return SpecificHarborTradeRate;
            }

            if (HasGenericHarbor(player))
            {
                return GenericHarborTradeRate;
            }

            return DefaultTradeRate;
        }

        public bool HasGenericHarbor(GamePlayer player)
        {
            foreach (Harbor harbor in _board.Harbors)
            {
                if (harbor.Type == HarborType.Generic && harbor.GivesAccessTo(player))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasSpecificHarbor(GamePlayer player, ResourceId resource)
        {
            foreach (Harbor harbor in _board.Harbors)
            {
                if (harbor.IsSpecificFor(resource) && harbor.GivesAccessTo(player))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
