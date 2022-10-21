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
            output = output.Replace("—", "-").Replace("--", "-");


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

            string outp = input.ToLower().Replace("\n", " ").Replace("\t", " ").Replace("ё", "е");
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
        public static string OnlyChars(string input)
        {
            List<char> chars = new List<char>();

            string outp = input.ToLower().Replace("\n", " ").Replace("\t", " ").Replace("ё", "е");

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

            string outp = input.ToLower().Replace("\n", " ").Replace("\t", " ").Replace("ё", "е");

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
    }
}
