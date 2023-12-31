using System;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Класс, содержащий константы для меток токенов.
    /// </summary>
    [Serializable]
    public class Tokens
    {
        /// <summary>
        /// Пустой токен (паддинг).
        /// </summary>
        public const string Padding = "";

        /// <summary>
        /// Метка для неизвестных слов.
        /// </summary>
        public const string Unknown = "[UNK]";

        /// <summary>
        /// Метка для классификации.
        /// </summary>
        public const string Classification = "[CLS]";

        /// <summary>
        /// Метка разделения.
        /// </summary>
        public const string Separation = "[SEP]";

        /// <summary>
        /// Метка маскировки.
        /// </summary>
        public const string Mask = "[MASK]";
    }

}
