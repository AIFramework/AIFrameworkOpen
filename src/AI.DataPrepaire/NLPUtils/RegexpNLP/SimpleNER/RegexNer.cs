using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER
{
    /// <summary>
    /// NER на базе регулярных выражений
    /// </summary>
    [Serializable]
    public class RegexNer : NerProcessor
    {
        /// <summary>
        /// Паттерн
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Имя токена NER
        /// </summary>
        public string NameToken { get; set; }

        /// <summary>
        /// Экстрактор NER на базе 
        /// </summary>
        public RegexNer(string pattern, string nameToken) : base()
        {
            Pattern = pattern;
            NameToken = nameToken;
        }


        /// <summary>
        /// Замена номера на токен
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override string RunProcessor(string text)
        {
            var matches = Regex.Matches(text, Pattern);

            foreach (Match match in matches)
            {
                var phone = match.Value;
                if (!NerToNerToken.ContainsKey(phone))
                {
                    var token = $"%{NameToken}_{TokenCounter++}%";
                    NerToNerToken.Add(phone, token);
                    NerTokenToNer.Add(token, phone);
                }

                // Замена номера на токен в тексте
                text = text.Replace(phone, NerToNerToken[phone]);
            }

            return text;
        }
    }
}
