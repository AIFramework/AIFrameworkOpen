using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ML.DataSets.Base
{
    /// <summary>
    /// Sequence for training a recurrent network on a many-to-many basis
    /// </summary>
    [Serializable]
    public class Many2ManyVectorClassifier
    {
        /// <summary>
        /// Class labels
        /// </summary>
        public List<int> Labels { get; set; }
        /// <summary>
        /// Features
        /// </summary>
        public List<Vector> Features { get; set; }

        /// <summary>
        /// Sequence for training a recurrent network on a many-to-many basis
        /// </summary>
        public Many2ManyVectorClassifier()
        {
        }

        /// <summary>
        /// Sequence for training a recurrent network on a many-to-many basis
        /// </summary>
        /// <param name="labels">Class labels</param>
        /// <param name="features"></param>
        public Many2ManyVectorClassifier(IEnumerable<int> labels, IEnumerable<IAlgebraicStructure> features)
        {
            Labels = labels.ToList();
            Features = features.Select(x => (Vector)x.Data).ToList();
        }

        /// <summary>
        /// Getting a dataset, for use with a neural network manager
        /// </summary>
        /// <param name="data">Dataset</param>
        /// <param name="doShuffling">Whether it is necessary to shuffle the data in random order</param>
        public Many2ManyVectorClassifierDataset GetDataset(IEnumerable<Many2ManyVectorClassifier> data, bool doShuffling = true)
        {
            Many2ManyVectorClassifier[] array = data.ToArray();

            if (doShuffling)
            {
                array.Shuffle();
            }

            Many2ManyVectorClassifierDataset many2ManyVectorClassifiers = new Many2ManyVectorClassifierDataset();
            many2ManyVectorClassifiers.AddRange(array);
            return many2ManyVectorClassifiers;
        }
    }
}
