using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.ExplainitALL.Metrics.SimAlgs
{
    /// <summary>
    /// Матрица схожестей без перефразировок
    /// </summary>
    [Serializable]
    public class NgramJaccardSim : SimMatrix<HashSet<string>>
    {

        /// <summary>
        /// Нормализовать ли на длинну вопроса
        /// </summary>
        public bool QNorm { get; set; } = true;
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
        /// Матрица схожестей без перефразировок
        /// </summary>
        public NgramJaccardSim(int nGSize = 2, bool spaceDel = true, bool pDel = true)
        {
            NGrammSize = nGSize;
            SpaceDel = spaceDel;
            PDel = pDel;
            PDel = pDel;
        }

        public override double Sim(HashSet<string> set1, HashSet<string> set2) =>
            JaccardCoefficient(set1, set2);

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

        //Симметричный коэффициент Жаккара
        private double JaccardCoefficient(HashSet<string> set1, HashSet<string> set2) =>
            set1.Intersect(set2).Count() / set1.Union(set2).Count();
    }
}
