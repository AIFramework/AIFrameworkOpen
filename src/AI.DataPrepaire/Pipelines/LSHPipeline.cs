using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using AI.ML.AlgorithmAnalysis;
using AI.ML.Classifiers;
using AI.ML.Regression;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.DataPrepaire.Pipelines
{
    /// <summary>
    /// LSH Конвейер
    /// </summary>
    [Serializable]
    public class LSHPipeline<T>
    {
        Random random = new Random(1);

        /// <summary>
        /// Извлечение признаков из данных
        /// </summary>
        public FeaturesExtractor<T> Extractor { get; set; }

        /// <summary>
        /// Нормализация данных
        /// </summary>
        public Normalizer Normalizer { get; set; }

        /// <summary>
        /// Регрессия
        /// </summary>
        public IMultyRegression ProbRegr { get; set; }

        /// <summary>
        /// Детектор аномальных и/или неподходящих данных
        /// </summary>
        public IDetector<T> Detector { get; set; }




        /// <summary>
        /// Запуск регрессора (Возвращает вектор вероятностей)
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector GetProb(T input)
        {
            return ProbRegr.Predict(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }

        /// <summary>
        /// Запуск регрессора (Возвращает вектор вероятностей)
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector[] GetProb(T[] input)
        {
            Vector[] clProbs = new Vector[input.Length];

            for (int i = 0; i < clProbs.Length; i++)
            {
                clProbs[i] = GetProb(input[i]);
            }

            return clProbs;
        }


        /// <summary>
        /// Хэширование объектов
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual string GetHash(T input, double tr = 0.01)
        {
            Vector prob = GetProb(input);
            string lsh = string.Empty;

            for (int i = 0; i < prob.Count; i++)
                lsh += prob[i] < tr ? 0 : 1;

            return lsh;
        }

        /// <summary>
        /// Хэширование объектов
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual string[] GetHash(T[] input, double tr = 0.01)
        {
            string[] clProbs = new string[input.Length];

            for (int i = 0; i < clProbs.Length; i++)
                clProbs[i] = GetHash(input[i], tr);

            return clProbs;
        }


        /// <summary>
        /// Стохастическое хэширование объектов
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector GetStoсhasticHash(T input)
        {
            Vector prob = GetProb(input);
            Vector lsh = prob - Statistic.UniformDistribution(prob.Count, random);
            return lsh.Transform(x => x >= 0 ? 1 : 0);          
        }

        /// <summary>
        /// Стохастическое кэширование объектов
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector[] GetStoсhasticHash(T[] input)
        {
            Vector[] clProbs = new Vector[input.Length];

            for (int i = 0; i < clProbs.Length; i++)
                clProbs[i] = GetStoсhasticHash(input[i]);

            return clProbs;
        }


        /// <summary>
        /// Обучение ковейера
        /// </summary>
        /// <param name="data"></param>
        /// <param name="target"></param>
        public virtual void Train(IEnumerable<T> data, IEnumerable<Vector> target)
        {
            //Очистка данных
            DatasetForMR dataSamples = ClearData(data.ToArray(), target.ToArray());
            dataSamples.ShuffleData(); // Премешивание

            T[] datas = dataSamples.ReturnData();
            Vector[] probs = dataSamples.ReturnClasses();

            Vector[] features = new Vector[datas.Length];

            // Извлечение признаков
            for (int i = 0; i < datas.Length; i++)
                features[i] = Extractor.GetFeatures(datas[i]);

            // Обучение нормализатора
            Normalizer.Train(features);
            features = (Vector[])Normalizer.Transform(features); // Нормализация

            // Обучение классификатора
            ProbRegr.Train(features, probs);
        }

        /// <summary>
        /// Очистка данных
        /// </summary>
        public DatasetForMR ClearData(T[] dataIn, Vector[] classes)
        {
            DatasetForMR dataSamples = new DatasetForMR(classes.Length / 2);

            for (int i = 0; i < classes.Length; i++)
            {
                // Добавляем элементы, если мы знаем их класс и они не являются аномальными и/или неподходящими
                if (!Detector.IsDetected(dataIn[i]))
                    dataSamples.Add(dataIn[i], classes[i]);
            }

            return dataSamples;
        }

        /// <summary>
        /// Датасет для конвейера классификации
        /// </summary>
        [Serializable]
        public class DatasetForMR : List<DataSampleMR>
        {

            /// <summary>
            /// Датасет для конвейера классификации
            /// </summary>
            public DatasetForMR() : base() { }

            /// <summary>
            /// Датасет для конвейера классификации
            /// </summary>
            /// <param name="cap">Емкость коллекции</param>
            public DatasetForMR(int cap) : base(cap) { }

            /// <summary>
            /// Перемешать датасет
            /// </summary>
            public void ShuffleData()
            {
                this.Shuffle();
            }

            /// <summary>
            /// Вернуть данные
            /// </summary>
            public T[] ReturnData()
            {
                var ret = new T[Count];

                for (int i = 0; i < Count; i++)
                    ret[i] = this[i].Obj;

                return ret;
            }

            /// <summary>
            /// Вернуть метки классов
            /// </summary>
            /// <returns></returns>
            public Vector[] ReturnClasses()
            {
                var ret = new Vector[Count];

                for (int i = 0; i < Count; i++)
                    ret[i] = this[i].Probs;

                return ret;
            }

            /// <summary>
            /// Добавление объекта в выборку
            /// </summary>
            /// <param name="obj">Признаковое описание/param>
            /// <param name="probs">Индекс</param>
            public void Add(T obj, Vector probs)
            {
                Add(new DataSampleMR(obj, probs));
            }
        }

        /// <summary>
        /// Элемент датасета
        /// </summary>
        [Serializable]
        public class DataSampleMR
        {
            /// <summary>
            /// Вектор вероятностей
            /// </summary>
            public Vector Probs { get; set; }

            /// <summary>
            /// Признаковое описание
            /// </summary>
            public T Obj { get; set; }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            public DataSampleMR() { }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            /// <param name="probs">Вектор вероятностей</param>
            /// <param name="obj">Признаковое описание</param>
            public DataSampleMR(T obj, Vector probs)
            {
                Probs = probs;
                Obj = obj;
            }
        }
    }
}
