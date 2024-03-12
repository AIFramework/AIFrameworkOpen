using System.Collections.Generic;

namespace AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER
{
    /// <summary>
    /// Реализация обработчика NER.
    /// </summary>
    public abstract class NerProcessor : INerProcessor
    {
        /// <summary>
        /// Счетчик для уникальных токенов
        /// </summary>
        protected int TokenCounter { get; set; } = 1;

        /// <summary>
        /// Словарь преобразования нера в токен
        /// </summary>
        public Dictionary<string, string> NerToNerToken { get; protected set; }

        /// <summary>
        /// Словарь преобразования токена в нер
        /// </summary>
        public Dictionary<string, string> NerTokenToNer { get; protected set; }

        /// <summary>
        /// Реализация обработчика NER.
        /// </summary>
        public NerProcessor()
        {
            // Инициализация словарей
            NerToNerToken = new Dictionary<string, string>();
            NerTokenToNer = new Dictionary<string, string>();
        }

        /// <summary>
        /// Преобразует текст, заменяя элементы NER на их токены.
        /// </summary>
        /// <param name="text">Текст для обработки</param>
        /// <returns>Текст с замененными элементами NER на токены</returns>
        public abstract string RunProcessor(string text);

        /// <summary>
        /// Декодирует токены NER обратно в их исходный текст.
        /// </summary>
        /// <param name="text">Текст с токенами NER для декодирования</param>
        /// <returns>Текст с восстановленными элементами NER</returns>
        public string NerDecoder(string text)
        {
            foreach (var pair in NerTokenToNer)
            {
                text = text.Replace(pair.Key, pair.Value);
            }
            return text;
        }
    }

}
