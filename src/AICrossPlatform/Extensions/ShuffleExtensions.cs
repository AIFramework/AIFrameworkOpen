using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AI.Extensions
{
    /// <summary>
    /// Расширение для алгоритма перемешивания данных
    /// </summary>
    public static class ShuffleExtensions
    {
        /// <summary>
        /// Перемешивание данных в массиве методом Кнута
        /// </summary>
        /// <param name="data">Массив</param>
        /// <param name="seed">Seed для ГПСЧ</param>
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
        /// Перемешивание списка методом Кнута
        /// </summary>
        /// <param name="data">Список</param>
        /// <param name="seed">Seed для генератора псевдо-случайных чисел</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> data, int? seed = null)
        {
            int size = data.Count;
            Random random = seed == null ? new Random() : new Random(seed.Value);

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