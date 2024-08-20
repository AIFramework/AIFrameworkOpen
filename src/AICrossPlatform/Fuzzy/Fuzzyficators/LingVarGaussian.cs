using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using System;
using System.Collections.Generic;

namespace AI.Fuzzy.Fuzzyficators
{
    /// <summary>
    /// Лингвистическая переменная
    /// </summary>
    [Serializable]
    public class LingVarGaussian
    {
        private readonly KNNCl _knn = new KNNCl();
        private readonly List<string> _strings = new List<string>();

        /// <summary>
        /// Лингвистическая переменная
        /// </summary>
        public LingVarGaussian(int k = 3)
        {
            _knn.IsParsenMethod = true;
            _knn.K = k;
        }

        /// <summary>
        /// Добавить переменную
        /// </summary>
        /// <param name="features"></param>
        /// <param name="nameVar"></param>
        public void AddVar(Vector features, string nameVar)
        {
            int cl = -1;

            for (int i = 0; i < _strings.Count; i++)
                if (_strings[i] == nameVar) cl = i;

            if (cl < 0)
            {
                cl = _strings.Count;
                _strings.Add(nameVar);
            }


            _knn.AddClass(features, cl);
        }

        /// <summary>
        /// Распознать
        /// </summary>
        /// <param name="features"></param>
        public string Recognition(Vector features)
        {
            int cl = _knn.Classify(features);
            return _strings[cl];
        }

        /// <summary>
        /// Преобразование вектора в нечеткое множество
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public Dictionary<string, double> FeaturesToFuzzySet(Vector features)
        {
            var probs = _knn.ClassifyProbVector(features);
            //Console.WriteLine($"probs: {probs}");
            Dictionary<string, double> result = new Dictionary<string, double>();

            for (int i = 0; i < probs.Count; i++)
                result.Add(_strings[i], probs[i]);

            return result;
        }

        /// <summary>
        /// Преобразование вектора в нечеткое множество
        /// </summary>
        public Dictionary<string, double> FeaturesToFuzzySet(Vector features, Func<Vector, Vector> probTransformer, double pow = 1)
        {
            var probs = _knn.ClassifyProbVector(features).Transform(x => Math.Pow(x, pow));
            probs = probTransformer(probs);
            Dictionary<string, double> result = new Dictionary<string, double>();

            for (int i = 0; i < probs.Count; i++)
                if (probs[i] > 0) result.Add(_strings[i], probs[i]);

            return result;
        }
    }
}
