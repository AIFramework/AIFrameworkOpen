using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.DataSets.Base
{

    /// <summary>
    /// Набор данных, содержащий массив последовательностей для обучения рекуррентных нейронных сетей.
    /// </summary>
    [Serializable]
    public class Many2ManyVectorClassifierDataset : List<Many2ManyVectorClassifier>
    {
        /// <summary>
        /// Метод возвращает массив списков (массив последовательностей) меток классов
        /// </summary>
        public List<int>[] GetLabels()
        {
            return this.Select(x => x.Labels).ToArray();
        }

        /// <summary>
        /// Метод возвращает массив списков (массив последовательностей) векторов признаков.
        /// </summary>
        public List<Vector>[] GetFeatures()
        {
            return this.Select(x => x.Features).ToArray();
        }

        /// <summary>
        /// Метод возвращает массив списков (массив последовательностей) меток классов в векторном представлении.
        /// </summary>
        public List<Vector>[] GetVectorLabels(int countClasses)
        {
            return ToVectors(GetLabels(), countClasses);
        }

        /// <summary>
        /// Перемешивание с равномерным распределением
        /// </summary>
        public void ShufflingDataset()
        {
            Many2ManyVectorClassifier[] data = ToArray();
            Clear();
            data.Shuffle();
            AddRange(data);
        }

        private List<Vector>[] ToVectors(List<int>[] labels, int countClasses)
        {
            List<Vector>[] outps = new List<Vector>[labels.Length];

            for (int i = 0; i < outps.Length; i++)
            {
                outps[i] = new List<Vector>();

                for (int j = 0; j < labels[i].Count; j++)
                {
                    Vector label = new Vector(countClasses)
                    {
                        [labels[i][j]] = 1
                    };
                    outps[i].Add(label); // Выход
                }
            }

            return outps;
        }
    }
}
