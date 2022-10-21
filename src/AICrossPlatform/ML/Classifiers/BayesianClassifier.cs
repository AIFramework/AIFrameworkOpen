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
        private List<Dictionary<string, Vector>> classifiersParams = new List<Dictionary<string, Vector>>();
        private Vector w = new Vector(0);

        /// <summary>
        /// Классификатор основанный на теореме Байеса
        /// </summary>
        public BayesianClassifier(int nInp, int nOutp)
        {
            w = new Vector(nOutp)+0.5;

            for (int i = 0; i < nOutp; i++)
            {
                var dat = new Dictionary<string, Vector>();
                dat.Add("std", new Vector(nInp)+1);
                dat.Add("mean", new Vector(nInp));
                classifiersParams.Add(dat);
            }
        }

        /// <summary>
        /// Классификатор основанный на теореме Байеса
        /// </summary>
        public BayesianClassifier()
        {

        }

        /// <summary>
        /// Классификация
        /// </summary>
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

        /// <summary>
        /// Не реализовано
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Не реализовано
        /// </summary>
        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Обучение байесовского классификатора
        /// </summary>
        public void Train(Vector[] features, int[] classes)
        {
            VectorIntDataset vectorClasses = new VectorIntDataset();

            for (int i = 0; i < features.Length; i++)
            {
                vectorClasses.Add(new VectorClass(features[i], classes[i]));
            }

            Train(vectorClasses);
        }

        /// <summary>
        /// Обучение байесовского классификатора
        /// </summary>
        /// <param name="dataset"></param>
        public void Train(VectorIntDataset dataset)
        {
            var gr = dataset.GetGroupes();
            w = new Vector(gr.Length);
            classifiersParams = new List<Dictionary<string, Vector>>();

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
