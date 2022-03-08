using AI.DataStructs.Algebraic;
using AI.Statistics;
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
        public static Matrix GetImplicationMatrixG(Vector @if, Vector then)
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
        public static Matrix GetImplicationMatrixG(IEnumerable<Vector> ifEn, IEnumerable<Vector> thenEn)
        {
            var ifs = ifEn.ToArray();
            var thens = thenEn.ToArray();

            Matrix ef = new Matrix(ifs[0].Count(), thens[0].Count);

            for (int i = 0; i < ifs.Length; i++)
                ef += GetImplicationMatrixG(ifs[i], thens[i]);

            return ef / ifs.Length;
        }

        /// <summary>
        /// Усредненная матрица импликаций, обучение с подкреплением
        /// </summary>
        public static Matrix GetImplicationMatrix(IEnumerable<Matrix> impl, Vector reward, double q = 0.5)
        {
            Matrix[] matrices = impl.ToArray();
            int n = 0;
            Matrix ef = new Matrix(matrices[0].Height, matrices[0].Width);
            Quantile quantile = new Quantile(reward);
            double treshold = quantile.GetQuantile(q);

            for (int i = 0; i < reward.Count; i++)
            {
                if(treshold < reward[i]) 
                {
                    n++;
                    ef += matrices[i];
                }
            }


            return ef / (n + AI.AISettings.GlobalEps);
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
