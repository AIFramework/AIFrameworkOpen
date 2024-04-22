// ------------------------------
// Оригинальный проект Python:
// https://github.com/Bots-Avatar/ExplainitAll/blob/main/explainitall/metrics/CheckingForHallucinations.py
// -----------------------------------
using AI.ML.Distances;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AI.ExplainitALL.Metrics.SimMetrics
{
    /// <summary>
    /// Работа с коэфициентом Жаккара
    /// </summary>
    [Serializable]
    public class NgramJaccardMetric : SimTextMetricsBase<HashSet<string>>
    {
        /// <summary>
        /// Размер N-граммы
        /// </summary>
        public int NGrammSize { get; protected set; } = 2;
        /// <summary>
        /// Удалять ли пробелы и переносы
        /// </summary>
        public bool SpaceDel { get; protected set; }
        /// <summary>
        /// Удалять ли знаки пунктуации
        /// </summary>
        public bool PDel { get; protected set; }

        /// <summary>
        /// Работа с коэфициентом Жаккара
        /// </summary>
        public NgramJaccardMetric(int nGSize = 2, bool spaceDel = true, bool pDel = true)
        {
            NGrammSize = nGSize;
            SpaceDel = spaceDel;
            PDel = pDel;
        }

        /// <summary>
        /// Сравнение на базе расстояния Жаккара
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public override double Sim(HashSet<string> set1, HashSet<string> set2) 
            => BaseDist.JaccardCoefMin(set1, set2);

        /// <summary>
        /// Преобразование текста в множество
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override HashSet<string> Transform(string text)
        {
            string textData = text.ToLower();
            if (PDel) textData = Regex.Replace(textData, @"[\p{P}]", "");
            if (SpaceDel) textData = Regex.Replace(textData, @"[\s]", "");
            return TextToNGrammSet(textData);
        }

        // Разбивавает текст на n-граммы
        private HashSet<string> TextToNGrammSet(string text)
        {
            int n = NGrammSize - 1;
            var bigrams = new HashSet<string>();
            for (int i = 0; i < text.Length - n; i++)
                bigrams.Add(text.Substring(i, NGrammSize));

            return bigrams;
        }
    }
}
