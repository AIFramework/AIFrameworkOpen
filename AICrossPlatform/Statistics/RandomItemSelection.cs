using AI.DataStructs.Algebraic;
using System;

namespace AI.Statistics
{
    /// <summary>
    /// Random selection of items
    /// </summary>
    /// <typeparam name="T">Array type</typeparam>
    [Serializable]
    public class RandomItemSelection<T>
    {
        /// <summary>
        /// Random selection of elements with a given distribution function
        /// </summary>
        /// <param name="distributionFunction">Samples of a discrete distribution function</param>
        /// <param name="arrayStates">Array of states</param>
        /// <param name="random">Random number generator</param>
        public static T GetElement(Vector distributionFunction, T[] arrayStates, Random random)
        {
            while (true)
            {
                int index = random.Next(arrayStates.Length);
                if (random.NextDouble() < distributionFunction[index])
                {
                    return arrayStates[index];
                }
            }
        }

        /// <summary>
        /// Случайный выбор индекса по заданному распределению
        /// </summary>
        public static int GetIndex(Vector distributionFunction, Random random, double t = 1)
        {
            Vector d = t != 1? distributionFunction.Transform(x => Math.Pow(x, 1.0/t)): distributionFunction;

            while (true)
            {
                int index = random.Next(d.Count);
                if (random.NextDouble() < d[index])
                {
                    return index;
                }
            }
        }
    }
}
