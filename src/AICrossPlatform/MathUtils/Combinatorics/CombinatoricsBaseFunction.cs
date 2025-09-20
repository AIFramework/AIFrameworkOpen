using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;

namespace AI.MathUtils.Combinatorics
{
    /// <summary>
    /// Базовые функции коммбинарики
    /// </summary>
    [Serializable]
    public static class CombinatoricsBaseFunction
    {
        /// <summary>
        /// Размещение без повторов
        /// </summary>
        /// <param name="k">Количество элементов</param>
        /// <param name="n">Количество возможных позиций</param>
        public static double PlacingWithoutRepetition(int k, int n)
        {
            int newN = n + 1;
            Vector vect = FunctionsForEachElements.GenerateTheSequence(newN - k, newN);
            return Functions.Multiplication(vect);
        }
        /// <summary>
        /// Количество комбинаций
        /// </summary>
        /// <param name="k">Количество элементов</param>
        /// <param name="n">Количество возможных позиций</param>
        public static double NumberOfCombinations(int k, int n)
        {
            double Akn = PlacingWithoutRepetition(k, n);
            return Akn / FunctionsForEachElements.Factorial(k);
        }


        /// <summary>
        /// Вычисляет количество сочетаний C(n, k) итеративным методом.
        /// Метод оптимизирован для предотвращения переполнения на промежуточных шагах.
        /// </summary>
        /// <param name="n">Общее количество элементов. Должно быть >= 0.</param>
        /// <param name="k">Количество выбираемых элементов. Должно быть >= 0.</param>
        /// <returns>Целочисленное значение C(n, k) или 0, если входные данные некорректны.</returns>
        public static long Combinations(int n, int k)
        {
            // Базовые случаи и проверка на корректность
            if (k < 0 || n < 0)
            {
                return 0; // Не определено для отрицательных чисел
            }
            if (k > n)
            {
                return 0; // Нельзя выбрать больше, чем আছে
            }
            if (k == 0 || k == n)
            {
                return 1;
            }

            // Используем свойство симметрии: C(n, k) = C(n, n-k)
            // Выбираем меньшее k для уменьшения количества итераций.
            if (k > n / 2)
            {
                k = n - k;
            }

            long result = 1;
            for (int i = 1; i <= k; i++)
            {
                // result = result * (n - i + 1) / i;
                // Разделяем операции, чтобы избежать проблем с порядком вычислений для больших чисел
                result = result * (n - i + 1);
                result = result / i;
            }

            return result;
        }
    }
}
