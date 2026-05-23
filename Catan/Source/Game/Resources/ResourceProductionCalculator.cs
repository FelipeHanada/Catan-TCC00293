using System.Collections.Generic;
using Catan.Source.Game.Board;
using BoardModel = Catan.Source.Game.Board.Board;

namespace Catan.Source.Game.Resources
{
    public class ResourceProductionCalculator
    {
        private readonly BoardModel _board;

        public ResourceProductionCalculator(BoardModel board)
        {
            _board = board;
        }

        public List<ResourceProductionEntry> CalculateExpectedProductions(int diceNumber)
        {
            List<ResourceProductionEntry> productions = new();

            foreach (Tile tile in _board.GetProducingTiles(diceNumber))
            {
                ResourceId resource = tile.ProducedResource.Value;

                foreach (Building building in tile.GetAdjacentBuildings())
                {
                    productions.Add(new ResourceProductionEntry(
                        building.Owner,
                        resource,
                        building.ProductionAmount
                    ));
                }
            }

            return productions;
        }
    }
}
