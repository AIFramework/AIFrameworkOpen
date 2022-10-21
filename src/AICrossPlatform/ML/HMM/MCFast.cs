using AI.DataStructs.Algebraic;
using AI.DataStructs;
using AI.Extensions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.ML.HMM
{
    /// <summary>
    /// Марковская цепь (Быстрый алгоритм)
    /// </summary>
    [Serializable]
    public class MCFast
    {
        #region Поля и свойства

        /// <summary>
        /// Токен начала
        /// </summary>
        public int StartToken { get; set; } = -1;

        /// <summary>
        /// Токен окончания
        /// </summary>
        public int EndToken { get; set; } = -2;

        /// <summary>
        /// Элемент модели
        /// </summary>
        public MCFastModel[] Models { get; private set; }
        /// <summary>
        /// Глубина моделирования
        /// </summary>
		public int NGram { get; set; } = 3;
        /// <summary>
        /// Вектор вероятностей
        /// </summary>
		public Vector ProbabilityVector { get; set; }
        /// <summary>
        /// 1- вектор вероятностей, полезен для установки квантелей
        /// </summary>
		public Vector InvertedProbabilityVector { get; set; }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Быстрые марковские цепи
        /// </summary>
        public MCFast() { }
        /// <summary>
        /// Быстрые марковские цепи
        /// </summary>
        public MCFast(MCFastModel[] models, Vector prob)
        {
            Models = new MCFastModel[models.Length];
            NGram = models[0].Model.Length;
            Array.Copy(models, 0, Models, 0, models.Length);
            ProbabilityVector = prob.Clone();
            InvertedProbabilityVector = 0.9999 - (ProbabilityVector / prob.Max());
        }
        #endregion

        /// <summary>
        /// Обучение языковой модели
        /// </summary>
        /// <param name="trainSeq">Тренировочная последовательность</param>
        /// <param name="addStart">Добавлять ли старт вначале</param>
        public void Train(int[] trainSeq, bool addStart = false)
        {
            if (trainSeq == null)
                throw new ArgumentNullException(nameof(trainSeq));

            List<int> trainTextFinal = new List<int>(trainSeq.Length);

            if (addStart)
                for (int i = 0; i < NGram - 1; i++)
                    trainTextFinal.Add(StartToken);

            trainTextFinal.AddRange(trainSeq); // Добвление последовательности

            List<MCFastModel> list = new List<MCFastModel>(); // модель
            int[] nG = new int[NGram]; // n-Грамма

            for (int i = 0; i < NGram; i++)
                nG[i] = trainTextFinal[i]; // Заполнение первой n-грамы

            MCFastModel data = new MCFastModel(nG, 1);
            list.Add(data); // добавление н-грамы в список
            bool flag; // установка флага, который проверяет есть ли данная н-грама

            for (int i = 0; i < trainTextFinal.Count - NGram + 1; i++)
            {
                nG = new int[NGram]; // сброс н-грамы для повторного использования

                for (int k = 0; k < NGram; k++)
                    nG[k] = trainTextFinal[i + k]; // заполнение новой н-грамы

                flag = false; // флаг устанавливается в false

                for (int j = 0; j < list.Count; j++)
                    // Сравненние массивов строк, если они равны возвращается true
                    if (nG.ElementWiseEqual(list[j].Model))
                    {
                        flag = true; // флаг устанавливается в true
                        list[j].Probability++; // Увеличение значения счетчика для данной н-грамы
                        break;// выход из цикла
                    }


                //Если н-грамы нет — добавляем ее
                if (!flag)
                {
                    data = new MCFastModel(nG, 1);
                    list.Add(data);
                }
            }

            ProbabilityVector = new Vector(list.Count); // вероятностный вектор

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Probability /= trainTextFinal.Count;
                ProbabilityVector[i] = list[i].Probability;
            }

            Models = list.ToArray(); // получение массива н-грам с вероятностями

            InvertedProbabilityVector = 0.9999 - (ProbabilityVector / Statistic.MaximalValue(ProbabilityVector)); // создание генеративного вектора
        }
        /// <summary>
		/// Генерация текста
		/// </summary>
		/// <param name="num">Число токенов</param>
		/// <returns>Сгенерированная строка</returns>
		public int[] Generate(int num)
        {
            Random random = new Random();

            int[] tokens = Models[random.Next(Models.Length)].Model;

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
                throw new ArgumentOutOfRangeException(nameof(num), "Число слов должно быть больше нуля");

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            if (rnd == null)
                throw new ArgumentNullException(nameof(rnd));

            List<int> seqTokensGenerate = new List<int>(num + NGram); // Массив для генерации
            int[] startNgramState = new int[NGram - 1]; // начальное состояние н-граммы
            List<int> generatedSeqList = new List<int>(num + NGram); // сгенерированная строка
            MCNextToken[] nextToken; // массив "следующих токенов", концы н-грам с соот. вероятностями
            bool stop = false;

            int i = 0;

            for (; i < NGram - 1; i++)
                seqTokensGenerate.Add(tokens[i]);

            for (; i < num && !stop; i++)
            {
                // ToDo: Оптимизировать
                Array.Copy(seqTokensGenerate.ToArray(), i - NGram + 1, startNgramState, 0, NGram - 1);
                nextToken = FindInversProbabilityNGramm(startNgramState); // получения завершений н-граммы с соот вероятностями

                if (nextToken.Length == 0) break;

                int counter = 0, // счетчик
                mZ = nextToken.Length; // модуль кольца вычетов

                while (true)
                {
                    if (rnd.NextDouble() > nextToken[counter % mZ].Probability)
                    {
                        seqTokensGenerate.Add(nextToken[counter % mZ].Value);
                        if (seqTokensGenerate[i] == StartToken || seqTokensGenerate[i] == EndToken)
                        {
                            stop = true;
                            break;
                        }
                        generatedSeqList.Add(seqTokensGenerate[i]);
                        break;
                    }

                    if (counter > num * 100) break;
                    counter++;
                }
            }

            return generatedSeqList.ToArray();
        }
        /// <summary>
        /// Преобразование последовательности в вектор + изменение модели
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns>вектор</returns>
        public Vector SeqToVector(int[] text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            for (int i = 0; i < Models.Length; i++)
                Models[i].Probability = 0;

            int[] nG; // n-Грамма
            Vector output;


            for (int i = 0; i < text.Length - NGram + 1; i++)
            {
                nG = new int[NGram]; // сброс н-грамы для повторного использования

                for (int k = 0; k < NGram; k++)
                    nG[k] = text[i + k]; // заполнение новой н-грамы

                for (int j = 0; j < Models.Length; j++)
                    // Сравненние массивов строк, если они равны возвращается true
                    if (nG.ElementWiseEqual(Models[j].Model))
                    {
                        Models[j].Probability++; // Увеличение значения счетчика для данной н-грамы
                        break;// выход из цикла
                    }
            }

            ProbabilityVector = new Vector(Models.Length); // вероятностный вектор

            for (int i = 0; i < Models.Length; i++)
            {
                Models[i].Probability /= text.Length;
                ProbabilityVector[i] = Models[i].Probability;
            }

            output = ProbabilityVector / Statistic.MaximalValue(ProbabilityVector);
            InvertedProbabilityVector = 0.9999 - output; // создание генеративного вектора

            return output;
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
        private MCNextToken[] FindInversProbabilityNGramm(int[] begin)
        {
            List<MCNextToken> hmmList = new List<MCNextToken>();
            bool flag;

            for (int i = 0; i < Models.Length; i++)
            {
                flag = true;

                for (int j = 0; j < NGram - 1; j++)
                    if (begin[j] != Models[i].Model[j])
                    {
                        flag = false;
                        break;
                    }

                if (flag)
                    hmmList.Add(new MCNextToken(Models[i].Model[NGram - 1], InvertedProbabilityVector[i]));
            }

            return hmmList.ToArray();
        }
        #endregion
    }
}
