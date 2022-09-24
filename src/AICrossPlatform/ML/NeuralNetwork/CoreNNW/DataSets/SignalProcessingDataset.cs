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
    /// Данныеset for DSP tasks
    /// </summary>
    [Serializable]
    public class SignalProcessingDataSet : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        /// <summary>
        /// Данныеset for DSP tasks
        /// </summary>
        /// <param name="loss"></param>
        public SignalProcessingDataSet(ILoss loss = null) : base(new Shape(1, 1, 1), new Shape(1, 1, 1), loss) { }
        /// <summary>
        /// Данныеset for DSP tasks
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        /// <param name="loss">Функция ошибки</param>
        /// <param name="valSplit">Fraction of the sample to be used for validation</param>
        public SignalProcessingDataSet(Vector[] inputs, Vector[] outputs, ILoss loss, double valSplit = 0.0) : base(new Shape(1, 1, 1), new Shape(1, 1, 1), loss)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            if (outputs == null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }

            if (inputs.Length != outputs.Length)
            {
                throw new InvalidOperationException("Count of inputs and outputs mismatches");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Count of inputs and outputs mismatches");
                }

                valueInp[i] = new List<NNValue>();
                valueOutp[i] = new List<NNValue>();

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    valueInp[i].Add(new NNValue(inputs[i][j]));
                    valueOutp[i].Add(new NNValue(outputs[i][j]));
                }
            }

            Init(valueInp, valueOutp, valSplit);
        }

        #region Методы
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(Vector input, Vector output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (input.Count != output.Count)
            {
                throw new InvalidOperationException("Count of inputs and outputs mismatches");
            }

            List<NNValue> valueInp = new List<NNValue>(input.Count);
            List<NNValue> valueOutp = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                valueInp[i] = new NNValue(input[i]);
                valueOutp[i] = new NNValue(output[i]);
            }

            TrainingInternal.Add(GetSequence(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(Vector input, Vector output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (input.Count != output.Count)
            {
                throw new InvalidOperationException("Count of inputs and outputs mismatches");
            }

            List<NNValue> valueInp = new List<NNValue>(input.Count);
            List<NNValue> valueOutp = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                valueInp[i] = new NNValue(input[i]);
                valueOutp[i] = new NNValue(output[i]);
            }

            ValidationInternal.Add(GetSequence(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(Vector input, Vector output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (input.Count != output.Count)
            {
                throw new InvalidOperationException("Count of inputs and outputs mismatches");
            }

            List<NNValue> valueInp = new List<NNValue>(input.Count);
            List<NNValue> valueOutp = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                valueInp[i] = new NNValue(input[i]);
                valueOutp[i] = new NNValue(output[i]);
            }

            TestingInternal.Add(GetSequence(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(Vector[] inputs, Vector[] outputs)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            if (outputs == null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }

            if (inputs.Length != outputs.Length)
            {
                throw new InvalidOperationException("Count of inputs and outputs mismatches");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Count of inputs and outputs mismatches");
                }

                valueInp[i] = new List<NNValue>();
                valueOutp[i] = new List<NNValue>();

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    valueInp[i].Add(new NNValue(inputs[i][j]));
                    valueOutp[i].Add(new NNValue(outputs[i][j]));
                }
            }

            TrainingInternal.AddRange(GetSequences(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddValidationRange(Vector[] inputs, Vector[] outputs)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            if (outputs == null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }

            if (inputs.Length != outputs.Length)
            {
                throw new ArgumentException("Count of inputs and outputs mismatches");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Count of inputs and outputs mismatches");
                }

                valueInp[i] = new List<NNValue>();
                valueOutp[i] = new List<NNValue>();

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    valueInp[i].Add(new NNValue(inputs[i][j]));
                    valueOutp[i].Add(new NNValue(outputs[i][j]));
                }
            }

            ValidationInternal.AddRange(GetSequences(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTestingRange(Vector[] inputs, Vector[] outputs)
        {
            if (inputs == null)
            {
                throw new ArgumentNullException(nameof(inputs));
            }

            if (outputs == null)
            {
                throw new ArgumentNullException(nameof(outputs));
            }

            if (inputs.Length != outputs.Length)
            {
                throw new ArgumentException("Count of inputs and outputs mismatches");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Count of inputs and outputs mismatches");
                }

                valueInp[i] = new List<NNValue>();
                valueOutp[i] = new List<NNValue>();

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    valueInp[i].Add(new NNValue(inputs[i][j]));
                    valueOutp[i].Add(new NNValue(outputs[i][j]));
                }
            }

            TestingInternal.AddRange(GetSequences(valueInp, valueOutp));
        }
        /// <summary>
        /// Добавить все образцы из датасета "anotherSet" в текущий
        /// </summary>
        /// <param name="anotherSet"></param>
        public void Merge(SignalProcessingDataSet anotherSet)
        {
            if (anotherSet == null)
            {
                throw new ArgumentNullException(nameof(anotherSet));
            }

            if (anotherSet.InputShape != InputShape)
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
        /// Сохранить датасет в файл
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранить датасет в поток
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузить датасет из файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SignalProcessingDataSet Load(string path)
        {
            return BinarySerializer.Load<SignalProcessingDataSet>(path);
        }
        /// <summary>
        /// Загрузить датасет из потока
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SignalProcessingDataSet Load(Stream stream)
        {
            return BinarySerializer.Load<SignalProcessingDataSet>(stream);
        }
        #endregion

        #endregion

        #region Приватные методы
        private void Init(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs, double valSplit)
        {
            List<DataSequence> data = GetSequences(inputs, outputs);

            // Валидация
            for (int i = 0; i < data.Count; i++)
            {
                if (_rnd.NextDouble() > valSplit)
                {
                    TrainingInternal.Add(data[i]);
                }
                else
                {
                    ValidationInternal.Add(data[i]);
                }
            }
        }

        // Получение последовательности
        private List<DataSequence> GetSequences(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs)
        {
            List<DataSequence> dataSequences = new List<DataSequence>();

            for (int i = 0; i < inputs.Length; i++)
            {
                DataStep[] steps = new DataStep[inputs[i].Count];

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    steps[i] = new DataStep(inputs[i][j], outputs[i][j]);
                }

                dataSequences.Add(new DataSequence(steps));
            }

            return dataSequences;
        }

        private DataSequence GetSequence(IReadOnlyList<NNValue> inputs, IReadOnlyList<NNValue> outputs)
        {
            DataStep[] steps = new DataStep[inputs.Count];

            for (int i = 0; i < inputs.Count; i++)
            {
                steps[i] = new DataStep(inputs[i], outputs[i]);
            }

            return new DataSequence(steps);
        }
        #endregion
    }
}
