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
        /// Пустой токен (паддинг)
        /// </summary>
        public string Padding = "";

        /// <summary>
        /// Метка для неизвестных слов
        /// </summary>
        public string Unknown = "[UNK]";

        /// <summary>
        /// Метка для классификации
        /// </summary>
        public string Classification = "[CLS]";

        /// <summary>
        /// Метка разделения
        /// </summary>
        public string Separation = "[SEP]";

        /// <summary>
        /// Метка маскировки
        /// </summary>
        public string Mask = "[MASK]";

        /// <summary>
        /// Загрузка токенов из JSON
        /// </summary>
        public void FromJson(string jsonMap)
        { 
            throw new NotImplementedException();
        }
    }

}
