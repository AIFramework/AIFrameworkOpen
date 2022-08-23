using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataStructs.Algebraic;
using AI.ML.AlgorithmAnalysis;
using AI.ML.Regression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Pipelines
{
    /// <summary>
    /// Конвейер обработки данных, регрессия на базе объекта
    /// </summary>
    [Serializable]
    public abstract class ObjectRegressionPipeline<T>
    {
        /// <summary>
        /// Извлечение признаков из данных
        /// </summary>
        public IFeaturesExtractor<T> Extractor { get; set; }

        /// <summary>
        /// Нормализация входных данных
        /// </summary>
        public Normalizer NormalizerX { get; set; }

        /// <summary>
        /// Среднее выхода
        /// </summary>
        public double MeanY { get; set; } = 0;

        /// <summary>
        /// Среднеквадратичное отклонение выхода
        /// </summary>
        public double StdY { get; set; } = 1;

        /// <summary>
        /// Классификатор
        /// </summary>
        public IRegression Regression { get; set; }

        /// <summary>
        /// Конвейер обработки данных, регрессия на базе объекта
        /// </summary>
        public ObjectRegressionPipeline()
        {

        }

        /// <summary>
        /// Запуск регрессии
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual double Predict(T input)
        {
            Vector features = (Vector)NormalizerX.Transform(
                    Extractor.GetFeatures(input));

            double normalPredict = Regression.Predict(features);

            return (normalPredict * StdY) + MeanY;
        }


        /// <summary>
        /// Запуск регрессии
        /// </summary>
        /// <param name="input">Вход</param>
        public virtual Vector Predict(IEnumerable<T> input)
        {
            Vector outp = new Vector(input.Count());

            int i = 0;
            foreach (T inputItem in input)
                outp[i++] = Predict(inputItem);

            return outp;
        }

       
        /// <summary>
        /// Обучение ковейера
        /// </summary>
        /// <param name="data"></param>
        /// <param name="labels"></param>
        public virtual void Train(IEnumerable<T> data, IEnumerable<double> labels, bool yNormal = true)
        {

            T[] datas = data.ToArray();
            Vector marks = labels.ToArray();

            Vector[] features = new Vector[datas.Length];

            // Извлечение признаков
            for (int i = 0; i < datas.Length; i++)
                features[i] = Extractor.GetFeatures(datas[i]);

            // Обучение нормализатора
            NormalizerX.Train(features);
            features = (Vector[])NormalizerX.Transform(features); // Нормализация

            // Если нормализуется выход
            if (yNormal)
            {
                MeanY = marks.Mean();
                StdY = marks.Std();

                StdY = StdY == 0 ? double.Epsilon : StdY;

                marks -= MeanY;
                marks /= StdY;
            }

            // Обучение классификатора
            Regression.Train(features, marks);
        }


        /// <summary>
        /// Обучение и тестирование
        /// </summary>
        /// <param name="data"></param>
        /// <param name="marks"></param>
        /// <returns></returns>
        public virtual double TrainTest(T[] data, double[] marks, double trainPart = 0.9, int seed = 0, bool yNormal = true)
        {
            List<T> trainX = new List<T>((int)(trainPart * data.Length));
            List<double> trainY = new List<double>((int)(trainPart * data.Length));


            List<T> testX = new List<T>((int)((1 - trainPart) * data.Length));
            List<double> testY = new List<double>((int)((1 - trainPart) * data.Length));

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

            Train(trainX, trainY, yNormal);

            return MetricsForRegression.R2(Predict(testX), (Vector)testY.ToArray());
        }
    }
}
