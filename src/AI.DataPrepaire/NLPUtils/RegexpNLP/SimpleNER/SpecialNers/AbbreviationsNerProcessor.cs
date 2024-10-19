using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
    /// <summary>
    /// Класс для замены аббревиатур
    /// </summary>
    [Serializable]
    public class AbbreviationsNerProcessor : INerProcessor
    {
        /// <summary>
        /// Словарь преобразования нера в токен
        /// </summary>
        public Dictionary<string, string> NerToNerToken { get => _regexNer.NerToNerToken; }

        /// <summary>
        /// Словарь преобразования токена в Ner
        /// </summary>
        public Dictionary<string, string> NerTokenToNer { get => _regexNer.NerTokenToNer; }

        RegexNer _regexNer;

        /// <summary>
        /// Класс для замены аббревиатур
        /// </summary>
        /// <param name="abbreviations">Аббревиатуры</param>
        public AbbreviationsNerProcessor(IEnumerable<string> abbreviations) 
        {
            HashSet<string> abbsSet = new HashSet<string>();

            foreach (string abb in abbreviations)
                abbsSet.Add(abb.Trim(' '));

            string abbsStr = "";
            foreach (string abb in abbsSet)
                abbsStr += @$"{abb}|";

            abbsStr = abbsStr.Trim('|');

            _regexNer = new RegexNer(abbsStr, "abbr_not_ner");
        }

        /// <summary>
        /// Замена аббревиатуры на токен
        /// </summary>
        public string RunProcessor(string text)
        {
            return _regexNer.RunProcessor(text);
        }

        /// <summary>
        /// Замена токена на аббревиатуру
        /// </summary>
        public string NerDecoder(string text)
        {
            return _regexNer.NerDecoder(text);
        }
    }

}
