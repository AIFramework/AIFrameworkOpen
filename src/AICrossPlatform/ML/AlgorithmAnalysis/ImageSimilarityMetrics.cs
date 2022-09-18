using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.ML.AlgorithmAnalysis
{
    /// <summary>
    /// Метрики схожести изображений
    /// </summary>
    [Serializable]
    public class ImageSimilarityMetrics
    {

        /// <summary>
        /// Приблизительный алгоритм вычисления значения метрики Дайса (метрика симметричная) 
        /// </summary>
        /// <param name="alg_str_1">Алгебраическая структура 1</param>
        /// <param name="alg_str_2">Алгебраическая структура 2</param>
        public double DiceApproximate(IAlgebraicStructure<double> alg_str_1, IAlgebraicStructure<double> alg_str_2)
        {
            Vector v1 = alg_str_1.Data;
            Vector v2 = alg_str_2.Data;
            return 2 * (v1 * v2).Mean() / (v1.Mean() + v2.Mean());
        }

        /// <summary>
        /// Значение метрики Дайса (метрика симметричная) 
        /// </summary>
        /// <param name="alg_str_1">Алгебраическая структура 1</param>
        /// <param name="alg_str_2">Алгебраическая структура 2</param>
        /// <param name="trashold">Порог</param>
        public double Dice(IAlgebraicStructure<double> alg_str_1, IAlgebraicStructure<double> alg_str_2, double trashold = 0.5)
        {
            int alg_str_1_count_el = alg_str_1.Shape.Count;
            double[] data1 = alg_str_1.Data;
            double[] data2 = alg_str_2.Data;
            double s1 = 0, s2 = 0, s_cross = 0;

            for (int i = 0; i < alg_str_1_count_el; i++)
            {
                bool struct_1 = data1[i] >= trashold;
                bool struct_2 = data2[i] >= trashold;

                if (struct_1) s1++;
                if (struct_2) s2++;
                if(struct_2 && struct_1) s_cross++;
            }

            return 2 * s_cross / (s1 + s2);
        }

    }
}
