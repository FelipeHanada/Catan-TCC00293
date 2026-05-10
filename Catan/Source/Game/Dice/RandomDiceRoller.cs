using System;

namespace Catan.Source.Game.Dice
{
    public class RandomDiceRoller
    {
        public DiceRoll Roll()
        {
            return new DiceRoll(Random.Shared.Next(1, 7), Random.Shared.Next(1, 7));
        }
    }
}
