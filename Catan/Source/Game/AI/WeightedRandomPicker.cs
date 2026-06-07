using System;
using System.Collections.Generic;
using System.Linq;

namespace Catan.Source.Game.AI
{
    public readonly struct WeightedCandidate<T>
    {
        public T Value { get; }
        public int Weight { get; }

        public WeightedCandidate(T value, int weight)
        {
            Value = value;
            Weight = weight;
        }
    }

    public static class WeightedRandomPicker
    {
        public static T Pick<T>(IReadOnlyList<WeightedCandidate<T>> candidates, Random random)
        {
            if (candidates == null || candidates.Count == 0)
            {
                throw new InvalidOperationException("Nenhum candidato disponivel para sorteio.");
            }

            int totalWeight = candidates.Sum(candidate => Math.Max(0, candidate.Weight));
            if (totalWeight <= 0)
            {
                return candidates[random.Next(candidates.Count)].Value;
            }

            int roll = random.Next(totalWeight);
            int accumulated = 0;

            foreach (WeightedCandidate<T> candidate in candidates)
            {
                accumulated += Math.Max(0, candidate.Weight);
                if (roll < accumulated)
                {
                    return candidate.Value;
                }
            }

            return candidates[^1].Value;
        }
    }
}
