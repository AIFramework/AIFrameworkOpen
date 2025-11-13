/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 07.07.2018
 * Время: 16:33
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;
using System.Collections.Generic;

namespace AI.ClassicMath.AlgorithmAnalysis
{
    /// <summary>
    /// Проверка схожести двух базисов
    /// </summary>
    [Serializable]
    public class SimilarityOfBases
    {
        private readonly List<Vector> bases1 = new List<Vector>();
        private readonly List<Vector> bases2 = new List<Vector>();
        private readonly Vector sim, maxSim;

        /// <summary>
        /// Проверка схожести двух базисов
        /// </summary>
        /// <param name="bas1">Базис №1</param>
        /// <param name="bas2">Базис №2</param>
        public SimilarityOfBases(Matrix bas1, Matrix bas2)
        {
            bases1.AddRange(Matrix.GetColumns(bas1));
            bases2.AddRange(Matrix.GetColumns(bas2));
            sim = new Vector(bases1.Count);
            maxSim = new Vector(bases1.Count);
        }


        /// <summary>
        /// Вероятность что базисы не связаны (случайны)
        /// </summary>
        /// <returns></returns>
        public double ProbRandBasis()
        {

            int k = 0;
            double prob;

            while (bases1.Count > 0)
            {
                for (int i = 0; i < bases2.Count; i++)
                {
                    sim[i] = Math.Abs(Statistic.CorrelationCoefficient(bases1[0], bases2[i]));
                }

                maxSim[k++] = Statistic.MaximalValue(sim);

                try
                {
                    bases1.RemoveAt(0);
                    bases2.RemoveAt(sim.FindIndex(el => el == maxSim[k - 1]));
                }
                catch { }

            }

            prob = 1 - Statistic.ExpectedValue(maxSim);

            return prob;
        }




    }
}
