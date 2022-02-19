using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;

namespace AI.Combinatorics
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

    }
}
