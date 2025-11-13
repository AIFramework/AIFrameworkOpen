using AI.DataStructs.Algebraic;
using System;

namespace AI.MathUtils.Algebra
{
    /// <summary>
    /// Метод Гаусса с выч. сложностью O(n^3)
    /// </summary>
    [Serializable]
    public static class Gauss
    {
        /// <summary>
        /// Решение СЛАУ методом Гаусса
        /// </summary>
        /// <param name="A">Матрица коэффициентов</param>
        /// <param name="B">Вектор свободных членов</param>
        public static Vector SolvingEquations(Matrix A, Vector B)
        {
            const double EPSILON = 1e-10; // Порог для проверки на вырожденность
            int Count = B.Count;
            Vector x = new Vector(Count);
            double coef;
            
            // Прямой ход
            for (int index = 0; index < Count; index++)
            {
                // КРИТИЧЕСКАЯ ПРОВЕРКА: диагональный элемент не должен быть близок к нулю
                if (Math.Abs(A[index, index]) < EPSILON)
                {
                    // Попытка найти строку для перестановки (частичное pivot)
                    int pivotRow = -1;
                    for (int i = index + 1; i < Count; i++)
                    {
                        if (Math.Abs(A[i, index]) > EPSILON)
                        {
                            pivotRow = i;
                            break;
                        }
                    }
                    
                    if (pivotRow == -1)
                    {
                        // Матрица вырожденная или плохо обусловленная
                        throw new InvalidOperationException(
                            $"Матрица вырожденная или плохо обусловленная. " +
                            $"Диагональный элемент [{index},{index}] = {A[index, index]} близок к нулю.");
                    }
                    
                    // Перестановка строк
                    for (int j = 0; j < Count; j++)
                    {
                        double temp = A[index, j];
                        A[index, j] = A[pivotRow, j];
                        A[pivotRow, j] = temp;
                    }
                    double tempB = B[index];
                    B[index] = B[pivotRow];
                    B[pivotRow] = tempB;
                }
                
                coef = 1.0 / A[index, index];
                A[index, index] = 1.0;
                
                for (int j = index + 1; j < Count; j++)
                {
                    A[index, j] *= coef;
                }

                B[index] *= coef;
                
                for (int k = index + 1; k < Count; k++)
                {
                    coef = A[k, index];
                    A[k, index] = 0;
                    for (int j = index + 1; j < Count; j++)
                    {
                        A[k, j] = A[k, j] - (A[index, j] * coef);
                    }

                    B[k] = B[k] - (B[index] * coef);
                }
            }
            
            // Обратный ход
            for (int index = Count - 1; index >= 0; index--)
            {
                coef = B[index];
                for (int j = index + 1; j < Count; j++)
                {
                    coef -= A[index, j] * x[j];
                }

                x[index] = coef;
            }
            
            return x;
        }

    }
}
