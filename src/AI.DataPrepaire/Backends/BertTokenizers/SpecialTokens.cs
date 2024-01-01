using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Класс, содержащий константы для меток токенов.
    /// </summary>
    [Serializable]
    public class SpecialTokens
    {
        /// <summary>
        /// Пустой токен (паддинг)
        /// </summary>
        [JsonPropertyName("pad_token")]
        public string Padding = "";

        /// <summary>
        /// Метка для неизвестных слов
        /// </summary>
        [JsonPropertyName("unk_token")]
        public string Unknown = "[UNK]";

        /// <summary>
        /// Метка для классификации
        /// </summary>
        [JsonPropertyName("cls_token")]
        public string Classification = "[CLS]";

        /// <summary>
        /// Метка разделения
        /// </summary>
        [JsonPropertyName("sep_token")]
        public string Separation = "[SEP]";

        /// <summary>
        /// Метка маскировки
        /// </summary>
        [JsonPropertyName("mask_token")]
        public string Mask = "[MASK]";


        /// <summary>
        /// Загрузка токенов из JSON
        /// </summary>
        public static SpecialTokens FromJson(string jsonMapPath)
        { 
            string json = File.ReadAllText(jsonMapPath);
            SpecialTokens tokens = JsonSerializer.Deserialize<SpecialTokens>(json);
            return tokens;
        }
    }

}
