using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers
{
    /// <summary>
    /// Класс представляет конфигурацию для BERT токенизатора.
    /// </summary>
    [Serializable]
    public class BertTokenizerConfig
    {
        /// <summary>
        /// Определяет, следует ли приводить текст к нижнему регистру при токенизации.
        /// </summary>
        [JsonPropertyName("do_lower_case")]
        public bool DoLowerCase { get; set; }

        /// <summary>
        /// Метка для неизвестных слов в токенизаторе
        /// </summary>
        [JsonPropertyName("unk_token")]
        public string UnknownToken { get; set; }

        /// <summary>
        /// Метка для разделения токенов
        /// </summary>
        [JsonPropertyName("sep_token")]
        public string SeparationToken { get; set; }

        /// <summary>
        /// Метка для паддинга (пустых токенов)
        /// </summary>
        [JsonPropertyName("pad_token")]
        public string PaddingToken { get; set; }

        /// <summary>
        /// Метка для токена классификации
        /// </summary>
        [JsonPropertyName("cls_token")]
        public string ClassificationToken { get; set; }

        /// <summary>
        /// Метка для маскировки токенов при обучении
        /// </summary>
        [JsonPropertyName("mask_token")]
        public string MaskToken { get; set; }

        /// <summary>
        /// Определяет, следует ли токенизировать китайские символы отдельно
        /// </summary>
        [JsonPropertyName("tokenize_chinese_chars")]
        public bool TokenizeChineseChars { get; set; }

        /// <summary>
        /// Параметр, указывающий на необходимость удаления диакритических знаков при токенизации.
        /// </summary>
        [JsonPropertyName("strip_accents")]
        public object StripAccents { get; set; }

        /// <summary>
        /// Путь к предобученной модели или ее имя
        /// </summary>
        [JsonPropertyName("name_or_path")]
        public string NameOrPath { get; set; }

        /// <summary>
        /// Определяет, следует ли использовать базовую токенизацию
        /// </summary>
        [JsonPropertyName("do_basic_tokenize")]
        public bool DoBasicTokenize { get; set; }

        /// <summary>
        /// Параметр для токенизатора, указывающий на токены, которые не следует разбивать
        /// </summary>
        [JsonPropertyName("never_split")]
        public object NeverSplit { get; set; }

        /// <summary>
        /// Класс токенизатора, используемый для обработки текста
        /// </summary>
        [JsonPropertyName("tokenizer_class")]
        public string TokenizerClass { get; set; }

        /// <summary>
        /// Максимальная длина контекста модели в токенах
        /// </summary>
        [JsonPropertyName("model_max_length")]
        public int ModelMaxLength { get; set; }


        /// <summary>
        /// Загрузка конфигурации из JSON
        /// </summary>
        public static BertTokenizerConfig FromJson(string jsonConfigPath)
        {
            string json = File.ReadAllText(jsonConfigPath);
            var tokens = JsonSerializer.Deserialize<BertTokenizerConfig>(json);
            return tokens;
        }
    }
}
