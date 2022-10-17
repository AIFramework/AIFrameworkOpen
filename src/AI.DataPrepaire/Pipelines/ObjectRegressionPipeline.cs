using AI.DataPrepaire.DataNormalizers;
using AI.DataPrepaire.FeatureExtractors;
using AI.DataPrepaire.Pipelines.Utils;
using AI.DataStructs.Algebraic;
using AI.Extensions;
using AI.ML.AlgorithmAnalysis;
using AI.ML.Regression;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public FeaturesExtractor<T> Extractor { get; set; }

        /// <summary>
        /// Детектор аномальных и/или неподходящих данных
        /// </summary>
        public IDetector<T> Detector { get; set; }

        /// <summary>
        /// Метод аугметации данных
        /// </summary>
        public DataAugmetation<Vector> DataAugmetation { get; set; }

        /// <summary>
        /// Метод реставрации данных
        /// </summary>
        public Func<T, T> DataRestavration { get; set; } = x => x;

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
            Vector features = GetFeatures(input);
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
        /// Обучение конвейера
        /// </summary>
        public virtual void Train(IEnumerable<T> data, IEnumerable<double> labels, bool yNormal = true)
        {

            //Очистка данных
            DatasetForRegression dataSamples = ClearData(
                DataArrayResvration(data), labels.ToArray());
            dataSamples.ShuffleData(); // Премешивание

            T[] datas = dataSamples.ReturnData();
            Vector targets = dataSamples.ReturnClasses();

            Vector[] features = new Vector[datas.Length];
            // Извлечение признаков
            for (int i = 0; i < datas.Length; i++)
                features[i] = Extractor.GetFeatures(datas[i]);


            // Обучение нормализатора
            NormalizerX.Train(features);
            features = (Vector[])NormalizerX.Transform(features); // Нормализация

            //Аугментация данных
            var dataNew = DataAugmetation.Augmetation(features, targets);
            dataNew.Shuffle();


            features = new Vector[dataNew.Length];
            targets = new Vector(dataNew.Length);

            for (int i = 0; i < features.Length; i++)
            {
                features[i] = dataNew[i].Item1;
                targets[i] = dataNew[i].Item2;
            }



            // Если нормализуется выход
            if (yNormal)
            {
                MeanY = targets.Mean();
                StdY = targets.Std();

                StdY = StdY == 0 ? double.Epsilon : StdY;

                targets -= MeanY;
                targets /= StdY;
            }

            // Обучение классификатора
            Regression.Train(features, targets);
        }


        /// <summary>
        /// Обучение и тестирование
        /// </summary>
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

        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Vector GetFeatures(T data)
        {
            var input = DataRestavration(data);
            var features = Extractor.GetFeatures(input);
            return (Vector)NormalizerX.Transform(features);
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
        /// Очистка данных
        /// </summary>
        public DatasetForRegression ClearData(T[] dataIn, double[] targets)
        {
            DatasetForRegression dataSamples = new DatasetForRegression(targets.Length / 2);

            for (int i = 0; i < targets.Length; i++)
            {
                // Добавляем валидные элементы
                bool validTarget = !(double.IsNaN(targets[i]) || double.IsInfinity(targets[i]));
                if (validTarget && !Detector.IsDetected(dataIn[i])) dataSamples.Add(dataIn[i], targets[i]);
            }

            return dataSamples;
        }

        /// <summary>
        /// Датасет для конвейера регрессии
        /// </summary>
        [Serializable]
        public class DatasetForRegression : List<DataSampleR>
        {

            /// <summary>
            /// Датасет для конвейера регрессии
            /// </summary>
            public DatasetForRegression() : base() { }

            /// <summary>
            /// Датасет для конвейера регрессии
            /// </summary>
            /// <param name="cap">Емкость коллекции</param>
            public DatasetForRegression(int cap) : base(cap) { }

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
            /// Вернуть целевые значения
            /// </summary>
            /// <returns></returns>
            public double[] ReturnClasses()
            {
                var ret = new double[Count];

                for (int i = 0; i < Count; i++)
                    ret[i] = this[i].Target;

                return ret;
            }

            /// <summary>
            /// Добавление объекта в выборку
            /// </summary>
            /// <param name="obj">Классифицируемый объект</param>
            /// <param name="target">Целевое значение</param>
            public void Add(T obj, double target)
            {
                Add(new DataSampleR(obj, target));
            }
        }

        /// <summary>
        /// Элемент датасета регрессии
        /// </summary>
        [Serializable]
        public class DataSampleR
        {
            /// <summary>
            /// Целевое значение
            /// </summary>
            public double Target { get; set; }

            /// <summary>
            /// Классифицируемый объект
            /// </summary>
            public T Obj { get; set; }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            public DataSampleR() { }

            /// <summary>
            /// Элемент датасета
            /// </summary>
            /// <param name="target">Целевое значение</param>
            /// <param name="obj">Классифицируемый объект</param>
            public DataSampleR(T obj, double target)
            {
                Target = target;
                Obj = obj;
            }
        }
    }
}
