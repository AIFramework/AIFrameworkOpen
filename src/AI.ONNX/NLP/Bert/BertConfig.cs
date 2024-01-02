using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.ONNX.NLP.Bert
{
    /// <summary>
    /// Конфигурация Bert
    /// </summary>
    [Serializable]
    public class BertConfig
    {
        /// <summary>
        /// Получает или устанавливает свойство NameOrPath (имя модели)
        /// </summary>
        [JsonPropertyName("_name_or_path")]
        public string NameOrPath { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство Architectures (Архитектура модели)
        /// </summary>
        [JsonPropertyName("architectures")]
        public List<string> Architectures { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство AttentionProbsDropoutProb (Частота(вероятность) дропаута для слоя внимания)
        /// </summary>
        [JsonPropertyName("attention_probs_dropout_prob")]
        public double AttentionProbsDropoutProb { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство GradientCheckpointing
        /// </summary>
        [JsonPropertyName("gradient_checkpointing")]
        public bool GradientCheckpointing { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство HiddenAct (активационная функция в скрытом слое)
        /// </summary>
        [JsonPropertyName("hidden_act")]
        public string HiddenAct { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство HiddenDropoutProb (Частота(вероятность) дропаута для скрытого слоя)
        /// </summary>
        [JsonPropertyName("hidden_dropout_prob")]
        public double HiddenDropoutProb { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство HiddenSize (размерность скрытого слоя)
        /// </summary>
        [JsonPropertyName("hidden_size")]
        public int HiddenSize { get; set; } = 384;

        /// <summary>
        /// Получает или устанавливает свойство InitializerRange (разброс значений при инициализации)
        /// </summary>
        [JsonPropertyName("initializer_range")]
        public double InitializerRange { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство IntermediateSize (Промежуточная размерность)
        /// </summary>
        [JsonPropertyName("intermediate_size")]
        public int IntermediateSize { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство LayerNormEps (эпсилон для нормирующего слоя)
        /// </summary>
        [JsonPropertyName("layer_norm_eps")]
        public double LayerNormEps { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство MaxPositionEmbeddings (Длинна последовательности в токенах)
        /// </summary>
        [JsonPropertyName("max_position_embeddings")]
        public int MaxPositionEmbeddings { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство ClassifierDropout (Частота(вероятность) дропаута для классификатора)
        /// </summary>
        [JsonPropertyName("classifier_dropout")]
        public string ClassifierDropout { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство NumAttentionHeads (число голов внимания)
        /// </summary>
        [JsonPropertyName("num_attention_heads")]
        public int NumAttentionHeads { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство NumHiddenLayers (число скрытых слоев)
        /// </summary>
        [JsonPropertyName("num_hidden_layers")]
        public int NumHiddenLayers { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство PadTokenId (индекс добавочного пустого токена)
        /// </summary>
        [JsonPropertyName("pad_token_id")]
        public int PadTokenId { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство PositionEmbeddingType (Тип позиционного кодирования)
        /// </summary>
        [JsonPropertyName("position_embedding_type")]
        public string PositionEmbeddingType { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство TransformersVersion (Версия библиотеки transformers)
        /// </summary>
        [JsonPropertyName("transformers_version")]
        public string TransformersVersion { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство TypeVocabSize (Тип словаря)
        /// </summary>
        [JsonPropertyName("type_vocab_size")]
        public int TypeVocabSize { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство UseCache (используется ли кэш)
        /// </summary>
        [JsonPropertyName("use_cache")]
        public bool UseCache { get; set; }

        /// <summary>
        /// Получает или устанавливает свойство VocabSize (Размер словаря токенов)
        /// </summary>
        [JsonPropertyName("vocab_size")]
        public int VocabSize { get; set; }

        /// <summary>
        /// Загрузка конфигурации из JSON
        /// </summary>
        public static BertConfig FromJson(string jsonConfigPath)
        {
            string json = File.ReadAllText(jsonConfigPath);
            var tokens = JsonSerializer.Deserialize<BertConfig>(json);
            return tokens;
        }
    }
}
