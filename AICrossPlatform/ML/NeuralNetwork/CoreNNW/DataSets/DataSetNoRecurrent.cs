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
    /// Dataset for non-recurrent neural networks
    /// </summary>
    [Serializable]
    public class DataSetNoRecurrent : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        #region Конструкторы
        /// <summary>
        /// Dataset for non-recurrent neural networks
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="outputShape"></param>
        /// <param name="loss"></param>
        public DataSetNoRecurrent(Shape inputShape, Shape outputShape, ILoss loss = null) : base(inputShape, outputShape, loss) { }
        /// <summary>
        /// Dataset for non-recurrent neural networks
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        /// <param name="loss">Loss function</param>
        /// <param name="valSplit">Fraction of the sample to be used for validation</param>
        public DataSetNoRecurrent(NNValue[] inputs, NNValue[] outputs, ILoss loss, double valSplit = 0.0) : base(inputs[0].Shape, outputs[0].Shape, loss)
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

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }
            }

            Init(inputs, outputs, valSplit);
        }
        /// <summary>
        /// Dataset for non-recurrent neural networks
        /// </summary>
        /// <param name="inputs">Inputs</param>
        /// <param name="outputs">Outputs(target values)</param>
        /// <param name="loss">Loss function</param>
        /// <param name="valSplit">Fraction of the sample to be used for validation</param>
        public DataSetNoRecurrent(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs, ILoss loss, double valSplit = 0.0) : base(inputs[0].Shape, outputs[0].Shape, loss)
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

            NNValue[] valueInp = new NNValue[inputs.Length];
            NNValue[] valueOutp = new NNValue[outputs.Length];

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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }

                valueInp[i] = new NNValue(inputs[i]);
                valueOutp[i] = new NNValue(outputs[i]);
            }

            Init(valueInp, valueOutp, valSplit);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Adds one sample to the training subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(NNValue input, NNValue output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            TrainingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Adds one sample to the training subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(IAlgebraicStructure input, IAlgebraicStructure output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            TrainingInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Adds one sample to the validation subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(NNValue input, NNValue output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            ValidationInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Adds one sample to the validation subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(IAlgebraicStructure input, IAlgebraicStructure output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            ValidationInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Adds one sample to the testing subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(NNValue input, NNValue output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            TestingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Adds one sample to the testing subset
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(IAlgebraicStructure input, IAlgebraicStructure output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!input.Shape.FuzzyEquals(InputShape))
            {
                throw new ArgumentException("Provided input data doesn't match input shape", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Provided output data doesn't match output shape", nameof(output));
            }

            TestingInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Adds sample range to the training subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(NNValue[] inputs, NNValue[] outputs)
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

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }
            }

            TrainingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Adds sample range to the training subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            TrainingInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Adds sample range to the validation subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddValidationRange(NNValue[] inputs, NNValue[] outputs)
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

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }
            }

            ValidationInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Adds sample range to the validation subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddValidationRange(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            ValidationInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Adds sample range to the testing subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTestingRange(NNValue[] inputs, NNValue[] outputs)
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

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }
            }

            TestingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Adds sample range to the testing subset
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTestingRange(IAlgebraicStructure[] inputs, IAlgebraicStructure[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Provided input data peace({i}) is null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Provided output data peace({i}) is null");
                }

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Provided input data peace({i}) doesn't match input shape", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Provided output data peace({i}) doesn't match output shape", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            TestingInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Add all samples from the "anotherSet" to current
        /// </summary>
        /// <param name="anotherSet"></param>
        public void Merge(DataSetNoRecurrent anotherSet)
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
        public static DataSetNoRecurrent Load(string path)
        {
            return BinarySerializer.Load<DataSetNoRecurrent>(path);
        }
        /// <summary>
        /// Load dataset from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataSetNoRecurrent Load(Stream stream)
        {
            return BinarySerializer.Load<DataSetNoRecurrent>(stream);
        }
        #endregion

        #endregion

        #region Приватные методы
        private void Init(NNValue[] inputs, NNValue[] outputs, double valSplit)
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
        private List<DataSequence> GetSequences(NNValue[] inputs, NNValue[] outputs)
        {
            List<DataSequence> dataSequences = new List<DataSequence>(inputs.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                dataSequences.Add(new DataSequence(new DataStep(inputs[i], outputs[i])));
            }

            return dataSequences;
        }

        private DataSequence GetSequence(NNValue input, NNValue output)
        {
            return new DataSequence(new DataStep(input, output));
        }
        #endregion
    }
}
