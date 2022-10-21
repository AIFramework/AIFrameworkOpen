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
    /// Набор данных для нерекуррентной нейронной сети
    /// </summary>
    [Serializable]
    public class DataSetNoRecurrent : DataSet, ISavable
    {
        [NonSerialized]
        private readonly Random _rnd = new Random(5);

        #region Конструкторы
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="outputShape"></param>
        /// <param name="loss"></param>
        public DataSetNoRecurrent(Shape inputShape, Shape outputShape, ILoss loss = null) : base(inputShape, outputShape, loss) { }
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="inputs">Входы</param>
        /// <param name="outputs">Выходы (целевые значения)</param>
        /// <param name="loss">Функция ошибки</param>
        /// <param name="valSplit">Часть выборки для валидации</param>
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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }
            }

            Init(inputs, outputs, valSplit);
        }
        /// <summary>
        /// Набор данных для нерекуррентной нейронной сети
        /// </summary>
        /// <param name="inputs">Входы</param>
        /// <param name="outputs">Выходы (целевые значения)</param>
        /// <param name="loss">Функция ошибки</param>
        /// <param name="valSplit">Часть выборки для валидации</param>
        public DataSetNoRecurrent(IAlgebraicStructure<double>[] inputs, IAlgebraicStructure<double>[] outputs, ILoss loss, double valSplit = 0.0) : base(inputs[0].Shape, outputs[0].Shape, loss)
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

            NNValue[] valueInp = new NNValue[inputs.Length];
            NNValue[] valueOutp = new NNValue[outputs.Length];

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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }

                valueInp[i] = new NNValue(inputs[i]);
                valueOutp[i] = new NNValue(outputs[i]);
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            TrainingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в обучающую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingSample(IAlgebraicStructure<double> input, IAlgebraicStructure<double> output)
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            TrainingInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            ValidationInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в валидационную выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddValidationSample(IAlgebraicStructure<double> input, IAlgebraicStructure<double> output)
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            ValidationInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            TestingInternal.Add(GetSequence(input, output));
        }
        /// <summary>
        /// Добавить один пример в тестовую выборку
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTestingSample(IAlgebraicStructure<double> input, IAlgebraicStructure<double> output)
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
                throw new ArgumentException("Входные данные не соотвествуют форме входа", nameof(input));
            }

            if (!output.Shape.FuzzyEquals(OutputShape))
            {
                throw new ArgumentException("Выходные данные не соотвествуют форме выхода", nameof(output));
            }

            TestingInternal.Add(GetSequence(new NNValue(input), new NNValue(output)));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }
            }

            TrainingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в обучающую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTrainingRange(IAlgebraicStructure<double>[] inputs, IAlgebraicStructure<double>[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            TrainingInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }
            }

            ValidationInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в валидационную выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddValidationRange(IAlgebraicStructure<double>[] inputs, IAlgebraicStructure<double>[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            ValidationInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }
            }

            TestingInternal.AddRange(GetSequences(inputs, outputs));
        }
        /// <summary>
        /// Добавить массив примеров в тестовую выборку
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        public void AddTestingRange(IAlgebraicStructure<double>[] inputs, IAlgebraicStructure<double>[] outputs)
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

            NNValue[] inputNNVal = new NNValue[inputs.Length];
            NNValue[] outputNNVal = new NNValue[outputs.Length];

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

                if (!inputs[i].Shape.FuzzyEquals(InputShape))
                {
                    throw new ArgumentException($"Входные данные  ({i}) не соотвествуют форме входа", nameof(inputs));
                }

                if (!outputs[i].Shape.FuzzyEquals(OutputShape))
                {
                    throw new ArgumentException($"Выходные данные  ({i}) не соотвествуют форме выхода", nameof(outputs));
                }

                inputNNVal[i] = new NNValue(inputs[i]);
                outputNNVal[i] = new NNValue(outputs[i]);
            }

            TestingInternal.AddRange(GetSequences(inputNNVal, outputNNVal));
        }
        /// <summary>
        /// Добавить все образцы из датасета "anotherSet" в текущий
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
        public static DataSetNoRecurrent Load(string path)
        {
            return BinarySerializer.Load<DataSetNoRecurrent>(path);
        }
        /// <summary>
        /// Загрузить датасет из потока
        /// </summary>
        /// <param name="stream">Поток</param>
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
