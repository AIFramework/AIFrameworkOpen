﻿using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using AI.ML.AlgorithmAnalysis;
using AI.ML.Classifiers;
using AI.ML.DataSets;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines
{
    /// <summary>
    /// Конвейер обработки данных, классификация объекта
    /// </summary>
    [Serializable]
    public abstract class ObjectClassifierPipeline<T>
    {

        Random random1 = new Random(1);


        /// <summary>
        /// Индекс неизвестного класса
        /// </summary>
        public int UnknowClass { get; set; } = -1;

        /// <summary>
        /// Извлечение признаков из данных
        /// </summary>
        public FeaturesExtractor<T> Extractor { get; set; }

        /// <summary>
        /// Нормализация данных
        /// </summary>
        public Normalizer Normalizer { get; set; }

        /// <summary>
        /// Классификатор
        /// </summary>
        public IClassifier Classifier { get; set; }

        /// <summary>
        /// Детектор аномальных и/или неподходящих данных
        /// </summary>
        public IDetector<T> Detector { get; set; }

        /// <summary>
        /// Конвейер обработки данных, классификация объекта
        /// </summary>
        public ObjectClassifierPipeline()
        {
            
        }

        /// <summary>
        /// Запуск классификатора
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual int Classify(T input)
        {
            return Classifier.Classify(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }

        /// <summary>
        /// Запуск классификатора
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual int[] Classify(IEnumerable<T> input)
        {
            int[] ints = new int[input.Count()];

            int i = 0;
            foreach (T inputItem in input)
                ints[i++] = Classify(inputItem);
            
            return ints;
        }

        /// <summary>
        /// Запуск классификатора (Возвращает вектор)
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector ClassifyProb(T input)
        {
            return Classifier.ClassifyProbVector(
                (Vector)Normalizer.Transform(
                    Extractor.GetFeatures(input)));
        }

        /// <summary>
        /// Запуск классификатора (Возвращает векторы распределений)
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector[] ClassifyProb(T[] input)
        {
            Vector[] clProbs = new Vector[input.Length];

            for (int i = 0; i < clProbs.Length; i++)
            {
                clProbs[i] = ClassifyProb(input[i]);
            }

            return clProbs;
        }

        /// <summary>
        /// Классификация объекта на базе распределения
        /// </summary>
        /// <param name="input">Входной объект</param>
        public virtual int StoсhasticClassify(T input)
        {
            return RandomItemSelection<T>.GetIndex(ClassifyProb(input), random1);
        }

        /// <summary>
        /// Классификация объекта на базе распределения
        /// </summary>
        /// <param name="input">Входной объект</param>
        public virtual int[] StoсhasticClassify(T[] input)
        {
            int[] clsIds = new int[input.Length];

            for (int i = 0; i < clsIds.Length; i++)
                 clsIds[i] = StoсhasticClassify(input[i]);
            
            return clsIds;
        }

        /// <summary>
        /// Обучение ковейера
        /// </summary>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        public virtual void Train(IEnumerable<T> data, IEnumerable<int> labels)
        {
            //Очистка данных
            DatasetForClassifier dataSamples = ClearData(data.ToArray(), labels.ToArray());
            dataSamples.ShuffleData(); // Премешивание

            T[] datas = dataSamples.ReturnData();
            int[] marks = dataSamples.ReturnClasses();

            Vector[] features = new Vector[datas.Length];

            // Извлечение признаков
            for (int i = 0; i < datas.Length; i++)
                features[i] = Extractor.GetFeatures(datas[i]);
            
            // Обучение нормализатора
            Normalizer.Train(features);
            features = (Vector[])Normalizer.Transform(features); // Нормализация

            // Обучение классификатора
            Classifier.Train(features, marks);
        }

        /// <summary>
        /// Обучение и тестирование
        /// </summary>
        /// <param name="data"></param>
        /// <param name="marks"></param>
        /// <returns></returns>
        public virtual string TrainTest(T[] data, int[] marks, double trainPart = 0.9, int seed = 0, bool reportForEachClass = true)
        {
            List<T> trainX = new List<T>((int)(trainPart * data.Length));
            List<int> trainY = new List<int>((int)(trainPart * data.Length));


            List<T> testX = new List<T>((int)((1-trainPart) * data.Length));
            List<int> testY = new List<int>((int)((1-trainPart) * data.Length));

            Random random = new Random(seed);

            // Разделение на обучающую и тестовую выборку
            for (int i = 0; i < data.Length; i++)
            {
                if (random.NextDouble() <= trainPart) 
                {
                    trainX.Add(data[i]);
                    trainY.Add(marks[i]);
                }
                else 
                {
                    testX.Add(data[i]);
                    testY.Add(marks[i]);
                }
            }

            Train(trainX, trainY);

            return MetricsForClassification.FullReport(Classify(testX), testY.ToArray(), isForEachClass:reportForEachClass);
        }

        /// <summary>
        /// Очистка данных
        /// </summary>
        public DatasetForClassifier ClearData(T[] dataIn, int[] classes) 
        {
            DatasetForClassifier dataSamples = new DatasetForClassifier(classes.Length/2);

            for (int i = 0; i < classes.Length; i++)
            {
                // Добавляем элементы, если мы знаем их класс и они не являются аномальными и/или неподходящими
                if (classes[i] != UnknowClass && !Detector.IsDetected(dataIn[i]))
                    dataSamples.Add(dataIn[i], classes[i]);  
            }

            return dataSamples;
        }





        /// <summary>
        /// Датасет для конвейера классификации
        /// </summary>
        [Serializable]
        public class DatasetForClassifier : List<DataSample>
        {

            /// <summary>
            /// Датасет для конвейера классификации
            /// </summary>
            public DatasetForClassifier() : base() { }

            /// <summary>
            /// Датасет для конвейера классификации
            /// </summary>
            /// <param name="cap">Емкость коллекции</param>
            public DatasetForClassifier(int cap) : base(cap) { }

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
            public int[] ReturnClasses() 
            {
                var ret = new int[Count];

                for (int i = 0; i < Count; i++)
                    ret[i] = this[i].ClassN;

                return ret;
            }

            /// <summary>
            /// Добавление объекта в выборку
            /// </summary>
            /// <param name="obj">Классифицируемый объект/param>
            /// <param name="clIndex">Индекс</param>
            public void Add(T obj, int clIndex)
            {
                Add(new DataSample(obj, clIndex));
            }
        }

        /// <summary>
        /// Элемент датасета
        /// </summary>
        [Serializable]
        public class DataSample
        {
            /// <summary>
            /// Индекс класса
            /// </summary>
            public int ClassN { get; set; }

            /// <summary>
            /// Классифицируемый объект
            /// </summary>
            public T Obj { get; set; }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            public DataSample() { }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            /// <param name="classN">Индекс класса</param>
            /// <param name="obj">Классифицируемый объект</param>
            public DataSample(T obj, int classN)
            {
                ClassN = classN;
                Obj = obj;
            }   
        }
    }

    
}