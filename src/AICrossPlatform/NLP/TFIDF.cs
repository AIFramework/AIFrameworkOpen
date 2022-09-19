using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.NLP
{
    /// <summary>
    /// TF-IDF
    /// </summary>
    [Serializable]
    public class TFIDF
    {
        internal readonly Dictionary<string, double>[] pDs;

        /// <summary>
        /// TF-IDF
        /// </summary>
        /// <param name="docs">Массив документов</param>
        public TFIDF(string[] docs)
        {
            ProbabilityDictionaryHash probabilityDictionary = new ProbabilityDictionaryHash();
            pDs = new Dictionary<string, double>[docs.Length];


            for (int i = 0; i < docs.Length; i++)
            {
                pDs[i] = probabilityDictionary.Run(docs[i]);
            }
        }

        /// <summary>
        /// tf
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dIndex"></param>
        /// <returns></returns>
        public double TFWord(string t, int dIndex)
        {
            if (pDs[dIndex].ContainsKey(t))
            {
                return pDs[dIndex][t];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Idf
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double IDFWord(string t)
        {
            int D = pDs.Length;
            double count = 0;

            for (int dIndex = 0; dIndex < D; dIndex++)
            {
                if (pDs[dIndex].ContainsKey(t))
                {
                    count++;
                }
            }

            return Math.Log10(0.1 + (D / (count + 0.001)));
        }

        /// <summary>
        /// TF-IDF
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dIndex"></param>
        /// <returns></returns>
        public double TF_IDF(string t, int dIndex)
        {
            return TFWord(t, dIndex) * IDFWord(t);
        }

        /// <summary>
        /// Принадлежность строки к определенному документу
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="dIndex">Индекс документа</param>
        public double TF_IDF_Str(string str, int dIndex)
        {
            string[] strs = ProbabilityDictionary.GetWords(str, true);
            double mean = 0;


            for (int i = 0; i < strs.Length; i++)
            {
                mean += TF_IDF(strs[i], dIndex);
            }

            return mean / strs.Length;
        }

        /// <summary>
        /// Поиск документа
        /// </summary>
        /// <param name="req">Запрос</param>
        public int Search(string req)
        {
            Vector ind = new Vector(pDs.Length);
            for (int i = 0; i < ind.Count; i++)
            {
                ind[i] = TF_IDF_Str(req, i);

            }


            return ind.MaxElementIndex();
        }


        //public void Save(string path)
        //{

        //}
    }



}
