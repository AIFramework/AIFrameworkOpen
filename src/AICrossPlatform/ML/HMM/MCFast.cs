using AI.DataStructs.Algebraic;
using AI.DataStructs;
using AI.Extensions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using AI.NLP;
using System.Text;
using System.Xml.Schema;
using System.Linq;
using AI.DataStructs.Data;

namespace AI.ML.HMM
{
    /// <summary>
    /// Марковская цепь (Быстрый алгоритм)
    /// </summary>
    [Serializable]
    public class MCFast
    {
        #region Поля и свойства

        /*
         Данные модели, словарь в котором ключ представляет собой начало n-граммы, а значение словарь завершений, в котором в свою очередь, 
        ключ - это завершение, а значение - вероятность того, что n-грамма завершится именно так 
         */
        private Dictionary<int[], Dictionary<int, double>> _dataMC = new Dictionary<int[], Dictionary<int, double>>(new IntArrayEqualityComparer());
        // Карта ключей, для создания вектора
        private List<McMapElement> _map = new List<McMapElement>();
        private bool _setedWLim = false;
        private HashSet<int> _wList; // Список разрешенных токенов
        /// <summary>
        /// Токен начала
        /// </summary>
        public int StartToken { get; set; } = -1;
        /// <summary>
        /// Токен окончания
        /// </summary>
        public int EndToken { get; set; } = -2;
        /// <summary>
        /// Глубина моделирования
        /// </summary>
		public int NGram { get; set; } = 3;
        /// <summary>
        /// Вектор вероятностей
        /// </summary>
		public Vector ProbabilityVector { get; set; }

        #endregion

        #region Конструкторы
        /// <summary>
        /// Быстрые марковские цепи
        /// </summary>
        public MCFast() { }
        
        #endregion

        /// <summary>
        /// Обучение модели
        /// </summary>
        /// <param name="trainSeq">Тренировочная последовательность</param>
        /// <param name="addStart">Добавлять ли старт вначале</param>
        public void Train(int[] trainSeq, bool addStart = false)
        {
            if (trainSeq == null)
                throw new ArgumentNullException(nameof(trainSeq));


            // ----------------- ДАТАСЕТ С ДОБАВЛЕННЫМ СТАРТОМ ----------------//
            List<int> trainTextFinal = new List<int>(trainSeq.Length);

            if (addStart)
                for (int i = 0; i < NGram - 1; i++)
                    trainTextFinal.Add(StartToken);

            trainTextFinal.AddRange(trainSeq); // Добвление последовательности

            // -------------------------------------------------------------//


            int ngramCount = trainTextFinal.Count - NGram+1, // Число проходов
                lenBuff = NGram - 1; // Длинна буфера (ключа)
            int[] buffNgMinus1 = new int[lenBuff];
            double probIncrease = 1.0/trainTextFinal.Count; // Прирост вероятности при новом упоминании

            for (int i = 0; i < ngramCount; i++)
            {
                for (int j = 0; j < lenBuff; j++)
                    buffNgMinus1[j] = trainTextFinal[i + j]; // Сбор n-грамм без завершения


                int[] key = CopyKey(buffNgMinus1);// Копирование(перезапись) ключа
                int endKey = trainTextFinal[i + lenBuff]; // Ключ завершения n граммы

                
                if (_dataMC.ContainsKey(key)) // Если нам известно начало n-граммы
                {
                    if (_dataMC[key].ContainsKey(endKey)) // Если нам известно завершение n-граммы
                        _dataMC[key][endKey] += probIncrease; // Корректирование статистики
                    else // Если нам неизвестно завершение n-граммы
                        _dataMC[key].Add(endKey, probIncrease); // Добавление завершения
                }
                else // Если нам неизвестно начало n-граммы
                {
                    _dataMC.Add(key, new Dictionary<int, double>());// Добавление n-граммы
                    _dataMC[key].Add(endKey, probIncrease); // Добавление завершения
                }
            }

            CreateVector();
        }

