﻿using AI.DataPrepaire.Backends.BertTokenizers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers
{
    /// <summary>
    /// Класс BertTokenizer представляет токенизатор для модели BERT.
    /// </summary>
    [Serializable]
    public class BertTokenizer : TokenizerBase
    {
        /// <summary>
        /// Конфигурация токенизатора
        /// </summary>
        public BertTokenizerConfig TokenizerConfig { get; set; } = new BertTokenizerConfig();

        /// <summary>
        /// Инициализирует новый экземпляр класса BertTokenizer.
        /// </summary>
        /// <param name="path">Путь к файлу словаря.</param>
        /// <param name="isUnCased">Учитывать ли регистр при токенизации</param>
        public BertTokenizer(string path, bool isUnCased = true) : base(path)
        {
            TokenizerConfig.DoLowerCase = isUnCased;
        }

        // ToDo: Реализовать
        /// <summary>
        /// Загрузка пред. обученного токенизатора Bert
        /// </summary>
        /// <param name="pathToFolder">Путь до папки</param>
        public BertTokenizer FromPretrained(string pathToFolder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Токенизация последовательности
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected override IEnumerable<string> TokenizeSentence(string text)
        {
            return BertSentenceTokenize(TokenizerConfig.DoLowerCase ? text.ToLower() : text);
        }


        private List<string> BertSentenceTokenize(string text)
        {
            List<string> result = new List<string>();

            // Разбиваем текст на слова, используя пробелы и переводы строк в качестве разделителей.
            string[] words = text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None);

            foreach (string word in words)
            {
                // Затем разбиваем каждое слово на токены с учетом дополнительных символов-разделителей.
                IEnumerable<string> tokens = word.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray());

                // Добавляем полученные токены в общий результат.
                result.AddRange(tokens);
            }

            return result;
        }
    }
}
