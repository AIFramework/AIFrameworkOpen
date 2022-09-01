using AI.DataPrepaire.DataNormalizers;
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

        internal Random random = new Random(1);

        /// <summary>
        /// Метод аугметации данных
        /// </summary>
        public DataAugmetation<Vector> DataAugmetation { get; set; }

        /// <summary>
        /// Метод реставрации данных
        /// </summary>
        public Func<T, T> DataRestavration { get; set; } = x => x;


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
                GetFeatures(input));
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
                GetFeatures(input));
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
        public virtual int StoсhasticClassify(T input, double temp = 1)
        {
            return RandomItemSelection<T>.GetIndex(ClassifyProb(input), random, temp);
        }

        /// <summary>
        /// Классификация объекта на базе распределения
        /// </summary>
        /// <param name="input">Входной объект</param>
        public virtual int[] StoсhasticClassify(T[] input, double temp = 1)
        {
            int[] clsIds = new int[input.Length];

            for (int i = 0; i < clsIds.Length; i++)
                 clsIds[i] = StoсhasticClassify(input[i], temp);
            
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
            DatasetForClassifier dataSamples = ClearData(
                DataArrayResvration(data), labels.ToArray());
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

            //Аугментация данных
            var dataNew = DataAugmetation.Augmetation(features, marks);
            dataNew.Shuffle();

            features = new Vector[dataNew.Length];
            marks = new int[dataNew.Length];

            for (int i = 0; i < features.Length; i++)
            {
                features[i] = dataNew[i].Item1;
                marks[i] = dataNew[i].Item2;
            }

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

            var dataset = ClearData(data, marks);

            List<T> trainX = new List<T>((int)(trainPart * dataset.Count));
            List<int> trainY = new List<int>((int)(trainPart * dataset.Count));


            List<T> testX = new List<T>((int)((1-trainPart) * dataset.Count));
            List<int> testY = new List<int>((int)((1-trainPart) * dataset.Count));

            Random random = new Random(seed);

            // Разделение на обучающую и тестовую выборку
            for (int i = 0; i < dataset.Count; i++)
            {
                if (random.NextDouble() <= trainPart) 
                {
                    trainX.Add(dataset[i].Obj);
                    trainY.Add(dataset[i].ClassN);
                }
                else 
                {
                    testX.Add(dataset[i].Obj);
                    testY.Add(dataset[i].ClassN);
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
        /// Восстановление массива данных
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public T[] DataArrayResvration(IEnumerable<T> data)
        {
            T[] dataArray = new T[data.Count()];
            int i = 0;
            foreach (var item in data)
                dataArray[i++] = DataRestavration(item);

            return dataArray;
        }

        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Vector GetFeatures(T data)
        {
            var input = DataRestavration(data);
            var features = Extractor.GetFeatures(input);
            return  (Vector)Normalizer.Transform(features);
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
