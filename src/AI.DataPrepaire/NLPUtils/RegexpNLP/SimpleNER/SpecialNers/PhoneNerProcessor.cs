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

}
