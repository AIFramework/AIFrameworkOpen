using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.DataStructs;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI.ML.NeuralNetwork.CoreNNW.DataSets
{
    /// <summary>
    /// Датасет для нерекуррентной сети-автокодировщика
    /// </summary>
    [Serializable]
    public class DataSetSingle : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        #region Конструкторы
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="loss"></param>
        public DataSetSingle(Shape inputShape, ILoss loss = null) : base(inputShape, null, loss) { }
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="data"></param>
        /// <param name="loss"></param>
        /// <param name="valSplit"></param>
        public DataSetSingle(NNValue[] data, ILoss loss, double valSplit = 0.0) : base(data[0].Shape, null, loss)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }
            }

            Init(data, valSplit);
        }
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="data"></param>
        /// <param name="loss"></param>
        /// <param name="valSplit"></param>
        public DataSetSingle(IAlgebraicStructure<double>[] data, ILoss loss, double valSplit = 0.0) : base(data[0].Shape, null, loss)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            NNValue[] dataNNVal = new NNValue[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }

                dataNNVal[i] = new NNValue(data[i]);
            }

            Init(dataNNVal, valSplit);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingSample(NNValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            TrainingInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingSample(IAlgebraicStructure<double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            TrainingInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddValidationSample(NNValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            ValidationInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddValidationSample(IAlgebraicStructure<double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            ValidationInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingSample(NNValue data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            TestingInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingSample(IAlgebraicStructure<double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(data));
            }

            TestingInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingRange(NNValue[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }
            }

            TrainingInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingRange(IAlgebraicStructure<double>[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            NNValue[] nnvals = new NNValue[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            TrainingInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddvalidationRange(NNValue[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }
            }

            ValidationInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddValidationRange(IAlgebraicStructure<double>[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            NNValue[] nnvals = new NNValue[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            ValidationInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingRange(NNValue[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }
            }

            TestingInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingRange(IAlgebraicStructure<double>[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            NNValue[] nnvals = new NNValue[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == null)
                {
                    throw new ArgumentNullException(nameof(data), $"Данные  ({i}) — null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            TestingInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Добавить все образцы из датасета "anotherSet" в текущий
        /// </summary>
        /// <param name="anotherSet"></param>
        public void Merge(DataSetSingle anotherSet)
        {
            if (anotherSet == null)
            {
                throw new ArgumentNullException(nameof(anotherSet));
            }

            if (!anotherSet.InputShape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Входная форма другого набора данных не соответствует текущей входной форме", nameof(anotherSet));
            }

            if (anotherSet.OutputShape != OutputShape)
            {
                throw new ArgumentException("Выходная форма другого набора данных не соответствует текущей выходной форме", nameof(anotherSet));
            }

            TrainingInternal.AddRange(anotherSet.Training);
            ValidationInternal.AddRange(anotherSet.Validation);
            TestingInternal.AddRange(anotherSet.Testing);
        }
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Сохранить датасет в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранить датасет в поток
        /// </summary>
        /// <param name="stream">Поток</param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузить датасет из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns></returns>
        public static DataSetSingle Load(string path)
        {
            return BinarySerializer.Load<DataSetSingle>(path);
        }
        /// <summary>
        /// Загрузить датасет из потока
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static DataSetSingle Load(Stream stream)
        {
            return BinarySerializer.Load<DataSetSingle>(stream);
        }
        #endregion

        #endregion

        #region Приватные методы
        private void Init(NNValue[] data, double valSplit)
        {
            List<DataSequence> Data = GetSequences(data);

            // Валидация
            for (int i = 0; i < Data.Count; i++)
            {
                if (_rnd.NextDouble() > valSplit)
                {
                    TrainingInternal.Add(Data[i]);
                }
                else
                {
                    ValidationInternal.Add(Data[i]);
                }
            }
        }

        // Получение последовательности
        private List<DataSequence> GetSequences(NNValue[] data)
        {
            List<DataSequence> dataSequences = new List<DataSequence>(data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                DataStep step = new DataStep(data[i]);
                DataSequence dataSequence = new DataSequence(step);
                dataSequences.Add(dataSequence);
            }

            return dataSequences;
        }

        private DataSequence GetSequence(NNValue data)
        {
            return new DataSequence(new DataStep(data));
        }
        #endregion
    }
}
