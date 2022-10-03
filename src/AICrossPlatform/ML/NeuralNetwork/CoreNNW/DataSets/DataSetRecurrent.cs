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
    /// Набор данных для рекуррентных нейронных сетей
    /// </summary>
    [Serializable]
    public class DataSetRecurrent : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        #region Конструкторы
        /// <summary>
        /// Набор данных для рекуррентных нейронных сетей
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="outputShape"></param>
        /// <param name="loss"></param>
        public DataSetRecurrent(Shape inputShape, Shape outputShape, ILoss loss = null) : base(inputShape, outputShape, loss) { }
        /// <summary>
        /// Набор данных для рекуррентных нейронных сетей
        /// </summary>
        /// <param name="inputs">Входы</param>
        /// <param name="outputs">Выходы (целевые значения)</param>
        /// <param name="loss">Функция ошибки</param>
        /// <param name="valSplit">Часть выборки для валидации</param>
        public DataSetRecurrent(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs, ILoss loss, double valSplit = 0.0) : base(inputs[0][0].Shape, outputs[0][0].Shape, loss)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }
                }
            }

            Init(inputs, outputs, valSplit);
        }
        /// <summary>
        /// Набор данных для рекуррентных нейронных сетей
        /// </summary>
        /// <param name="inputs">Входы</param>
        /// <param name="outputs">Выходы (целевые значения)</param>
        /// <param name="loss">Функция ошибки</param>
        /// <param name="valSplit">Часть выборки для валидации</param>
        public DataSetRecurrent(IReadOnlyList<IAlgebraicStructure<double>>[] inputs, IReadOnlyList<IAlgebraicStructure<double>>[] outputs, ILoss loss, double valSplit = 0.0) : base(inputs[0][0].Shape, outputs[0][0].Shape, loss)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                valueInp[i] = new List<NNValue>(inputs[i].Count);
                valueOutp[i] = new List<NNValue>(outputs[i].Count);

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }

                    valueInp[i].Add(new NNValue(inputs[i][j]));
                    valueOutp[i].Add(new NNValue(outputs[i][j]));
                }
            }

            Init(valueInp, valueOutp, valSplit);
        }
        #endregion

        #region Методы
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(IReadOnlyList<NNValue> input, IReadOnlyList<NNValue> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }
            }

            TrainingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(IReadOnlyList<IAlgebraicStructure<double>> input, IReadOnlyList<IAlgebraicStructure<double>> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue> inputList = new List<NNValue>(input.Count);
            List<NNValue> outputList = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }

                inputList[i] = new NNValue(input[i]);
                outputList[i] = new NNValue(output[i]);
            }

            TrainingInternal.Add(GetSequence(inputList, outputList));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(IReadOnlyList<NNValue> input, IReadOnlyList<NNValue> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }
            }

            ValidationInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(IReadOnlyList<IAlgebraicStructure<double>> input, IReadOnlyList<IAlgebraicStructure<double>> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue> inputList = new List<NNValue>(input.Count);
            List<NNValue> outputList = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }

                inputList[i] = new NNValue(input[i]);
                outputList[i] = new NNValue(output[i]);
            }

            ValidationInternal.Add(GetSequence(inputList, outputList));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(IReadOnlyList<NNValue> input, IReadOnlyList<NNValue> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }
            }

            TestingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(IReadOnlyList<IAlgebraicStructure<double>> input, IReadOnlyList<IAlgebraicStructure<double>> output)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue> inputList = new List<NNValue>(input.Count);
            List<NNValue> outputList = new List<NNValue>(output.Count);

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] == null)
                {
                    throw new ArgumentNullException(nameof(input), $"Входные данные  ({i}) — null");
                }

                if (output[i] == null)
                {
                    throw new ArgumentNullException(nameof(output), $"Выходные данные  ({i}) — null");
                }

                if (!input[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(input));
                }

                if (!output[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(output));
                }

                inputList[i] = new NNValue(input[i]);
                outputList[i] = new NNValue(output[i]);
            }

            TestingInternal.Add(GetSequence(inputList, outputList));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new ArgumentException("Количество входов не совпадает с числом выходов");
                }

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }
                }
            }

            TrainingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(IReadOnlyList<IAlgebraicStructure<double>>[] inputs, IReadOnlyList<IAlgebraicStructure<double>>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                valueInp[i] = new List<NNValue>(inputs[i].Count);
                valueOutp[i] = new List<NNValue>(outputs[i].Count);

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }

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
        public void AddValidationRange(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }
                }
            }

            ValidationInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddValidationRange(IReadOnlyList<IAlgebraicStructure<double>>[] inputs, IReadOnlyList<IAlgebraicStructure<double>>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                valueInp[i] = new List<NNValue>(inputs[i].Count);
                valueOutp[i] = new List<NNValue>(outputs[i].Count);

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }

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
        public void AddTestingRange(IReadOnlyList<NNValue>[] inputs, IReadOnlyList<NNValue>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }
                }
            }

            TestingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTestingRange(IReadOnlyList<IAlgebraicStructure<double>>[] inputs, IReadOnlyList<IAlgebraicStructure<double>>[] outputs)
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
                throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
            }

            List<NNValue>[] valueInp = new List<NNValue>[inputs.Length];
            List<NNValue>[] valueOutp = new List<NNValue>[outputs.Length];

            for (int i = 0; i < valueInp.Length; i++)
            {
                if (inputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}) — null");
                }

                if (outputs[i] == null)
                {
                    throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}) — null");
                }

                if (inputs[i].Count != outputs[i].Count)
                {
                    throw new InvalidOperationException("Количество входов не совпадает с числом выходов");
                }

                valueInp[i] = new List<NNValue>(inputs[i].Count);
                valueOutp[i] = new List<NNValue>(outputs[i].Count);

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    if (inputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(inputs), $"Входные данные  ({i}, {j})) — null");
                    }

                    if (outputs[i][j] == null)
                    {
                        throw new ArgumentNullException(nameof(outputs), $"Выходные данные  ({i}, {j})) — null");
                    }

                    if (!inputs[i][j].Shape.FuzzyEquals(InputShape))
                    {
                        throw new ArgumentException($"Входные данные  ({i}, {j}) не соотвествуют форме входа", nameof(inputs));
                    }

                    if (!outputs[i][j].Shape.FuzzyEquals(OutputShape))
                    {
                        throw new ArgumentException($"Выходные данные  ({i}, {j}) не соотвествуют форме выхода", nameof(outputs));
                    }

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
        public void Merge(DataSetRecurrent anotherSet)
        {
            if (anotherSet == null)
            {
                throw new ArgumentNullException(nameof(anotherSet));
            }

            if (anotherSet.InputShape != InputShape)
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
        public static DataSetRecurrent Load(string path)
        {
            return BinarySerializer.Load<DataSetRecurrent>(path);
        }
        /// <summary>
        /// Загрузить датасет из потока
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DataSetRecurrent Load(Stream stream)
        {
            return BinarySerializer.Load<DataSetRecurrent>(stream);
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
            List<DataSequence> dataSequences = new List<DataSequence>(inputs.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                DataStep[] steps = new DataStep[inputs[i].Count];

                for (int j = 0; j < inputs[i].Count; j++)
                {
                    steps[j] = new DataStep(inputs[i][j], outputs[i][j]);
                }

                dataSequences.Add(new DataSequence(steps));
            }

            return dataSequences;
        }

        private DataSequence GetSequence(IReadOnlyList<NNValue> input, IReadOnlyList<NNValue> output)
        {
            DataStep[] steps = new DataStep[input.Count];

            for (int i = 0; i < input.Count; i++)
            {
                steps[i] = new DataStep(input[i], output[i]);
            }

            return new DataSequence(steps);
        }
        #endregion
    }
}
