using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
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

}
