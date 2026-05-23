namespace Catan.Source.Game.Dice
{
    public readonly struct DiceRoll
    {
        public int First { get; }
        public int Second { get; }
        public int Total => First + Second;

        public DiceRoll(int first, int second)
        {
            if (first < 1 || first > 6)
            {
                throw new System.ArgumentOutOfRangeException(nameof(first), "First die must be between 1 and 6.");
            }

            if (second < 1 || second > 6)
            {
                throw new System.ArgumentOutOfRangeException(nameof(second), "Second die must be between 1 and 6.");
            }

            First = first;
            Second = second;
        }
    }
}
