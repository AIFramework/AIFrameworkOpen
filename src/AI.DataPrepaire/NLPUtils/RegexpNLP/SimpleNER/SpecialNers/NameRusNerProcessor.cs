using System;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers
{
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

}
