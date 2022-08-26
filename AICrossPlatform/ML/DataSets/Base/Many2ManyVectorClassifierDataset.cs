using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.DataSets.Base
{

    /// <summary>
    /// Dataset containing an array of sequences for training recurrent neural networks
    /// </summary>
    [Serializable]
    public class Many2ManyVectorClassifierDataset : List<Many2ManyVectorClassifier>
    {
        /// <summary>
        /// The method returns an array of lists(an array of sequences) of class labels
        /// </summary>
        public List<int>[] GetLabels()
        {
            return this.Select(x => x.Labels).ToArray();
        }

        /// <summary>
        /// The method returns an array of lists(an array of sequences) of feature vectors
        /// </summary>
        public List<Vector>[] GetFeatures()
        {
            return this.Select(x => x.Features).ToArray();
        }

        /// <summary>
        /// The method returns an array of lists(an array of sequences) of class labels in vector representation
        /// </summary>
        public List<Vector>[] GetVectorLabels(int countClasses)
        {
            return ToVectors(GetLabels(), countClasses);
        }

        /// <summary>
        /// Uniform shuffling
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
