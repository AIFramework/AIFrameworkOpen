using AI.DataPrepaire.NLPUtils.TextGeneration;
using AI.DataStructs.Algebraic;
using System;

namespace AI.DataPrepaire.NLPUtils
{
    /// <summary>
    /// Методы сравнения строк
    /// </summary>
    [Serializable]
    public static class CompareStringMethods
    {
        /// <summary>
        /// Схожесть строк выраженая через расстояние Левенштейна
        /// </summary>
        /// <param name="input"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float LevenshteinDistance(string input, string target)
        {
            int diff;
            int[,] m = new int[input.Length + 1, target.Length + 1];

            for (int i = 0; i <= input.Length; i++)
                m[i, 0] = i;

            for (int j = 0; j <= target.Length; j++)
                m[0, j] = j;

            for (int i = 1; i <= input.Length; i++)
                for (int j = 1; j <= target.Length; j++)
                {
                    diff = (input[i - 1] == target[j - 1]) ? 0 : 1;

                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1,
                                             m[i, j - 1] + 1),
                                             m[i - 1, j - 1] + diff);
                }

            double outp = m[input.Length, target.Length];
            double k = Math.Min(input.Length, target.Length) / 4.0;

            return (float)HightLevelFunctions.DistributionFunctions.GaussNorm1(outp, 0, k);
        }

        /// <summary>
        /// Корреляция строк выраженая через расстояние Левенштейна
        /// </summary>
        /// <param name="input"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float WordCorellation(string input, string target)
        {
            string[] inps = input.Split(' ');
            string[] targs = target.Split(' ');

            float output = 0;

            int len = Math.Min(inps.Length, targs.Length);

            for (int i = 0; i < len; i++)
                output += LevenshteinDistance(inps[i], targs[i]);

            return output / len;
        }

        /// <summary>
        /// Сравнение текста по n-граммам
        /// </summary>
        /// <param name="input">Строка входа</param>
        /// <param name="target">Целевая строка</param>
        /// <param name="n">Количество слов</param>
        public static float HistogramCos(string input, string target, int n)
        {
            HMMFast hMMFast = new HMMFast { NGram = n };

            hMMFast.Train(target + " " + input);
            Vector a = hMMFast.TextToVector(input);
            Vector b = hMMFast.TextToVector(target);

            return (float)a.Cos(b);
        }

        /// <summary>
        /// Сравнение текста по n-граммам
        /// </summary>
        /// <param name="input">Строка входа</param>
        /// <param name="target">Целевая строка</param>
        /// <param name="n">Количество слов</param>
        public static float HistogramCrossEntropy(string input, string target, int n)
        {
            HMMFast hMMFast = new HMMFast { NGram = n };
            hMMFast.Train(target + " " + input);
            Vector a = hMMFast.TextToVector(input);
            Vector b = hMMFast.TextToVector(target);

            a /= a.Sum();
            b /= b.Sum();

            float outp2 = 0;

            for (int i = 0; i < a.Count; i++)
                outp2 -= (float)(b[i] * Math.Log10(a[i] + 1e-10));

            outp2 = 1f / (1f + outp2);

            return outp2;
        }

        /// <summary>
        /// Сравнение текста по n-граммам
        /// </summary>
        /// <param name="input">Строка входа</param>
        /// <param name="target">Целевая строка</param>
        public static float HistogramCrossEntropy(string input, string target)
        {
            if (input.Split(' ').Length <= 4)
                return HistogramCrossEntropy(input, target, 1);
            else if (input.Split(' ').Length < 20)
                return HistogramCrossEntropy(input, target, 2);
            else
                return HistogramCrossEntropy(input, target, 3);
        }

        /// <summary>
        /// Сравнение текста по n-граммам
        /// </summary>
        /// <param name="input">Строка входа</param>
        /// <param name="target">Целевая строка</param>
        public static float HistogramCos(string input, string target)
        {
            if (input.Split(' ').Length <= 4)
                return HistogramCos(input, target, 1);
            else if (input.Split(' ').Length < 20)
                return HistogramCos(input, target, 2);
            else
                return HistogramCos(input, target, 3);
        }
    }
}
