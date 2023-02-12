using AI.DataStructs.Algebraic;
using System;

namespace AI.Statistics
{
    /// <summary>
    /// Случайный выбор
    /// </summary>
    /// <typeparam name="T">Тип массива элементов</typeparam>
    [Serializable]
    public class RandomItemSelection
    {
        /// <summary>
        /// Случайный выбор элемента по заданному распределению
        /// </summary>
        /// <param name="distributionFunction">Дискретная функция распределния</param>
        /// <param name="arrayStates">Массив элементов</param>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public static T GetElement<T>(Vector distributionFunction, T[] arrayStates, Random random)
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
