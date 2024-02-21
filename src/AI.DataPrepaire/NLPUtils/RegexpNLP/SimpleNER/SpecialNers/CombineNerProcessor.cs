using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
    /// <summary>
    /// Комбинированный NER
    /// </summary>
    [Serializable]
    public class CombineNerProcessor
    {
        RegexNer[] _regexNers;

        /// <summary>
        /// Комбинированный NER
        /// </summary>
        public CombineNerProcessor()
        {
            // Важно что mail перед site
            _regexNers = new RegexNer[]
            {
                new TimeProcessor(),
                new EmailAdressProcessor(),
                new SiteAdressProcessor(),
                new AdressProcessor(),
                new PhoneNerProcessor(),
                new NameRusNerProcessor()
            };
        }

        /// <summary>
        /// Запуск сегментации текста
        /// </summary>
        /// <param name="text"></param>
        public string RunProcessor(string text)
        {
            string outStr = text;

            foreach (var item in _regexNers)
                outStr = item.RunProcessor(outStr);

            return outStr;
        }


        /// <summary>
        /// Запуск декодирования текста
        /// </summary>
        /// <param name="text"></param>
        public string NerDecoder(string text)
        {
            string outStr = text;

            foreach (var item in _regexNers)
                outStr = item.NerDecoder(outStr);

            return outStr;
        }
    }

}
