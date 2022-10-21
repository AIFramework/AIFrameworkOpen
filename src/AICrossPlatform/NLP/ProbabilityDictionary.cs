/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 11.09.2013
 * Time: 17:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.NLP.Stemmers;
using System;
using System.Collections.Generic;

namespace AI.NLP
{

    /// <summary>
    /// Вероятностный словарь
    /// </summary>
    [Serializable]
    public class ProbabilityDictionary
    {

        /// <summary>
        /// Вероятностный словарь
        /// </summary>
        public ProbabilityDictionaryData[] pDictionary { get; private set; }

        private readonly List<ProbabilityDictionaryData> list = new List<ProbabilityDictionaryData>();
        private readonly List<string> Words = new List<string>();
        private int n;
        /// <summary>
        /// Удалять ли цифры
        /// </summary>
		public bool IsDigitDel { get; set; }
        /// <summary>
        /// Использовать ли стеммер
        /// </summary>
		public bool IsStem { get; set; }

        /// <summary>
        /// Слова не несущие смысла
        /// </summary>
        public static string[] stop = new string[0];

        /// <summary>
        /// Слова не несущие смысла при стат. анализе
        /// </summary>
        public static string[] StopWords => stop;




        /// <summary>
        /// Вероятностный словарь
        /// </summary>
        /// <param name="isStopDel">Удалять ли стоп-слова</param>
        /// <param name="isDigitDel">Удалять ли числа</param>
        /// <param name="isStem">Делать ли стеммеризацию</param>
        public ProbabilityDictionary(bool isStopDel = true, bool isDigitDel = true, bool isStem = true)
        {
            if (!isStopDel)
            {
                stop = new string[] { " " };
            }

            IsDigitDel = isDigitDel;
            IsStem = isStem;
        }

        /// <summary>
        /// Данные вероятностного словаря
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns></returns>
        public ProbabilityDictionaryData[] Run(string text)
        {
            GetWords(text);
            Analis();
            list.Sort((a, b) => a.Probability.CompareTo(b.Probability) * -1); // Сортировка массива
            pDictionary = list.ToArray(); // создание массива
            return pDictionary;
        }



        /// <summary>
        /// Запуск генерации словаря с выводом всех слов
        /// </summary>
        /// <param name="text">Текст для генерации</param>
        public string[] GetWordsRunAll(string text)
        {
            ProbabilityDictionaryData[] wsp = Run(text);
            string[] strs = new string[wsp.Length];

            for (int i = 0; i < wsp.Length; i++)
            {
                strs[i] = wsp[i].Word;
            }

            return strs;
        }



        /// <summary>
        /// Запуск генерации словаря с выводом определенного числа слов
        /// </summary>
        /// <param name="text">Текст для генерации</param>
        /// <param name="numW">Число слов</param>
        public string[] GetWordsRun(string text, int numW = 30)
        {
            ProbabilityDictionaryData[] wsp = Run(text);
            int len = (pDictionary.Length < numW) ? pDictionary.Length : numW;

            string[] strs = new string[len];

            for (int i = 0; i < len; i++)
                strs[i] = wsp[i].Word;

            return strs;
        }


        /// <summary>
        /// Возвращает true если в сторке есть цифры
        /// </summary>
        /// <param name="str">Строка</param>
        private static bool DigialPredickat(string str)
        {

            foreach (char ch in str)
                if (char.IsDigit(ch) || char.IsSeparator(ch))
                    return true;

            return false;
        }


        /// <summary>
        /// Анализ текста
        /// </summary>
        private void Analis()
        {

            bool flag;

            if (IsDigitDel)
                Words.RemoveAll(DigialPredickat);// Удаление строк с числами

            foreach (string str in stop)
                do flag = Words.Remove(str); while (flag);

            flag = false;

            // Составление словаря	
            while (Words.Count != 0)
            {
                string str = Words[0];
                double count = 0;
                ProbabilityDictionaryData fD = new ProbabilityDictionaryData();

                for (int i = 0; i < Words.Count; i++)
                    if (Words[i] == str)  count++;

                fD.Probability = count / n;
                fD.Word = str;

                list.Add(fD); // Добавить элемент

                // Удаление данных
                do flag = Words.Remove(str); while (flag);
            }

        }


        /// <summary>
        /// Переводит частотный словарь в строку
        /// </summary>
        /// <param name="index">До какого индекса</param>
        /// <returns></returns>
        public string ToString(int index)
        {
            string output = "";

            int len = (pDictionary.Length < index) ? pDictionary.Length : index;

            for (int i = 0; i < len; i++)
                output += pDictionary[i].Word + " " + pDictionary[i].Probability + "\n";

            return output.Trim();
        }

        /// <summary>
        /// Получение слов
        /// </summary>
        /// <param name="text">Текст</param>
        public void GetWords(string text)
        {

            string[] strs = text.ToLower().Replace("\r", "").Split(new char[] { ' ', '\n', '\t', '[', ']', '-' });
            n = strs.Length;
            string word;
            bool f = true;

            // 
            foreach (string str in strs)
            {
                f = true;

                for (int i = 0; i < stop.Length; i++)
                    if (stop[i] == str)  f = false;

                if (f)
                {
                    word = str.ToLower().Trim(new char[] { '?', '!', '.', ',', ' ', '\t', '(', ')', '<', '>', '»', '«', ':', '\"', '*', '-', ';' });
                    if (IsStem)
                    {
                        word = StemmerRus.TransformingWord(word);
                    }

                    Words.Add(word);
                }
            }
        }

        /// <summary>
        /// Получение слов
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsStem"></param>
        /// <returns></returns>
        public static string[] GetWords(string text, bool IsStem)
        {

            List<string> Words = new List<string>();
            string[] strs = TextStandard.OnlyCharsAndDigit(text).Split(' ');
            int n = strs.Length;
            string word;
            bool f = true;

            // 
            foreach (string str in strs)
            {
                f = true;

                for (int i = 0; i < stop.Length; i++)
                    if (stop[i] == str) f = false;

                if (f)
                {
                    word = str;
                    if (IsStem)
                        word = StemmerRus.TransformingWord(word);

                    Words.Add(word);
                }
            }

            return Words.ToArray();
        }


    }
}
