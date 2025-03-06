using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
    /// <summary>
    /// Экстрактор времени
    /// </summary>
    [Serializable]
    public class TimeProcessor : RegexNer
    {

        const string timePatten =
            @"\b([01]?\d|2[0-3]):([0-5]\d)\b";

        /// <summary>
        /// Экстрактор адресов
        /// </summary>
        public TimeProcessor() :
            base(timePatten, "time")
        { }
    }

}
