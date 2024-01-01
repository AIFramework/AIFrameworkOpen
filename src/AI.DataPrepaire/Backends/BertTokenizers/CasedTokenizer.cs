using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Абстрактный класс CasedTokenizer, предоставляющий базовую реализацию токенизатора с учетом регистра.
    /// </summary>
    [Serializable]
    public abstract class CasedTokenizer : TokenizerBase
    {
        bool _splitWords = true;

        /// <summary>
        /// Инициализирует новый экземпляр класса CasedTokenizer.
        /// </summary>
        /// <param name="vocabularyFilePath">Путь к файлу словаря.</param>
        /// <param name="splitWords">Разделять ли по словам</param>
        protected CasedTokenizer(string vocabularyFilePath, bool splitWords) : base(vocabularyFilePath) 
        {
            _splitWords = splitWords;
        }

        /// <summary>
        /// Реализация метода токенизации предложения с учетом регистра.
        /// </summary>
        /// <param name="text">Входной текст для токенизации.</param>
        /// <returns>Перечисление токенов.</returns>
        protected override IEnumerable<string> TokenizeSentence(string text)
        {

            // Разбиваем текст на слова, используя пробелы и переводы строк в качестве разделителей.
            // Затем разбиваем каждое слово на токены с учетом дополнительных символов-разделителей.
            if (_splitWords)
                return text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None)
                    .SelectMany(o => o.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray()));

            else
                return new[] { text };
        }
    }

}
