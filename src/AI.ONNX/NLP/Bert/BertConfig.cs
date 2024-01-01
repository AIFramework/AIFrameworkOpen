using System;
using System.Collections.Generic;
using System.Text;

namespace AI.ONNX.NLP.Bert
{
    /// <summary>
    /// Конфигурация Bert
    /// </summary>
    [Serializable]
    public class BertConfig
    {
        public string NameOrPath { get; set; }
        public List<string> Architectures { get; set; }
        public double AttentionProbsDropoutProb { get; set; }
        public bool GradientCheckpointing { get; set; }
        public string HiddenAct { get; set; }
        public double HiddenDropoutProb { get; set; }
        public int HiddenSize { get; set; }
        public double InitializerRange { get; set; }
        public int IntermediateSize { get; set; }
        public double LayerNormEps { get; set; }
        public int MaxPositionEmbeddings { get; set; }
        public string ModelType { get; set; }
        public int NumAttentionHeads { get; set; }
        public int NumHiddenLayers { get; set; }
        public int PadTokenId { get; set; }
        public string PositionEmbeddingType { get; set; }
        public string TransformersVersion { get; set; }
        public int TypeVocabSize { get; set; }
        public bool UseCache { get; set; }
        public int VocabSize { get; set; }
    }
}
