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
        /// Набор аббревиатур, которые не следует интерпретировать как конец предложения.
        /// </summary>
        HashSet<string> Abbreviations { get; set; }

        /// <summary>
        /// Конструктор по умолчанию, инициализирующий класс с предопределённым набором сокращений.
        /// </summary>
        public SentencesTokenizer()
        {
            Abbreviations = new HashSet<string>(StringComparer.Ordinal)
            {
                "д", "кв", "р", "т.к", "тп", "пр", "г", "н.э", "до н.э"
            };
        }

        /// <summary>
        /// Конструктор, позволяющий задать собственный набор сокращений.
        /// </summary>
        /// <param name="abbreviations">Список сокращений, которые не следует интерпретировать как конец предложения.</param>
        public SentencesTokenizer(List<string> abbreviations)
        {
            Abbreviations = new HashSet<string>(StringComparer.Ordinal);

            foreach (string abbreviation in abbreviations)
                Abbreviations.Add(abbreviation);
        }

        /// <summary>
        /// Разделяет входной текст на предложения, учитывая сокращения.
        /// </summary>
        /// <param name="text">Текст для токенизации.</param>
        /// <returns>Список предложений, извлечённых из входного текста.</returns>
        public List<string> Tokenize(string text)
        {

            foreach (var abbreviation in Abbreviations)
                text = text.Replace(abbreviation + ". ", abbreviation + ".");

            var sentences = new List<string>();
            var buffer = text;

            while (buffer.Length > 0)
            {
                var match = Regex.Match(buffer, @"[\.!\?](\s+|$)");
                if (match.Success)
                {
                    var sentence = buffer.Substring(0, match.Index + 1);
                    var isAbbreviation = false;

                    foreach (var abbr in Abbreviations)
                        if (sentence.EndsWith(abbr + ".", StringComparison.OrdinalIgnoreCase))
                        {
                            isAbbreviation = true;
                            break;
                        }

                    if (!isAbbreviation)
                    {
                        sentences.Add(sentence.Trim());
                        buffer = buffer.Substring(match.Index + match.Length);
                    }
                    else
                    {
                        buffer = buffer.Substring(match.Index + match.Length);
                    }
                }
                else
                {
                    sentences.Add(buffer.Trim());
                    break;
                }
            }

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
