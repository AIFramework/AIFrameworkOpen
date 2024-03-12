using AI.DataPrepaire.Backends.BertTokenizers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers
{
    /// <summary>
    /// Класс BertWithOutSplitWordTokenizer представляет токенизатор для модели BERT, без разделения по словам.
    /// </summary>
    [Serializable]
    public class BertWithOutSplitWordTokenizer : TokenizerBase
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
        public BertWithOutSplitWordTokenizer(string path, bool isUnCased = true) : base(path)
        {
            TokenizerConfig.DoLowerCase = isUnCased;
            TokenWordPart = "";
        }

        // ToDo: Реализовать
        /// <summary>
        /// Загрузка пред. обученного токенизатора Bert
        /// </summary>
        /// <param name="pathToFolder">Путь до папки</param>
        public BertWithOutSplitWordTokenizer FromPretrained(string pathToFolder)
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

            // Разбиваем текст на строки
            string[] lines = text.Split(new string[] { " ", "   ", "\r\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                string spCh = "▁" + line.Replace(" ", "▁");
                IEnumerable<string> tokens = spCh.SplitAndKeep(".,;:\\/?!#$%()=+-*\"'–_`<>&^@{}[]|~'".ToArray());
                result.AddRange(tokens);
            }

            return result;
        }
    }
}
