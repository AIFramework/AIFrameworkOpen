using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
    /// <summary>
    /// Экстрактор телефонных номеров.
    /// </summary>
    [Serializable]
    public class PhoneNerProcessor : RegexNer
    {
        /// <summary>
        /// Экстрактор телефонных номеров
        /// </summary>
        public PhoneNerProcessor() :
            base(@"\+?[1-9](?:[\d\-\(\)\s]{5,}\d)", "phone")
        { }
    }


    /// <summary>
    /// Экстрактор ФИО и ИОФ
    /// </summary>
    [Serializable]
    public class NameRusNerProcessor : RegexNer
    {

        const string namePatten =
            @"\b([А-ЯЁ][а-яё]+)?\s*([А-ЯЁ]\.\s*[А-ЯЁ]\.)\s*([А-ЯЁ][а-яё]+)?\b";

        /// <summary>
        /// Экстрактор ФИО и ИОФ
        /// </summary>
        public NameRusNerProcessor() :
            base(namePatten, "name_rus")
        { }
    }

    /// <summary>
    /// Экстрактор адресов сайтов
    /// </summary>
    [Serializable]
    public class SiteAdressProcessor : RegexNer
    {

        const string urlPatten =
            @"\b(?:https?:\/\/)?[\w.-]+\.[a-zA-Z]{2,}(?:\/\S*)?\b";

        /// <summary>
        /// Экстрактор адресов сайтов
        /// </summary>
        public SiteAdressProcessor() :
            base(urlPatten, "site")
        { }
    }

    /// <summary>
    /// Экстрактор адресов почты
    /// </summary>
    [Serializable]
    public class EmailAdressProcessor : RegexNer
    {

        const string mailPatten =
            @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";

        /// <summary>
        /// Экстрактор адресов почты
        /// </summary>
        public EmailAdressProcessor() :
            base(mailPatten, "mail")
        { }
    }

    /// <summary>
    /// Экстрактор адресов
    /// </summary>
    [Serializable]
    public class AdressProcessor : RegexNer
    {

        const string adressPatten =
            @"\bул\.\s+[А-Яа-яЁё\s]+,\s*д\.\s*\d+,\s*кв\.\s*\d+\b";

        /// <summary>
        /// Экстрактор адресов
        /// </summary>
        public AdressProcessor() :
            base(adressPatten, "adress_rus")
        { }
    }

    /// <summary>
    /// Экстрактор времени
    /// </summary>
    [Serializable]
    public class TimeProcessor : RegexNer
    {

        const string timePatten =
            @"\b([01]?\d|2[0-3]):([0-5]\d)\b|\b([01]?\d|2[0-3])[-.]([0-5]\d)\b";

        /// <summary>
        /// Экстрактор адресов
        /// </summary>
        public TimeProcessor() :
            base(timePatten, "time")
        { }
    }

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
