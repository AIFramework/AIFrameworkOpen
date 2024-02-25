using AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP
{
    /// <summary>
    /// Класс для токенизации текста на предложения, учитывая определённый набор сокращений.
    /// </summary>
    [Serializable]
    public class SentencesTokenizer
    {
       /// <summary>
       /// Обработчик аббревиатур 
       /// </summary>
       public AbbreviationsNerProcessor AbbreviationsProcessor { get; set; }

        /// <summary>
        /// Конструктор по умолчанию, инициализирующий класс с предопределённым набором сокращений.
        /// </summary>
        public SentencesTokenizer()
        {
            var abbreviations = new HashSet<string>(StringComparer.Ordinal)
            {
                "д.", "кв.", "ул.", "р.", @"т.\s*к.", "тп.", "пр.", "г.", @"н.\s*э.",
            };

            AbbreviationsProcessor = new AbbreviationsNerProcessor(abbreviations);
        }

        /// <summary>
        /// Конструктор, позволяющий задать собственный набор сокращений.
        /// </summary>
        /// <param name="abbreviations">Список сокращений, которые не следует интерпретировать как конец предложения.</param>
        public SentencesTokenizer(IEnumerable<string> abbreviations)
        {
            AbbreviationsProcessor = new AbbreviationsNerProcessor(abbreviations);
        }

        /// <summary>
        /// Разделяет входной текст на предложения, учитывая сокращения.
        /// </summary>
        /// <param name="text">Текст для токенизации.</param>
        /// <returns>Список предложений, извлечённых из входного текста.</returns>
        public List<string> Tokenize(string text)
        {

            var buffer = AbbreviationsProcessor.RunProcessor(text);
            var sentences = new List<string>();
            

            while (buffer.Length > 0)
            {
                var match = Regex.Match(buffer, @"[\.!\?](\s+|$)");
                if (match.Success)
                {
                    var sentence = buffer.Substring(0, match.Index + 1);
                    sentences.Add(sentence.Trim());
                    buffer = buffer.Substring(match.Index + match.Length);
                }
                else
                {
                    sentences.Add(buffer.Trim());
                    break;
                }
            }


            for (int i = 0; i < sentences.Count; i++)
                sentences[i] = AbbreviationsProcessor.NerDecoder(sentences[i]);

            return sentences;
        }


        /// <summary>
        /// Разделяет входной текст на предложения, учитывая сокращения и именованные сущности.
        /// </summary>
        /// <param name="text">Текст для токенизации.</param>
        /// <returns>Список предложений, извлечённых из входного текста.</returns>
        public List<string> TokenizeWithNer(string text)
        {
            CombineNerProcessor nerProcessor = new CombineNerProcessor();

            var textNer = nerProcessor.RunProcessor(text);
            var sentences = Tokenize(textNer);

            for (int i = 0; i < sentences.Count; i++)
                sentences[i] = nerProcessor.NerDecoder(sentences[i]);

            return sentences;
        }
    }
}
