using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AI.Extensions
{
    /// <summary>
    /// Extensions for collections shuffling
    /// </summary>
    public static class ShuffleExtensions
    {
        /// <summary>
        /// Shuffle an array using the Knuth method
        /// </summary>
        /// <param name="data">Array</param>
        /// <param name="seed">Random number generator seed</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this T[] data, int? seed = null)
        {
            int size = data.Length;
            Random random = seed == null ? new Random() : new Random(seed.Value);

            for (int i = 0; i < size; i++)
            {
                int index = random.Next(i + 1);
                T mid = data[index];
                data[index] = data[i];
                data[i] = mid;
            }
        }
        /// <summary>
        /// Shuffle a list using the Knuth method
        /// </summary>
        /// <param name="data">List</param>
        /// <param name="seed">Random number generator seed</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> data, int? seed = null)
        {
            int size = data.Count;
            Random random = seed==null ? new Random() : new Random(seed.Value);

            for (int i = 0; i < size; i++)
            {
                int index = random.Next(i + 1);
                T mid = data[index];
                data[index] = data[i];
                data[i] = mid;
            }
        }
    }
}