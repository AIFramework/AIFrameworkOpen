using System;
using System.Collections.Generic;
using System.Text;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER
{
    /// <summary>
    /// Обработчик NER
    /// </summary>
    public interface INerProcessor
    {
        /// <summary>
        /// Словарь преобразования нера в токен
        /// </summary>
        Dictionary<string, string> NerToNerToken { get; }
        /// <summary>
        /// Словарь преобразования токена в нер
        /// </summary>
        Dictionary<string, string> NerTokenToNer { get;}

        /// <summary>
        /// Преобразование нера в токен нера
        /// Например: "Встретимся в 22:00" -> "Встретимся в %time_1%"
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string RunProcessor(string text);

        /// <summary>
        /// Декодирует NER
        /// Например: "Встретимся в %time_1%" -> "Встретимся в 22:00"
        /// </summary>
        /// <param name="text"></param>
        string NerDecoder(string text);
    }
}
