using System;
using System.Collections.Generic;
using System.Text;

namespace AI.NLP
{
    /// <summary>
    /// Стандартизация текста
    /// </summary>
    [Serializable]
    public static class TextStandard
    {
        /// <summary>
        ///  Стандартизация входного текста
        /// </summary>
        /// <param name="input">Входной текст</param>
        /// <param name="isLower">Переводить ли текст в нижний регистр</param>
        public static string Normalize(string input, bool isLower = true)
        {
            if (input.Contains("base64"))
                return input;

            string output = isLower ? input.ToLower() : input;
            output = output.Replace("\r", "");
            output = output.Replace("\t", " ").Replace("\n", " ");
            output = output.Replace("!", ".").Replace("?", ".");
            output = output.Replace("—", "-").Replace("--", "-").Replace("ё", "е");


            while (output.Contains("  "))
                output = output.Replace("  ", " ");
            while (output.Contains(".."))
                output = output.Replace("..", ".");

            return output.Trim(' ');
        }



        /// <summary>
        /// В запросе остаются только буквы, цифры и знаки пробела
        /// </summary>
        /// <param name="input">Входной текст</param>
        /// <param name="isLower">Переводить ли в нижний регистр</param>
        public static string OnlyCharsAndDigit(string input, bool isLower = true)
        {
            List<char> charsADigit = new List<char>();

            string outp = Normalize(input, isLower);
            outp = isLower? outp.ToLower() : outp;

            for (int i = 0; i < outp.Length; i++)
                if (char.IsLetterOrDigit(outp[i]) || outp[i] == ' ')
                    charsADigit.Add(outp[i]);
                

            string output = new string(charsADigit.ToArray());

            while (output.Contains("  "))
                output = output.Replace("  ", " ");

            return output;
        }

        /// <summary>
        /// В запросе остаются только буквы и знаки пробела
        /// </summary>
        /// <param name="input">Входной текст</param>
        /// <param name="isLower">Переводить ли в нижний регистр</param>
        public static string OnlyChars(string input, bool isLower = true)
        {
            List<char> chars = new List<char>();

            string outp = Normalize(input, isLower);

            for (int i = 0; i < outp.Length; i++)
                if (char.IsLetter(outp[i]) || outp[i] == ' ')
                    chars.Add(outp[i]);

            string output = new string(chars.ToArray());

            while (output.Contains("  "))
                output = output.Replace("  ", " ");

            return output;
        }


        /// <summary>
        /// В запросе остаются только буквы и знаки пробела
        /// </summary>
        /// <param name="input">Входной текст</param>
        public static string OnlyRusChars(string input)
        {
            List<char> chars = new List<char>();

            string outp = Normalize(input);

            for (int i = 0; i < outp.Length; i++)
                if (IsRusLeter(outp[i]) || outp[i] == ' ')
                    chars.Add(outp[i]);

            string output = new string(chars.ToArray());

            while (output.Contains("  "))
                output = output.Replace("  ", " ");

            return output;
        }

        private static bool IsRusLeter(char ch)
        {
            return ch >= 'а' && ch <= 'я';
        }

        /// <summary>
        /// Удаляет повторы слов
        /// </summary>
        /// <param name="input">Входной текст</param>
        public static string NoDoubleWord(string input)
        {

            StringBuilder stringBuilder = new StringBuilder();
            string[] strs = input.Split(' ');
            string oldWord = "";

            for (int i = 0; i < strs.Length; i++)
                if (strs[i] != oldWord)
                {
                    stringBuilder.Append(strs[i]);
                    stringBuilder.Append(" ");
                    oldWord = strs[i];
                }

            return stringBuilder.ToString().Trim(' ');
        }

        /// <summary>
        /// Выдает множество слов
        /// </summary>
        /// <param name="input">Входной текст</param>
        /// <param name="preprocessingString">Обработчик текста</param>
        /// <param name="preprocessingWord">Обработчик слов</param>
        /// <param name="appendWord">Добавалять ли слово в список</param>
        /// <returns></returns>
        public static HashSet<string> GetWords(string input, Func<string, string> preprocessingString, Func<string, string> preprocessingWord, Func<string, bool> appendWord)
        {
            HashSet<string> set = new HashSet<string>();
            string[] words = preprocessingString(input).Split(' ');

            for (int i = 0; i < words.Length; i++)
                if (!set.Contains(words[i]) && appendWord(words[i])) set.Add(
                    preprocessingWord(words[i])
                    );
            
            return set;
        }

        /// <summary>
        /// Сходство текстов на базе множеств
        /// </summary>
        public static double SimTextDice(HashSet<string> set1, HashSet<string> set2)
        {
            double sim = 0;
            foreach (var item in set1)
                if (set2.Contains(item)) sim++;

            return 2 * sim / (set1.Count + set2.Count);
        }

        /// <summary>
        /// Асимvетричное сходство текстов на базе множеств
        /// </summary>
        public static double SimTextDiceAsymmetric(HashSet<string> main, HashSet<string> set)
        {
            double sim = 0;
            foreach (var item in main)
                if (set.Contains(item)) sim++;

            return sim / main.Count;
        }
    }
}
