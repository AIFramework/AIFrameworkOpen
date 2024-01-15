using AI.DataPrepaire.DataLoader.NNWBlockLoader;
using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.ONNX.NLP.Bert
{
    /// <summary>
    /// Эмбеддер последовательностей на базе Bert
    /// </summary>
    [Serializable]
    public class BertEmbedder
    {
        public BertInfer BertInference { get; set; }
        public BertTokenizer Tokenizer { get; set; }
        public BertConfig Config { get; set; } = new BertConfig();
        public List<INNWBlockV2V> V2VBlocks = new List<INNWBlockV2V>(); 

        /// <summary>
        /// Эмбеддер последовательностей на базе Bert
        /// </summary>
        /// <param name="tokenizer">Токенизатор</param>
        /// <param name="model">Модель</param>
        public BertEmbedder(BertTokenizer tokenizer, BertInfer model)
        {
            BertInference = model;
            Tokenizer = tokenizer;
        }

        /// <summary>
        /// Эмбеддер последовательностей на базе Bert
        /// </summary>
        public BertEmbedder() { }

        /// <summary>
        /// Прямой проход, преобразует всю последовательность (текст) в эмбеддинг
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Vector ForwardSBert(string text) 
        {
            var outputBert = ForwardBert(text);
            var output = Vector.Mean(outputBert.ToArray()); // ToDo: Добавить логику для моделей с одним выходом
            
            if (V2VBlocks.Count > 0) 
                foreach (var block in V2VBlocks)
                    output = block.Forward(output);

            return output;
        }

        /// <summary>
        /// Прямой проход, преобразует каждый токен в эмбеддинг
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerable<Vector> ForwardBert(string text) 
        {
            var tokens = Tokenizer.Encode(text);
            var output = BertInference.Forward(tokens.InputIds, tokens.AttentionMask, tokens.TypeIds)[0];

            int numVectors = output.Count / Config.HiddenSize;
            Vector[] vectors = new Vector[numVectors];

            for (int i = 0; i < numVectors; i++)
            {
                vectors[i] = new Vector(Config.HiddenSize);
                int ofset = i*Config.HiddenSize;

                for (int j = 0; j < Config.HiddenSize; j++)
                    vectors[i][j] = output[ofset + j];
            }

            return vectors;
        }
    }
}
