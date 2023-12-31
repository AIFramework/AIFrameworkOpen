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
            var padding = sequenceLength - tokens.Count <=0 ? Enumerable.Repeat(0, sequenceLength - tokens.Count).ToList() : new List<int>(0);

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
            IEnumerable<string> tokens = new string[] { Tokens.Classification };

            // Добавление токенов для каждого текста и метку разделения.
            foreach (var text in texts)
            {
                tokens = tokens.Concat(TokenizeSentence(text));
                tokens = tokens.Concat(new string[] { Tokens.Separation });
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

                if (token == Tokens.Separation)
                {
                    segmentIndex++;
                }
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
            // Если слово уже есть в словаре, возвращаем его.
            if (_vocabularyDict.ContainsKey(word))
            {
                return new (string, int)[] { (word, _vocabularyDict[word]) };
            }

            // Иначе разбиваем слово на подслова.
            var tokens = new List<(string, int)>();
            var remaining = word;

            while (!string.IsNullOrEmpty(remaining) && remaining.Length > 2)
            {
                string prefix = null;
                int subwordLength = remaining.Length;
                while (subwordLength >= 1)
                {
                    string subword = remaining.Substring(0, subwordLength);
                    if (!_vocabularyDict.ContainsKey(subword))
                    {
                        subwordLength--;
                        continue;
                    }

                    prefix = subword;
                    break;
                }

                if (prefix == null)
                {
                    tokens.Add((Tokens.Unknown, _vocabularyDict[Tokens.Unknown]));

                    return tokens;
                }

                var regex = new Regex(prefix);
                remaining = regex.Replace(remaining, "##", 1);

                tokens.Add((prefix, _vocabularyDict[prefix]));
            }

            // Если осталось непустое слово и список токенов пуст, добавляем метку неизвестного слова.
            if (!string.IsNullOrWhiteSpace(word) && !tokens.Any())
            {
                tokens.Add((Tokens.Unknown, _vocabularyDict[Tokens.Unknown]));
            }

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
