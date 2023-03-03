using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Fuzzy.Fuzzyficators
{
    /// <summary>
    /// Лингвистическая переменная
    /// </summary>
    public class LingVarGaussian
    {
        KNNCl _knn = new KNNCl();
        List<string> _strings = new List<string>();

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
    }
}
