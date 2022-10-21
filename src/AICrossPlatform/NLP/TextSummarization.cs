using AI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AI.NLP
{

    /// <summary>
    /// Суммаризация
    /// </summary>
    [Serializable]
    public class TextSummarization
    {
        private readonly List<DataSummText> dataSummTexts = new List<DataSummText>();
        private ProbabilityDictionaryData[] probabilityDictionaryDatas;
        private readonly ProbabilityDictionary probabilityDictionary = new ProbabilityDictionary();
        private ProbabilityDictionaryData[][] probabilityDictionaryDataSeqs;
        private string[] seqs;
        private static readonly string nPat = @"[А-Я]\."; // паттерн инициалов
        private static readonly string uKPat = @" [а-я]\."; // паттерн различных сокращений
        private static readonly string uKPat2 = @" [а-я]\.[а-я]\."; //паттерн сокращений типа т.п., т.н.
        private static readonly string rectPat = @"\[[\w\s]*\]"; // паттерн квадратных скобочек с описанием

        /// <summary>
        /// Суммаризация
        /// </summary>
        public TextSummarization()
        {

        }

        /// <summary>
        /// Суммаризация текста
        /// </summary>
        /// <param name="text">Текст</param>
        /// <param name="num">Кол-во предложений для описания текста</param>
        /// <returns></returns>
        public string Summarization(string text, int num = 1)
        {
            string outp = "";

            Step1(text);
            Step2();

            dataSummTexts.Sort((a, b) => a.W.CompareTo(b.W) * -1);

            for (int i = 0; i < num; i++)
                outp += dataSummTexts[i].Str + "\n\n";

            return outp;

        }

        /// <summary>
        /// Выдает предложения
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns></returns>
        public static string[] GetSeqs(string text)
        {
            string t = text;
            t = t.Replace("(", " ( ").Replace(")", " ) "); // скобки



            while (t.Contains("  "))
                t = t.Replace("  ", " ");


            //Сокращения
            t = t.Replace(" р.", " р").Replace(" д.", "д").Replace(" кв.", " квартира").Replace(" ул.", " улица").Replace(" г.", " г")
                .Replace("гг.", " гг").Replace(" др.", " др").Replace(" исл.", " исландский").Replace(" вел.", " вел").Replace(" кн.", " кн").Replace(" г.", " г")
                .Replace("км.", " км").Replace(" тд.", " тд").Replace(" англ.", " английский").Replace(" кр.", " кр").Replace(" тк.", " тк").Replace("Рис.", " рис").
                Replace(" рис.", " рис");

            //Форматирование
            t = t.Replace("\n", "").Replace("\r", "").Replace("\t", "");

            // Удаление паттернов
            t = Regex.Replace(t, nPat, "");
            t = Regex.Replace(t, uKPat, "");
            t = Regex.Replace(t, rectPat, "");
            t = Regex.Replace(t, uKPat2, "");

            // Замена знаков
            t = t.Replace("!", ".").Replace("?", ".");

            while (t.Contains(".."))
                t = t.Replace("..", ".");
            return t.Split('.').Transform(x => x.Trim() + ".");
        }

        /// <summary>
        /// Первый шаг алгоритма (составление вероятностных словарей)
        /// </summary>
        /// <param name="text">Текст</param>
        private void Step1(string text)
        {
            seqs = GetSeqs(text);

            probabilityDictionaryDataSeqs = new ProbabilityDictionaryData[seqs.Length][];

            for (int i = 0; i < seqs.Length; i++)
                probabilityDictionaryDataSeqs[i] = probabilityDictionary.Run(seqs[i]);

            string text2 = string.Join(" ", seqs);
            probabilityDictionaryDatas = probabilityDictionary.Run(text2);
        }


        // Второй шаг составления списка: предложение, вес
        private void Step2()
        {
            for (int i = 0; i < seqs.Length; i++)
            {
                try
                {
                    double w = GetW(i);
                    if (!(double.IsNaN(w) || double.IsInfinity(w)))
                        dataSummTexts.Add(new DataSummText(seqs[i], w));
                }
                catch { }
            }
        }


        // Расчет семантического веса
        private double GetW(int ind)
        {
            double[] inSeq = new double[probabilityDictionaryDataSeqs[ind].Length], allText = new double[probabilityDictionaryDataSeqs[ind].Length];
            string word;
            double w = 0;


            for (int i = 0; i < inSeq.Length; i++)
            {
                inSeq[i] = probabilityDictionaryDataSeqs[ind][i].Probability;
                word = probabilityDictionaryDataSeqs[ind][i].Word;

                for (int j = 0; j < probabilityDictionaryDatas.Length; j++)
                    if (probabilityDictionaryDatas[j].Word == word)
                        allText[i] = probabilityDictionaryDatas[j].Probability;
            }

            for (int i = 0; i < inSeq.Length; i++)
                w += allText[i] / inSeq[i];

            return w / inSeq.Length;
        }



    }

    /// <summary>
    /// Данные предложений
    /// </summary>
    public class DataSummText
    {
        /// <summary>
        /// Вес
        /// </summary>
        public double W { get; set; }
        /// <summary>
        /// Содержание
        /// </summary>
        public string Str { get; set; }

        /// <summary>
        /// Данные предложений
        /// </summary>
        /// <param name="str">строка</param>
        /// <param name="w">вес</param>
        public DataSummText(string str, double w)
        {
            W = w;
            Str = str;
        }
    }
}
