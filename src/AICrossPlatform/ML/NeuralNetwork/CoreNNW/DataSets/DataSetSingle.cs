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
    [Serializable]
    public class DataSetSingle : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        #region Конструкторы
        /// <summary>
        /// Dataset for non-recurrent neural networks
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="loss"></param>
        public DataSetSingle(Shape inputShape, ILoss loss = null) : base(inputShape, null, loss) { }
        /// <summary>
        /// Dataset for non-recurrent neural networks
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }
            }

            Init(data, valSplit);
        }
        /// <summary>
        /// Dataset for non-recurrent neural networks
        /// </summary>
        /// <param name="data"></param>
        /// <param name="loss"></param>
        /// <param name="valSplit"></param>
        public DataSetSingle(IAlgebraicStructure[] data, ILoss loss, double valSplit = 0.0) : base(data[0].Shape, null, loss)
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }

                dataNNVal[i] = new NNValue(data[i]);
            }

            Init(dataNNVal, valSplit);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Adds one sample to the training subset
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
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            TrainingInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Adds one sample to the training subset
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingSample(IAlgebraicStructure data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            TrainingInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Adds one sample to the validation subset
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
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            ValidationInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Adds one sample to the validation subset
        /// </summary>
        /// <param name="data"></param>
        public void AddValidationSample(IAlgebraicStructure data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            ValidationInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Adds one sample to the testing subset
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
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            TestingInternal.Add(GetSequence(data));
        }
        /// <summary>
        /// Adds one sample to the testing subset
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingSample(IAlgebraicStructure data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided data doesn't match input shape", nameof(data));
            }

            TestingInternal.Add(GetSequence(new NNValue(data)));
        }
        /// <summary>
        /// Adds sample range to the training subset
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }
            }

            TrainingInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Adds sample range to the training subset
        /// </summary>
        /// <param name="data"></param>
        public void AddTrainingRange(IAlgebraicStructure[] data)
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            TrainingInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Adds sample range to the validation subset
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }
            }

            ValidationInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Adds sample range to the validation subset
        /// </summary>
        /// <param name="data"></param>
        public void AddValidationRange(IAlgebraicStructure[] data)
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            ValidationInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Adds sample range to the testing subset
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }
            }

            TestingInternal.AddRange(GetSequences(data));
        }
        /// <summary>
        /// Adds sample range to the testing subset
        /// </summary>
        /// <param name="data"></param>
        public void AddTestingRange(IAlgebraicStructure[] data)
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
                    throw new ArgumentNullException(nameof(data), $"Provided data peace({i}) is null");
                }

                if (!data[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided data peace({i}) doesn't match input shape", nameof(data));
                }

                nnvals[i] = new NNValue(data[i]);
            }

            TestingInternal.AddRange(GetSequences(nnvals));
        }
        /// <summary>
        /// Add all samples from the "anotherSet" to current
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
                throw new ArgumentException("Another dataset's input shape doesn't match current input shape", nameof(anotherSet));
            }

            if (anotherSet.OutputShape != OutputShape)
            {
                throw new ArgumentException("Another dataset's output shape doesn't match current output shape", nameof(anotherSet));
            }

            TrainingInternal.AddRange(anotherSet.Training);
            ValidationInternal.AddRange(anotherSet.Validation);
            TestingInternal.AddRange(anotherSet.Testing);
        }
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Save dataset to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save dataset to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Load dataset from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DataSetSingle Load(string path)
        {
            return BinarySerializer.Load<DataSetSingle>(path);
        }
        /// <summary>
        /// Load dataset from stream
        /// </summary>
        /// <param name="stream"></param>
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
