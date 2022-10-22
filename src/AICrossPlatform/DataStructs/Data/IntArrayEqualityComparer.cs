using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataStructs.Data
{
    /// <summary>
    /// Класс для сравнения и генерации хэшей массива целых чисел
    /// </summary>
    public class IntArrayEqualityComparer : IEqualityComparer<int[]>
    {
        /// <summary>
        /// Проверка равенства
        /// </summary>
        /// <param name="left">Левая часть равенства</param>
        /// <param name="right">Правая часть равенства</param>
        /// <returns></returns>
        public bool Equals(int[] left, int[] right)
        {
            if (left.Length != right.Length) return false;

            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i]) return false;

            return true;
        }

        /// <summary>
        /// Генерация хэша
        /// </summary>
        /// <param name="array">Массив int</param>
        /// <returns></returns>
        public int GetHashCode(int[] array)
        {
            int result = 7;
            int mZ = 20000003;
            for (int i = 0; i < array.Length; i++)
            {
                unchecked
                {
                    result = result * 11 + array[i];
                    if (result > mZ) result %= mZ;
                }
            }
            return result;
        }
    }
}
