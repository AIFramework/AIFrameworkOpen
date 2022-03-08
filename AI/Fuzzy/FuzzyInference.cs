using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Fuzzy
{
    /// <summary>
    /// Нечеткий вывод по аналогии
    /// </summary>
    public class FuzzyAnalogyInference
    {
        /// <summary>
        /// Матрица импликаций с применение импликации Гогена
        /// </summary>
        public static Matrix GetMatrixG(Vector @if, Vector then)
        {
            Matrix ef = new Matrix(@if.Count, then.Count);

            for (int i = 0; i < @if.Count; i++)
            {
                for (int j = 0; j < then.Count; j++)
                {
                    ef[i, j] = Fuzzy.FLV.GImplication(@if[i], then[j]);
                }
            }

            return ef;
        }

        /// <summary>
        /// Усредненная матрица импликаций с применение импликации Гогена
        /// </summary>
        public static Matrix GetMatrixG(Vector[] ifs, Vector[] thens)
        {
            Matrix ef = new Matrix(ifs[0].Count, thens[0].Count);

            for (int i = 0; i < ifs.Length; i++)
                ef += GetMatrixG(ifs[i], thens[i]);

            return ef/ ifs.Length;
        }

        /// <summary>
        /// Логический вывод
        /// </summary>
        /// <param name="matrix">Матрица импликаций</param>
        /// <param name="ifVector">Вектор условия</param>
        public static Vector Inference(Matrix matrix, Vector ifVector) 
        {
            Vector then = new Vector(matrix.Width);
            

            for (int i = 0; i < matrix.Width; i++)
            {
                Vector th = new Vector(matrix.Height);
                for (int j = 0; j < matrix.Height; j++)
                    th[j] = ifVector[j] * matrix[j, i];

                then[i] = th.Max();
            }

            return then;
        }
    }
}
