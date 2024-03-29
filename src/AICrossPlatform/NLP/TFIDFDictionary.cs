﻿using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.NLP
{
    /// <summary>
    /// TF-IDF словарь
    /// </summary>
    [Serializable]
    public class TFIDFDictionary
    {
        /// <summary>
        /// TF-IDF алгоритм
        /// </summary>
        public TFIDF TfIdf { get; set; }

        private readonly int _n;

        /// <summary>
        /// Создание словаря tf-idf
        /// </summary>
        /// <param name="pathToDir">Путь до директории с документами</param>
        public TFIDFDictionary(string pathToDir)
        {
            string[] fs = Directory.GetFiles(pathToDir);
            string[] strs = new string[fs.Length];

            _n = fs.Length;

            for (int i = 0; i < fs.Length; i++)
                strs[i] = File.ReadAllText(fs[i]);

            TfIdf = new TFIDF(strs);
        }

        /// <summary>
        /// Преобразование слова в вектор
        /// </summary>
        /// <param name="word">Слово</param>
        public Vector ToVect(string word)
        {
            Vector ind = new Vector(_n);
            for (int i = 0; i < ind.Count; i++)
                ind[i] = TfIdf.TF_IDF_Str(word, i);

            return ind / ind.Max();
        }


        /// <summary>
        /// Расчет близости слов
        /// </summary>
        /// <returns></returns>
        public double Sim(string word1, string word2)
        {
            return Statistics.Statistic.CorrelationCoefficient(ToVect(word1), ToVect(word2));
        }


        /// <summary>
        /// Составление векторного словаря
        /// </summary>
        public Dictionary<string, Vector> VectorDictionary()
        {
            Dictionary<string, double>[] dotDicts = TfIdf.pDs;
            Dictionary<string, Vector> vecDict = new Dictionary<string, Vector>();

            for (int i = 0; i < dotDicts.Length; i++)
            {
                foreach (KeyValuePair<string, double> item in dotDicts[i])
                {
                    string word = item.Key;
                    if (!vecDict.ContainsKey(word))
                    {
                        Vector vect = ToVect(word);
                        vecDict.Add(word, vect);
                    }
                }
            }

            return vecDict;
        }
    }
}
