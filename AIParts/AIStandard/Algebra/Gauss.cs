using AI.DataStructs.Algebraic;
using System;

namespace AI.Algebra
{
    /// <summary>
    /// Gauss method has computational complexity O(n^3)
    /// </summary>
    [Serializable]
    public static class Gauss
    {
        /// <summary>
        /// Solve the SLAE
        /// </summary>
        /// <param name="A">Coefficient matrix</param>
        /// <param name="B">Free members vector</param>
        public static Vector SolvingEquations(Matrix A, Vector B)
        {

            int Count = B.Count;
            Vector x = new Vector(Count);
            double сoef;
            try
            {
                // Прямой ход
                for (int index = 0; index < Count; index++)
                {
                    сoef = 1 / A[index, index];
                    A[index, index] = 1;
                    for (int j = index + 1; j < Count; j++)
                    {
                        A[index, j] *= сoef;
                    }

                    B[index] *= сoef;
                    for (int k = index + 1; k < Count; k++)
                    {
                        сoef = A[k, index];
                        A[k, index] = 0;
                        for (int j = index + 1; j < Count; j++)
                        {
                            A[k, j] = A[k, j] - A[index, j] * сoef;
                        }

                        B[k] = B[k] - B[index] * сoef;
                    }
                }
            }
            catch (DivideByZeroException)
            {
                return x;
            }
            // Обратный ход
            for (int index = Count - 1; index >= 0; index--)
            {
                сoef = B[index];
                for (int j = index + 1; j < Count; j++)
                {
                    сoef -= A[index, j] * x[j];
                }

                x[index] = сoef;
            }
            return x;
        }

    }
}
