using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI.NLP
{
    /// <summary>
    /// Быстрые марковские цепи
    /// </summary>
    [Serializable]
    public class HMMFast : ISavable
    {
        #region Поля и свойства
        /// <summary>
        /// Элемент модели
        /// </summary>
        public HMMFastModel[] Models { get; private set; }
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
        public HMMFast() { }
        /// <summary>
        /// Быстрые марковские цепи
        /// </summary>
        public HMMFast(HMMFastModel[] models, Vector prob)
        {
            Models = new HMMFastModel[models.Length];
            NGram = models[0].Model.Length;
            Array.Copy(models, 0, Models, 0, models.Length);
            ProbabilityVector = prob.Clone();
            InvertedProbabilityVector = 0.9999 - (ProbabilityVector / prob.Max());
        }
        #endregion

        /// <summary>
        /// Обучение языковой модели
        /// </summary>
        /// <param name="trainText">Тренировочный текст</param>
        /// <param name="addStart">Добавлять ли старт вначале</param>
        public void Train(string trainText, bool addStart = false)
        {
            if (trainText == null)
            {
                throw new ArgumentNullException(nameof(trainText));
            }

            string trainTextFinal;

            if (addStart)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < NGram - 1; i++)
                {
                    sb.Append("<start> ");
                }

                trainTextFinal = sb.ToString() + trainText.ToLower(); // тренировочный текст
            }

            else
            {
                trainTextFinal = trainText.ToLower();
            }

            string[] words = trainTextFinal.Split(); // массив слов
            List<HMMFastModel> list = new List<HMMFastModel>(); // модель
            string[] nG = new string[NGram]; // n-Грамма

            for (int i = 0; i < NGram; i++)
            {
                nG[i] = words[i]; // Заполнение первой n-грамы
            }

            HMMFastModel data = new HMMFastModel(nG, 1);
            list.Add(data); // добавление н-грамы в список
            bool flag; // установка флага, который проверяет есть ли данная н-грама

            for (int i = 0; i < words.Length - NGram + 1; i++)
            {
                nG = new string[NGram]; // сброс н-грамы для повторного использования

                for (int k = 0; k < NGram; k++)
                {
                    nG[k] = words[i + k]; // заполнение новой н-грамы
                }
                flag = false; // флаг устанавливается в false

                for (int j = 0; j < list.Count; j++)
                {
                    // Сравненние массивов строк, если они равны возвращается true
                    if (nG.ElementWiseEqual(list[j].Model))
                    {
                        flag = true; // флаг устанавливается в true
                        list[j].Probability++; // Увеличение значения счетчика для данной н-грамы
                        break;// выход из цикла
                    }

                }
                //Если н-грамы нет — добавляем ее
                if (!flag)
                {
                    data = new HMMFastModel(nG, 1);
                    list.Add(data);
                }
            }

            ProbabilityVector = new Vector(list.Count); // вероятностный вектор

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Probability /= words.Length;
                ProbabilityVector[i] = list[i].Probability;
            }

            Models = list.ToArray(); // получение массива н-грам с вероятностями

            InvertedProbabilityVector = 0.9999 - (ProbabilityVector / Statistic.MaximalValue(ProbabilityVector)); // создание генеративного вектора
        }
        /// <summary>
		/// Генерация текста
		/// </summary>
		/// <param name="num">число слов</param>
		/// <returns>сгенерированная строка</returns>
		public string Generate(int num)
        {
            Random random = new Random();

            string[] strs = Models[random.Next(Models.Length)].Model;

            return Generate(num, strs, random);
        }
        /// <summary>
        /// Генерация текста
        /// </summary>
        /// <param name="num">число слов</param>
        /// <param name="strs">начальное состояние</param>
        /// <returns>сгенерированная строка</returns>
        public string Generate(int num, string[] strs)
        {
            return Generate(num, strs, new Random());
        }
        /// <summary>
        /// Генерация текста
        /// </summary>
        /// <param name="num">число слов</param>
        /// <param name="strs">начальное состояние</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        /// <returns>сгенерированная строка</returns>
        public string Generate(int num, string[] strs, Random rnd)
        {
            if (num <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(num), "Count of words must be positive value");
            }

            if (strs == null)
            {
                throw new ArgumentNullException(nameof(strs));
            }

            if (rnd == null)
            {
                throw new ArgumentNullException(nameof(rnd));
            }

            string[] strs2 = new string[num]; // Массив для генерации
            string[] wordBeg = new string[NGram - 1]; // начальное состояние н-граммы
            string str = string.Empty; // сгенерированная строка
            HMMNextWord[] nextWords; // массив "следующий слов", концы н-грам с соот. вероятностями
            bool stop = false;

            int i = 0;

            for (; i < NGram - 1; i++)
            {
                strs2[i] = strs[i];
            }

            for (; i < num && !stop; i++)
            {
                Array.Copy(strs2, i - NGram + 1, wordBeg, 0, NGram - 1);
                nextWords = FindInversProbabilityNGramm(wordBeg); // получения завершений н-граммы с соот вероятностями

                if (nextWords.Length == 0)
                {
                    break;
                }

                int counter = 0, // счетчик
                mZ = nextWords.Length; // модуль кольца вычетов

                while (true)
                {
                    if (rnd.NextDouble() > nextWords[counter % mZ].Probability)
                    {
                        strs2[i] = nextWords[counter % mZ].Value;
                        if (strs2[i] == "<start>" || strs2[i] == "<end>")
                        {
                            stop = true;
                            break;
                        }
                        str += " " + strs2[i];
                        break;
                    }

                    if (counter > num * 100)
                    {
                        break;
                    }

                    counter++;
                }
            }

            return str;
        }
        /// <summary>
        /// Преобразование текста в вектор + изменение модели
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns>вектор</returns>
        public Vector TextToVector(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            for (int i = 0; i < Models.Length; i++)
            {
                Models[i].Probability = 0;
            }

            string trainText = text.ToLower(); // тренировочный текст
            string[] words = trainText.Split(); // массив слов
            string[] nG; // n-Грамма
            Vector output;


            for (int i = 0; i < words.Length - NGram + 1; i++)
            {
                nG = new string[NGram]; // сброс н-грамы для повторного использования

                for (int k = 0; k < NGram; k++)
                {
                    nG[k] = words[i + k]; // заполнение новой н-грамы
                }

                for (int j = 0; j < Models.Length; j++)
                {
                    // Сравненние массивов строк, если они равны возвращается true
                    if (nG.ElementWiseEqual(Models[j].Model))
                    {
                        Models[j].Probability++; // Увеличение значения счетчика для данной н-грамы
                        break;// выход из цикла
                    }

                }
            }

            ProbabilityVector = new Vector(Models.Length); // вероятностный вектор

            for (int i = 0; i < Models.Length; i++)
            {
                Models[i].Probability /= words.Length;
                ProbabilityVector[i] = Models[i].Probability;
            }

            output = ProbabilityVector / Statistic.MaximalValue(ProbabilityVector);
            InvertedProbabilityVector = 0.9999 - output; // создание генеративного вектора

            return output;
        }

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Сохранениеs HMMFast to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранениеs HMMFast to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Loads HMMFast from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HMMFast Load(string path)
        {
            return BinarySerializer.Load<HMMFast>(path);
        }
        /// <summary>
        /// Loads HMMFast from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static HMMFast Load(Stream stream)
        {
            return BinarySerializer.Load<HMMFast>(stream);
        }
        #endregion

        #endregion

        #region Приватные методы
        private HMMNextWord[] FindInversProbabilityNGramm(string[] begin)
        {
            List<HMMNextWord> hmmList = new List<HMMNextWord>();
            bool flag;

            for (int i = 0; i < Models.Length; i++)
            {
                flag = true;

                for (int j = 0; j < NGram - 1; j++)
                {
                    if (begin[j] != Models[i].Model[j])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    hmmList.Add(new HMMNextWord(Models[i].Model[NGram - 1], InvertedProbabilityVector[i]));
                }
            }

            return hmmList.ToArray();
        }
        #endregion
    }

    /// <summary>
    /// Блок для сохранения
    /// </summary>
    [Serializable]
    public class HMMFastModel
    {
        /// <summary>
        /// N-грамма
        /// </summary>
		public string[] Model { get; }
        /// <summary>
        /// Вероятность
        /// </summary>
        public double Probability { get; set; }

        /// <summary>
        /// Создание параметров для хранения марковской цепи
        /// </summary>
        /// <param name="model"></param>
        /// <param name="probability"></param>
        public HMMFastModel(string[] model, double probability)
        {
            Model = model;
            Probability = probability;
        }
    }

    /// <summary>
    /// Слово
    /// </summary>
    public class HMMNextWord
    {
        /// <summary>
        /// Слово
        /// </summary>
		public string Value { get; }
        /// <summary>
        /// Вероятность
        /// </summary>
		public double Probability { get; }

        /// <summary>
        /// Слово
        /// </summary>
        public HMMNextWord(string val, double pr)
        {
            Value = val;
            Probability = pr;
        }
    }
}