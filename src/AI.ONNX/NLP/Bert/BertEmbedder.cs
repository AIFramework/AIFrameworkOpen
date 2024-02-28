using AI.DataPrepaire.Backends.BertTokenizers;
using AI.DataPrepaire.DataLoader.NNWBlockLoader;
using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        /// Очистка строки
        /// </summary>
        public Func<string, string> Cleaner { get; set; }

        /// <summary>
        /// Используется в методе ForwardSBert
        /// Нарезать ли текст на блоки (увеличивает котекст и скорость, ухудшает качество)
        /// </summary>
        public bool IsCutting { get; set; } = true;

        /// <summary>
        /// Используется в методе ForwardSBert
        /// Размер блока, при включенной нарезке (чем меньше блок, тем выше скорость, но хуже качество)
        /// </summary>
        public int BlockSize { get; set; } = 512;

        /// <summary>
        /// Очистка строки
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CleanString(string text)
        {
            string seq = text.Replace('\n', ' ');
            Regex rChar = new Regex("[^A-zА-яЁё0-9\": ]");
            Regex rSpaces = new Regex("\\s+");
            seq = rChar.Replace(seq, " ");
            seq = rSpaces.Replace(seq, " ").Trim();
            return seq.ToLower();
        }

        /// <summary>
        /// Эмбеддер последовательностей на базе Bert
        /// </summary>
        /// <param name="tokenizer">Токенизатор</param>
        /// <param name="model">Модель</param>
        public BertEmbedder(BertTokenizer tokenizer, BertInfer model)
        {
            Cleaner = CleanString;
            BertInference = model;
            Tokenizer = tokenizer;
        }

        /// <summary>
        /// Эмбеддер последовательностей на базе Bert
        /// </summary>
        public BertEmbedder() { Cleaner = CleanString; }

        /// <summary>
        /// Прямой проход, преобразует всю последовательность (текст) в эмбеддинг
        /// </summary>
        /// <param name="text">Текст для векторизации</param>
        /// <returns></returns>
        public Vector ForwardSBert(string text)
            => IsCutting ? ForwardSBertBlocks(text) : ForwardSBertWithoutBlocs(text);

        /// <summary>
        /// Поблочная векторизация текста с учетом контекста
        /// </summary>
        /// <param name="texts">Тексты (блоки)</param>
        /// <returns>Векторизованные представления блоков</returns>
        public Vector[] ForwardBlockPooling(IEnumerable<string> texts)
        {
            if (!texts.Any())
                return Array.Empty<Vector>();

            TokenizeResult[] tokenizeResults = BlockTokenize(texts.ToArray());
            TokenizeResult tokens = JoinTokens(tokenizeResults);
            var output = BertInference.Forward(tokens.InputIds, tokens.AttentionMask, tokens.TypeIds)[0];
            Vector[] embeddings = Vector2Vectors(output);
            List<Vector> results = new List<Vector>(tokenizeResults.Length);

            int indexInEmbeddings = 0;
            foreach (var tokenizeResult in tokenizeResults)
            {
                int blockLength = tokenizeResult.AttentionMask.Length;
                Vector blockVector = new Vector(Config.HiddenSize);

                for (int j = 0; j < blockLength; j++)
                    blockVector += embeddings[indexInEmbeddings++];

                blockVector /= blockLength + AISettings.GlobalEps;
                blockVector = OutpTransform(blockVector);
                results.Add(blockVector);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Векторизация текста с учетом весов блоков
        /// </summary>
        /// <param name="texts">Тексты (блоки)</param>
        /// <param name="blockWeights">Веса для каждого блока</param>
        /// <returns>Агрегированный вектор</returns>
        public Vector ForwardBlockPooling(IEnumerable<string> texts, IEnumerable<double> blockWeights)
        {
            if (!texts.Any() || !blockWeights.Any())
            throw new ArgumentException("Тексты и веса блоков не должны быть пустыми.");

            Vector[] vectors = ForwardBlockPooling(texts);
            if (vectors.Length != blockWeights.Count())
                throw new ArgumentException("Количество весов должно соответствовать количеству текстовых блоков.");

            Vector weights = new Vector(blockWeights.ToArray());
            weights /= weights.Sum();

            Vector output = new Vector(vectors[0].Count);

            for (int i = 0; i < vectors.Length; i++)
                output += weights[i] * vectors[i];

            return output;
        }


        // Прямой проход с разбивкой на блоки по blockSize символов
        private Vector ForwardSBertBlocks(string text)
        {
            // Если размер блока меньше текста
            if(text.Length<=BlockSize) return ForwardSBertWithoutBlocs(text);

            int nBlocs = text.Length/BlockSize;
            int mod = text.Length % BlockSize;
            
            // Рассчет векторов с учетом нарезки
            Vector output = ForwardSBertWithoutBlocs(
                 text.Substring(0, BlockSize)
                );

            for (int i = 0; i < nBlocs; i++)
                output += ForwardSBertWithoutBlocs(
                    text.Substring(i*BlockSize, BlockSize));

            if (mod != 0)
                output += ForwardSBertWithoutBlocs(
                    text.Substring(nBlocs * BlockSize, mod));

            // Рассчет среднего
            output /= mod == 0 ? nBlocs : nBlocs + 1;

            return output;
        }

        // Прямой проход с без разбивки на блоки
        private Vector ForwardSBertWithoutBlocs(string text)
        {
            var outputBert = ForwardBert(text);
            var output = Vector.Mean(outputBert.ToArray());

            output = OutpTransform(output);

            return output;
        }




        /// <summary>
        /// Прямой проход, преобразует каждый токен в эмбеддинг
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerable<Vector> ForwardBert(string text) 
        {
            var tokens = Tokenizer.Encode(
                Cleaner(text));

            var output = BertInference.Forward(tokens.InputIds, tokens.AttentionMask, tokens.TypeIds)[0];

            return Vector2Vectors(output);
        }


        /// <summary>
        /// Загрузка пред. обученного эмбедера
        /// </summary>
        /// <param name="pathToFolder"></param>
        /// <returns></returns>
        public static BertEmbedder FromPretrained(string pathToFolder) 
        {
            BertTokenizer tokenizer = new BertTokenizer($"{pathToFolder}\\vocab.txt");
            tokenizer.TokenizerConfig = BertTokenizerConfig.FromJson($"{pathToFolder}\\tokenizer_config.json");
            BertInfer model = new BertInfer($"{pathToFolder}\\model.onnx");
            BertEmbedder embedder = new BertEmbedder(tokenizer, model);
            embedder.Config = BertConfig.FromJson($"{pathToFolder}\\config.json");
            return embedder;
        }



        /// <summary>
        /// Преобразование выхода
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private Vector OutpTransform(Vector output)
        {
            if (V2VBlocks.Count > 0)
                foreach (var block in V2VBlocks)
                    output = block.Forward(output);

            return output;
        }

        // Разбивка вектора на эмбеддинги
        private Vector[] Vector2Vectors(Vector outputBert)
        {
            int numVectors = outputBert.Count / Config.HiddenSize;
            Vector[] vectors = new Vector[numVectors];

            for (int i = 0; i < numVectors; i++)
            {
                vectors[i] = new Vector(Config.HiddenSize);
                int ofset = i * Config.HiddenSize;

                for (int j = 0; j < Config.HiddenSize; j++)
                    vectors[i][j] = outputBert[ofset + j];
            }

            return vectors;
        }

        // Поблочная токенизация
        private TokenizeResult[] BlockTokenize(string[] texts)
        {
            TokenizeResult[] tokenizeResults = new TokenizeResult[texts.Length];

            for (int k = 0; k < texts.Length; k++)
            {
                string textWithSp = texts[k].Trim() + " ";
                tokenizeResults[k] = Tokenizer.Encode(Cleaner(textWithSp));

                // Удаление токенов в начале или в конце, если это необходимо
                bool isFirstBlock = k == 0;
                bool isLastBlock = k == tokenizeResults.Length - 1;

                if (tokenizeResults.Length > 1)
                    AdjustTokenizeResult(tokenizeResults[k], isFirstBlock, isLastBlock);
            }

            return tokenizeResults;
        }

        // Переписывание токенов
        private void AdjustTokenizeResult(TokenizeResult result, bool isFirstBlock, bool isLastBlock)
        {
            // Смещение для всех кроме первого (удаляем токен начала)
            int startOffset = !isFirstBlock ? 1 : 0;

            // Из первого и последнего вычитается 1, из средних и токен начала и токен конца
            int newLength = !(isFirstBlock || isLastBlock)? result.InputIds.Length - 2: result.InputIds.Length - 1;

            int[] newAttentionMask = new int[newLength];
            int[] newInputIds = new int[newLength];
            int[] newTypeIds = new int[newLength];

            for (int i = 0; i < newLength; i++)
            {
                newAttentionMask[i] = result.AttentionMask[i + startOffset];
                newInputIds[i] = result.InputIds[i + startOffset];
                newTypeIds[i] = result.TypeIds[i + startOffset];
            }

            result.AttentionMask = newAttentionMask;
            result.InputIds = newInputIds;
            result.TypeIds = newTypeIds;
        }

        // Объединение токенов по всем блокам
        private TokenizeResult JoinTokens(TokenizeResult[] tokenizeResults)
        {
            var totalLength = tokenizeResults.Sum(tr => tr.InputIds.Length);

            var joinedTokens = new TokenizeResult
            {
                AttentionMask = new int[totalLength],
                InputIds = new int[totalLength],
                TypeIds = new int[totalLength]
            };

            int currentPosition = 0;
            foreach (var tokenizeResult in tokenizeResults)
            {
                Array.Copy(tokenizeResult.InputIds, 0, joinedTokens.InputIds, currentPosition, tokenizeResult.InputIds.Length);
                Array.Copy(tokenizeResult.AttentionMask, 0, joinedTokens.AttentionMask, currentPosition, tokenizeResult.AttentionMask.Length);
                Array.Copy(tokenizeResult.TypeIds, 0, joinedTokens.TypeIds, currentPosition, tokenizeResult.TypeIds.Length);

                currentPosition += tokenizeResult.InputIds.Length;
            }

            return joinedTokens;
        }
    }
}
