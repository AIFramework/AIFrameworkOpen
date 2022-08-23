using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataStructs.Algebraic;
using AI.ML.AlgorithmAnalysis;
using AI.ML.Classifiers;
using AI.ML.DataSets;
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
        /// Обучение ковейера
        /// </summary>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        public virtual void Train(IEnumerable<T> data, IEnumerable<int> labels)
        {

            T[] datas = data.ToArray();
            int[] marks = labels.ToArray();

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
    }
}
