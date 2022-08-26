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
        /// Random selection of index with a given distribution function
        /// </summary>
        /// <param name="distributionFunction">Samples of a discrete distribution function</param>
        /// <param name="random">Random number generator</param>
        public static int GetIndex(Vector distributionFunction, Random random)
        {
            while (true)
            {
                int index = random.Next(distributionFunction.Count);
                if (random.NextDouble() < distributionFunction[index])
                {
                    return index;
                }
            }
        }
    }
}
