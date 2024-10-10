using System;

namespace AI.NLP.Stemmers
{
    /// <summary>
    /// Получение окончаний слов (для русских слов)
    /// </summary>
    [Serializable]
    public class WordEndingsRU
    {
        /// <summary>
        /// Получение окончаний слов
        /// </summary>
        /// <param name="text">Текст входа</param>
        public static string[] Endings(string text)
        {
            string std_text = TextStandard.OnlyRusChars(text);
            string[] words = std_text.Split(' '); // слова
            string[] endings = new string[words.Length];

            for (int i = 0; i < words.Length; i++)
                try
                {
                    endings[i] = words[i].Diff(StemmerRus.TransformingWord(words[i]));
                }
                catch
                {
                    endings[i] = "";
                }
            return endings;
        }
    }
}
