using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.NLP
{
    /// <summary>
    /// TF-IDF Dictionary
    /// </summary>
    [Serializable]
    public class TFIDFDictionary
    {
        /// <summary>
        /// TF-IDF algorithm
        /// </summary>
        public TFIDF tFIDF { get; set; }

        private readonly int _n;

        /// <summary>
        /// Generating TF-IDF Dictionary
        /// </summary>
        /// <param name="pathToDir">Path to the directory with documents</param>
        public TFIDFDictionary(string pathToDir)
        {
            string[] fs = Directory.GetFiles(pathToDir);
            string[] strs = new string[fs.Length];

            _n = fs.Length;

            for (int i = 0; i < fs.Length; i++)
            {
                strs[i] = File.ReadAllText(fs[i]);
            }

            tFIDF = new TFIDF(strs);
        }

        /// <summary>
        /// Word to vector based on dictionary
        /// </summary>
        /// <param name="word">Word</param>
        public Vector ToVect(string word)
        {
            Vector ind = new Vector(_n);
            for (int i = 0; i < ind.Count; i++)
            {
                ind[i] = tFIDF.TF_IDF_Str(word, i);

            }


            return ind / ind.Max();
        }


        /// <summary>
        /// Word proximity calculation
        /// </summary>
        /// <returns></returns>
        public double Sim(string word1, string word2)
        {
            return Statistics.Statistic.CorrelationCoefficient(ToVect(word1), ToVect(word2));
        }


        /// <summary>
        /// Drawing up a vector dictionary
        /// </summary>
        public Dictionary<string, Vector> VectorDictionary() 
        {
            Dictionary<string, double>[] dotDicts = tFIDF.pDs;
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
