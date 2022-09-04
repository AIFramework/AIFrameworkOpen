/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 11.09.2013
 * Time: 17:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace AI.NLP
{


    /// <summary>
    /// Вероятностный словарь
    /// </summary>
    [Serializable]
    public class ProbabilityDictionaryHash
    {
        /// <summary>
        /// Вероятностный словарь 
        /// </summary>
        public Dictionary<string, double> pDictionary { get; private set; }

        private readonly List<string> Words = new List<string>();
        /// <summary>
        /// Применять ли стеммер
        /// </summary>
        public bool IsStem { get; set; }






        /// <summary>
        /// Вероятностный словарь
        /// </summary>
        /// <param name="isStem">Делать ли стеммеризацию</param>
        public ProbabilityDictionaryHash(bool isStem = true)
        {
            pDictionary = new Dictionary<string, double>();
            IsStem = isStem;
        }

        /// <summary>
        /// Данные вероятностного словаря
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns></returns>
        public Dictionary<string, double> Run(string text)
        {
            Words.Clear();
            Words.AddRange(ProbabilityDictionary.GetWords(text, IsStem));
            Analis();
            return pDictionary;
        }



        /// <summary>
        /// Анализ текста
        /// </summary>
        private void Analis()
        {
            pDictionary = new Dictionary<string, double>();
            pDictionary.Clear();
            Dictionary<string, double> data = new Dictionary<string, double>();

            for (int i = 0; i < Words.Count; i++)
            {
                if (data.ContainsKey(Words[i]))
                {
                    data[Words[i]]++;
                }
                else
                {
                    data.Add(Words[i], 1);
                }
            }


            foreach (KeyValuePair<string, double> sempl in data)
            {
                pDictionary.Add(sempl.Key, sempl.Value / Words.Count);
            }

        }


    }
}