        /// <summary>
        /// Белый список токенов
        /// </summary>
        public void SetLimitationsWList(int[] wList) 
        {
            if (wList == null)
                throw new ArgumentNullException("wList имеет тип null");
            if (wList.Length == 0)
                throw new ArgumentException("wList - пустой массив");

            _wList = new HashSet<int>();

            foreach (var item in wList)
                if (!_wList.Contains(item)) _wList.Add(item);

            _setedWLim = true;
        }
        /// <summary>
		/// Генерация текста
		/// </summary>
		/// <param name="num">Число токенов</param>
		/// <returns>Сгенерированная строка</returns>
		public int[] Generate(int num)
        {
            Random random = new Random();

            int[] tokens = new int[NGram-1];

            for (int i = 0; i < tokens.Length; i++)
                tokens[i] = StartToken;

            return Generate(num, tokens, random);
        }
        /// <summary>
        /// Генерация текста
        /// </summary>
        /// <param name="num">число слов</param>
        /// <param name="tokens">начальное состояние</param>
        /// <returns>сгенерированная строка</returns>
        public int[] Generate(int num, int[] tokens)
        {
            return Generate(num, tokens, new Random());
        }
        /// <summary>
        /// Генерация текста
        /// </summary>
        /// <param name="num">число слов</param>
        /// <param name="tokens">начальное состояние</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        /// <returns>сгенерированная строка</returns>
        public int[] Generate(int num, int[] tokens, Random rnd)
        {
            if (num <= 0)
                throw new ArgumentOutOfRangeException(nameof(num), "Длинна последовательности должна быть больше нуля");

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (rnd == null)
                throw new ArgumentNullException(nameof(rnd));

            int lenBuff = NGram - 1;
            
            List<int> generatedSeqList = new List<int>(num + NGram); // сгенерированная строка
            MCNextToken[] nextToken; // массив "следующих токенов", концы н-грам с соот. вероятностями
            bool stop = false;

            // Установка затравки
            for (int i = lenBuff; i != -1; i--)
                generatedSeqList.Add(tokens[lenBuff-i]);
            

            for (int i = 0; i < num && !stop; i++)
            {

                nextToken = CalculateProbabilityNGramm(generatedSeqList);
                if (nextToken==null) break; // Если нет продолжения

                int counter = 0, // счетчик
                max = nextToken.Length; // максимальный индекс

                while (true)
                {
                    int idx = rnd.Next(max);
                    if (rnd.NextDouble() < nextToken[idx].Probability)
                    {
                        generatedSeqList.Add(nextToken[idx].Value);
                        if (generatedSeqList[i] == StartToken || generatedSeqList[i] == EndToken)
                        {
                            stop = true;
                            break;
                        }
                        break;
                    }

                    if (counter > num * 100) break;
                    counter++;
                }
            }

            return ToOutArray(generatedSeqList, tokens.Length);
        }
        /// <summary>
        /// Преобразование последовательности в вектор + изменение модели
        /// </summary>
        /// <param name="seq">Последовательность</param>
        /// <returns>вектор</returns>
        public Vector SeqToVector(int[] seq)
        {
            Vector ret = new Vector(_map.Count);
            int ngramCount = seq.Length - NGram + 1, // Число проходов
                lenBuff = NGram - 1; // Длинна буфера (ключа)
            int[] buffNgMinus1 = new int[lenBuff];

            for (int i = 0; i < ngramCount; i++)
            {
                for (int j = 0; j < lenBuff; j++)
                    buffNgMinus1[j] = seq[i + j]; // Сбор n-грамм без завершения

                int[] key = CopyKey(buffNgMinus1);// Копирование(перезапись) ключа
                int endKey = seq[i + lenBuff]; // Ключ завершения n граммы
                int index = GetIndex(key, endKey);// Индекс в карте ключей
                if (index >= 0) ret[index]++;
            }

            return ret/ret.Sum();
        }

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Сохранение Марковской модели в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранение Марковской модели в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузка марковской модели из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static MCFast Load(string path)
        {
            return BinarySerializer.Load<MCFast>(path);
        }
        /// <summary>
        /// Загрузка марковской модели из потока
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static MCFast Load(Stream stream)
        {
            return BinarySerializer.Load<MCFast>(stream);
        }
        #endregion

