using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using AI.Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Классификатор основанный на теореме Байеса
    /// </summary>
    [Serializable]
    public class BayesianClassifier : IClassifier
    {
        private readonly NonCorrelatedGaussian nonCorrelatedGaussian = new NonCorrelatedGaussian();
        private readonly List<Dictionary<string, Vector>> classifiersParams = new List<Dictionary<string, Vector>>();
        private Vector w = new Vector(0);

        public int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }

        /// <summary>
        /// Классификация
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public Vector ClassifyProbVector(Vector inp)
        {
            Vector classes = new Vector(classifiersParams.Count);

            for (int i = 0; i < classifiersParams.Count; i++)
                classes[i] = w[i] * nonCorrelatedGaussian.CulcProb(inp, classifiersParams[i]);

            return classes / classes.Sum();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Train(Vector[] features, int[] classes)
        {
            throw new NotImplementedException();
        }

        public void Train(VectorIntDataset dataset)
        {
            var gr = dataset.GetGroupes();
            w = new Vector(gr.Length);

            for (int i = 0; i < gr.Length; i++)
            {
                var dat = new Dictionary<string, Vector>();
                dat.Add("std", gr[i].Std);
                dat.Add("mean", gr[i].Mean);

                w[i] = gr[i].GroupeFeatures.Count;
                classifiersParams.Add(dat);
            }
        }
    }
}
