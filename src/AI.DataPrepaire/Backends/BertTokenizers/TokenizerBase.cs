using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AI.DataPrepaire.Backends.BertTokenizers
{
    /// <summary>
    /// Абстрактный базовый класс для токенизаторов.
    /// </summary>
    [Serializable]
    public abstract class TokenizerBase
    {

        /// <summary>
        /// Специальные токены
        /// </summary>
        public SpecialTokens SpecialTokenMap { get; set; } = new SpecialTokens();

        /// <summary>
        /// Список слов из словаря.
        /// </summary>
        protected readonly List<string> _vocabulary;

        /// <summary>
        /// Словарь, отображающий слово в его индекс в словаре
        /// </summary>
        protected readonly Dictionary<string, int> _vocabularyDict;

        /// <summary>
        /// Инициализирует новый экземпляр класса TokenizerBase.
        /// </summary>
        /// <param name="vocabularyFilePath">Путь к файлу словаря.</param>
        protected TokenizerBase(string vocabularyFilePath)
        {
            // Загрузка словаря из файла.
            _vocabulary = File.ReadAllLines(vocabularyFilePath).ToList();

            // Создание словаря, сопоставляющего слово его индексу в словаре.
            _vocabularyDict = new Dictionary<string, int>();
            for (int i = 0; i < _vocabulary.Count; i++)
                _vocabularyDict[_vocabulary[i]] = i;
        }

        /// <summary>
        /// Кодирует входные тексты в виде токенов и возвращает список кортежей (InputIds, TokenTypeIds, AttentionMask).
        /// </summary>
        /// <param name="texts">Входные тексты для токенизации.</param>
        /// <param name="sequenceLength">Длина последовательности токенов.</param>
        /// <returns>Список кортежей (InputIds, TokenTypeIds, AttentionMask).</returns>
        public virtual List<(int InputIds, int TokenTypeIds, int AttentionMask)> Encode(IEnumerable<string> texts, int sequenceLength = 0)
        {
            // Получение токенов из текстов.
            var tokens = Tokenize(texts.ToArray());

            // Добавление паддинга для соблюдения длины последовательности.
            var padding = sequenceLength - tokens.Count > 0 ? Enumerable.Repeat(0, sequenceLength - tokens.Count).ToList() : new List<int>(0);

            // Формирование списков для InputIds, TokenTypeIds и AttentionMask.
            var tokenIndexes = tokens.Select(token => token.VocabularyIndex).Concat(padding).ToArray();
            var segmentIndexes = tokens.Select(token => token.SegmentIndex).Concat(padding).ToArray();
            var inputMask = tokens.Select(o => 1).Concat(padding).ToArray();

            // Сборка результатов в список кортежей.
            var output = tokenIndexes.Zip(segmentIndexes, Tuple.Create)
                .Zip(inputMask, (t, z) => Tuple.Create(t.Item1, t.Item2, z));

            return output.Select(x => (InputIds: x.Item1, TokenTypeIds: x.Item2, AttentionMask: x.Item3)).ToList();
        }

        /// <summary>
        /// Кодирует входные тексты в виде токенов и возвращает список кортежей (InputIds, TokenTypeIds, AttentionMask).
        /// </summary>
        /// <param name="text">Входной текст для токенизации</param>
        /// <param name="sequenceLength">Длина последовательности токенов.</param>
        /// <returns>Список кортежей (InputIds, TokenTypeIds, AttentionMask).</returns>
        public virtual List<(int InputIds, int TokenTypeIds, int AttentionMask)> Encode(string text, int sequenceLength = 0)
        {
            // Получение токенов из текстов.
            var tokens = Tokenize(text);

            // Добавление паддинга для соблюдения длины последовательности.
            var padding = sequenceLength - tokens.Count > 0 ? Enumerable.Repeat(0, sequenceLength - tokens.Count).ToList() : new List<int>(0);

            // Формирование списков для InputIds, TokenTypeIds и AttentionMask.
            var tokenIndexes = tokens.Select(token => token.VocabularyIndex).Concat(padding).ToArray();
            var segmentIndexes = tokens.Select(token => token.SegmentIndex).Concat(padding).ToArray();
            var inputMask = tokens.Select(o => 1).Concat(padding).ToArray();

            // Сборка результатов в список кортежей.
            var output = tokenIndexes.Zip(segmentIndexes, Tuple.Create)
                .Zip(inputMask, (t, z) => Tuple.Create(t.Item1, t.Item2, z));

            return output.Select(x => (InputIds: x.Item1, TokenTypeIds: x.Item2, AttentionMask: x.Item3)).ToList();
        }

        /// <summary>
        /// Кодирует входные тексты в виде токенов
        /// </summary>
        /// <param name="texts">Входные тексты для токенизации.</param>
        /// <param name="sequenceLength">Длина последовательности токенов.</param>
        /// <returns>Структуру с токенами</returns>
        public virtual TokenizeResult Encode2Struct(IEnumerable<string> texts, int sequenceLength = 0)
        {
            var enc = Encode(texts, sequenceLength);

            var hfTokens = new TokenizeResult()
            {
                InputIds = enc.Select(t => t.InputIds).ToArray(),
                AttentionMask = enc.Select(t => t.AttentionMask).ToArray(),
                TypeIds = enc.Select(t => t.TokenTypeIds).ToArray(),
            };

            return hfTokens;
        }

        /// <summary>
        /// Кодирует входные тексты в виде токенов
        /// </summary>
        /// <param name="text">Входной текст для токенизации</param>
        /// <param name="sequenceLength">Длина последовательности токенов</param>
        /// <returns>Структуру с токенами</returns>
        public virtual TokenizeResult Encode2Struct(string text, int sequenceLength = 0)
        {
            var enc = Encode(text, sequenceLength);

            var hfTokens = new TokenizeResult()
            {
                InputIds = enc.Select(t => t.InputIds).ToArray(),
                AttentionMask = enc.Select(t => t.AttentionMask).ToArray(),
                TypeIds = enc.Select(t => t.TokenTypeIds).ToArray(),
            };

            return hfTokens;
        }

        /// <summary>
        /// Возвращает слово по его индексу в словаре.
        /// </summary>
        /// <param name="id">Индекс слова в словаре.</param>
        /// <returns>Слово.</returns>
        public string IdToToken(int id)
        {
            return _vocabulary[id];
        }

        /// <summary>
        /// Объединяет список токенов в единое предложение.
        /// </summary>
        /// <param name="tokens">Список токенов.</param>
        /// <returns>Единое предложение.</returns>
        public virtual List<string> Untokenize(List<string> tokens)
        {
            // Реверс списка токенов для правильной последовательности.
            var currentToken = string.Empty;
            var untokens = new List<string>();
            tokens.Reverse();

            tokens.ForEach(token =>
            {
                if (token.StartsWith("##"))
                    currentToken = token.Replace("##", "") + currentToken;
                else
                {
                    currentToken = token + currentToken;
                    untokens.Add(currentToken);
                    currentToken = string.Empty;
                }
            });

            // Реверс полученного результата для правильной последовательности.
            untokens.Reverse();

            return untokens;
        }

        /// <summary>
        /// Токенизирует входные тексты и возвращает список токенов вместе с их индексами в словаре и индексами сегментов.
        /// </summary>
        /// <param name="texts">Входные тексты для токенизации.</param>
        /// <returns>Список токенов вместе с индексами в словаре и индексами сегментов.</returns>
        public virtual List<(string Token, int VocabularyIndex, int SegmentIndex)> Tokenize(params string[] texts)
        {
            // Исходное множество токенов содержит только метку классификации.
            IEnumerable<string> tokens = new string[] { SpecialTokenMap.Classification };

            // ToDo: Сделать параллельную обработку
            foreach (var text in texts)
            {
                tokens = tokens.Concat(TokenizeSentence(text));
                tokens = tokens.Concat(new string[] { SpecialTokenMap.Separation });
            }

            // Получение токенов и их индексов в словаре и сегментов.
            var tokenAndIndex = tokens
                .SelectMany(TokenizeSubwords)
                .ToList();

            var segmentIndexes = SegmentIndex(tokenAndIndex);

            // Формирование результирующего списка токенов с индексами и сегментами.
            return tokenAndIndex.Zip(segmentIndexes, (tokenindex, segmentindex)
                                => (tokenindex.Token, tokenindex.VocabularyIndex, segmentindex)).ToList();
        }

        /// <summary>
        /// Возвращает индексы сегментов для списка токенов.
        /// </summary>
        /// <param name="tokens">Список токенов.</param>
        /// <returns>Список индексов сегментов.</returns>
        private IEnumerable<int> SegmentIndex(List<(string token, int index)> tokens)
        {
            var segmentIndex = 0;
            var segmentIndexes = new List<int>();

            foreach (var (token, index) in tokens)
            {
                segmentIndexes.Add(segmentIndex);

                if (token == SpecialTokenMap.Separation)
                    segmentIndex++;
            }

            return segmentIndexes;
        }

        /// <summary>
        /// Токенизирует подслова входного слова.
        /// </summary>
        /// <param name="word">Входное слово.</param>
        /// <returns>Список токенов вместе с их индексами в словаре.</returns>
        private IEnumerable<(string Token, int VocabularyIndex)> TokenizeSubwords(string word)
        {
            if (_vocabularyDict.TryGetValue(word, out int vocabIndex))
                return new[] { (word, vocabIndex) };

            var tokens = new List<(string, int)>();
            var remaining = word;
            int wordLength = word.Length;

            while (wordLength > 2)
            {
                string prefix = null;
                int subwordLength = remaining.Length;

                while (subwordLength >= 1)
                {
                    string subword = remaining.Substring(0, subwordLength);

                    if (_vocabularyDict.TryGetValue(subword, out int subwordIndex))
                    {
                        prefix = subword;
                        tokens.Add((prefix, subwordIndex));
                        break;
                    }

                    subwordLength--;
                }

                if (prefix == null)
                {
                    tokens.Add((SpecialTokenMap.Unknown, _vocabularyDict[SpecialTokenMap.Unknown]));
                    return tokens;
                }

                remaining = $"##{remaining.Substring(prefix.Length)}";
                wordLength = remaining.Length;
            }

            if (!string.IsNullOrWhiteSpace(word) && tokens.Count == 0)
                tokens.Add((SpecialTokenMap.Unknown, _vocabularyDict[SpecialTokenMap.Unknown]));

            return tokens;
        }

        /// <summary>
        /// Абстрактный метод для токенизации предложения.
        /// </summary>
        /// <param name="text">Текст для токенизации.</param>
        /// <returns>Перечисление токенов.</returns>
        protected abstract IEnumerable<string> TokenizeSentence(string text);
    }

}
