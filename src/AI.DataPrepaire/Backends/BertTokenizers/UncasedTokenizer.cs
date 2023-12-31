using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Абстрактный класс UncasedTokenizer, предоставляющий базовую реализацию токенизатора без учета регистра.
    /// </summary>
    [Serializable]
    public abstract class UncasedTokenizer : TokenizerBase
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса UncasedTokenizer.
        /// </summary>
        /// <param name="vocabularyFilePath">Путь к файлу словаря.</param>
        protected UncasedTokenizer(string vocabularyFilePath) : base(vocabularyFilePath)
        {
        }

        /// <summary>
        /// Реализация метода токенизации предложения без учета регистра.
        /// </summary>
        /// <param name="text">Входной текст для токенизации.</param>
        /// <returns>Перечисление токенов.</returns>
        protected override IEnumerable<string> TokenizeSentence(string text)
        {
            // Разбиваем текст на слова, используя пробелы и переводы строк в качестве разделителей.
            // Затем разбиваем каждое слово на токены с учетом дополнительных символов-разделителей,
            // и приводим их к нижнему регистру.
            return text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None)
                .SelectMany(o => o.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray()))
                .Select(o => o.ToLower());
        }
    }

}
