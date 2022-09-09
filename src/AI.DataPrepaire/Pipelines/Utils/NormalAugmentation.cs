using AI.DataStructs.Algebraic;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.Pipelines.Utils
{

    /// <summary>
    /// Аугментация на базе гауссового распределения
    /// </summary>
    [Serializable]
    public class NormalAugmentation : DataAugmetation<Vector>
    {

        double _mean = 0;
        double _std = 1;
        Random random = new Random();


        /// <summary>
        /// Аугментация на базе гауссового распределения (Вокруг каждой точки рисует окружность радиусом 3*std)
        /// </summary>
        public NormalAugmentation(int kAug, double mean = 0, double std = 0.01) : base(kAug)
        {
            _mean = mean;
            _std = std;
        }


        /// <summary>
        /// Аугментация данных
        /// </summary>
        /// <param name="sample">Реальные данные</param>
        public override Vector[] Augmetation(Vector sample)
        {
            Vector[] data = new Vector[KAug];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = sample + (Statistic.RandNorm(sample.Count, random) + _mean)*_std;
            }

            return data;
        }
    }
}
