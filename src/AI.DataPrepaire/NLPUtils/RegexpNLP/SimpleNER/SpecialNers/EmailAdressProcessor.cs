using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
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

}
