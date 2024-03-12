using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
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

}
