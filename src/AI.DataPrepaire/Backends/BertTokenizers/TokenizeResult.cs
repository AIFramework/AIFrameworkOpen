using System;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Структура TokenizeResult представляет результат токенизации текста.
    /// </summary>
    [Serializable]
    public struct TokenizeResult
    {
        /// <summary>
        /// Массив идентификаторов входных токенов.
        /// </summary>
        public int[] InputIds { get; set; }

        /// <summary>
        /// Массив масок внимания (attention mask).
        /// </summary>
        public int[] AttentionMask { get; set; }

        /// <summary>
        /// Массив идентификаторов типов токенов.
        /// </summary>
        public int[] TypeIds { get; set; }
    }

}