        #endregion

        #region Приватные методы
        // Получить индекс в карте ключей
        private int GetIndex(int[] key, int end) 
        {
            McMapElement mcMap = new McMapElement(key, end);

            for (int i = 0; i < _map.Count; i++)
                if (_map[i] == mcMap) return i;
            return -1;
        }

        // Копирует ключ
        private int[] CopyKey(int[] key) 
        {
            int[] keyCopy = new int[key.Length];
            Array.Copy(key, 0, keyCopy, 0, key.Length);
            return keyCopy;
        }

        // Создание вектора
        private void CreateVector() 
        {
            // Создание карты
            _map.Clear();
            // Создание карты ключей
            foreach (var item in _dataMC)
                foreach (var ends in item.Value)
                    _map.Add(new McMapElement(item.Key, ends.Key));
            // вероятностный вектор
            ProbabilityVector = new Vector(_map.Count); 
            // Создание вектора вероятностей
            for (int i = 0; i < _map.Count; i++)
                ProbabilityVector[i] = _dataMC[_map[i].KeyStart][_map[i].KeyEnd];
        }

        /// <summary>
        /// Вычисление вероятностей токенов
        /// </summary>
        /// <param name="start">Начальная последовательность</param>
        public MCNextToken[] CalculateProbabilityNGramm(List<int> start)
        {
            int index = start.Count - NGram+1;
            int lenBuff = NGram - 1;
            int[] buffNgMinus1 = new int[lenBuff];

            for (int j = 0; j < lenBuff; j++)
                buffNgMinus1[j] = start[index + j];

            int[] key = CopyKey(buffNgMinus1);// Копирование(перезапись) ключа

            if (!_dataMC.ContainsKey(key)) return null;

            var data = _dataMC[key];
            int countNext = data.Count;
            List<MCNextToken> nextToken = new List<MCNextToken>(countNext);// массив "следующих токенов", концы н-грам с соот. вероятностями
            double denom = 0;

            if (_setedWLim) // При активном ограничении (белый список)
            {
                foreach (var item in data)
                    if (_wList.Contains(item.Key)) denom += item.Value;

                if (denom == 0) return null; // Знаменатель равен нулю, значит нет элементов

                foreach (var item in data)
                    if (_wList.Contains(item.Key)) nextToken.Add(new MCNextToken(item.Key, item.Value / denom));
            }
            else // Без него
            {
                foreach (var item in data) denom += item.Value;
                foreach (var item in data)
                    nextToken.Add(new MCNextToken(item.Key, item.Value / denom));
            }


            return nextToken.ToArray();
        }

        // Получить выходной массив
        private int[] ToOutArray(List<int> listWithStart, int startLen) 
        {
            int lenBuff = NGram - 1;
            int[] outArray = new int[listWithStart.Count - startLen];

            for (int i = startLen; i < listWithStart.Count; i++)
                outArray[i- startLen] = listWithStart[i];

            return outArray;
        }
        #endregion

        /// <summary>
        /// Элемент карты марковской цепи
        /// </summary>
        [Serializable]
        public class McMapElement
        {
            /// <summary>
            /// Ключ начала n-gramm
            /// </summary>
            public int[] KeyStart;

            /// <summary>
            /// Ключ завершения n-gramm
            /// </summary>
            public int KeyEnd;

            /// <summary>
            /// Элемент карты марковской цепи
            /// </summary>
            /// <param name="keyStart">Ключ начала n-gramm</param>
            /// <param name="keyEnd">Ключ завершения n-gramm</param>
            public McMapElement(int[] keyStart, int keyEnd) 
            {
                KeyStart = keyStart;
                KeyEnd = keyEnd;
            }

        }
    }

}
