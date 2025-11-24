using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Набор данных для обучения нейронки
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Tr. count = {TrainingInternal.Count}, Val. count = {ValidationInternal.Count}, Test. count = {TestingInternal.Count}")]
    public class DataSet : IDataSet
    {
        /// <summary>
        /// Размерность входных данных
        /// </summary>
        public Shape3D InputShape { get; }
        /// <summary>
        /// Размерность выходных данных
        /// </summary>
        public Shape3D OutputShape { get; }
        /// <summary>
        /// Функция ошибки
        /// </summary>
        public ILoss LossFunction { get; set; }
        /// <summary>
        /// Обучающая выборка
        /// </summary>
        protected List<DataSequence> TrainingInternal { get; }
        /// <summary>
        /// Обучающая выборка
        /// </summary>
        public IReadOnlyList<DataSequence> Training => TrainingInternal.AsReadOnly();
        /// <summary>
        /// Проверка внутренней памяти подвыборки
        /// </summary>
        protected List<DataSequence> ValidationInternal { get; }
        /// <summary>
        /// Валидационная подвыборка
        /// </summary>
        public IReadOnlyList<DataSequence> Validation => ValidationInternal.AsReadOnly();
        /// <summary>
        /// Есть ли валидационная подвыборка
        /// </summary>
        public bool HasValidationData => ValidationInternal != null && ValidationInternal.Count > 0;
        /// <summary>
        /// Тестовая подвыборка internal storage
        /// </summary>
        protected List<DataSequence> TestingInternal { get; }
        /// <summary>
        /// Тестовая подвыборка
        /// </summary>
        public IReadOnlyList<DataSequence> Testing => TestingInternal.AsReadOnly();
        /// <summary>
        /// Есть ли тестовая подвыборка
        /// </summary>
        public bool HasTestingData => TestingInternal != null && TestingInternal.Count > 0;

        /// <summary>
        /// Инициализировать набор данных с заданной входной и выходной формой и потерями
        /// </summary>
        /// <param name="inputShape"></param>
        /// <param name="outputShape"></param>
        /// <param name="loss"></param>
        protected DataSet(Shape inputShape, Shape outputShape, ILoss loss)
        {
            if (inputShape == null)
            {
                throw new ArgumentNullException(nameof(inputShape));
            }

            if (outputShape == null)
            {
                throw new ArgumentNullException(nameof(outputShape));
            }

            if (inputShape.Rank > 3)
            {
                throw new ArgumentException("Ранг входных данных должен быть меньше или равен 3", nameof(inputShape));
            }

            switch (inputShape.Rank)
            {
                case 1:
                    InputShape = new Shape3D(1, inputShape[0]);
                    break;
                case 2:
                    InputShape = new Shape3D(inputShape[1], inputShape[0]);
                    break;
                case 3:
                    InputShape = new Shape3D(inputShape[1], inputShape[0], inputShape[2]);
                    break;
                default:
                    break;
            }

            if (outputShape.Rank > 3)
            {
                throw new ArgumentException("Ранг выходных данных должен быть меньше или равен 3", nameof(inputShape));
            }

            switch (outputShape.Rank)
            {
                case 1:
                    OutputShape = new Shape3D(1, outputShape[0]);
                    break;
                case 2:
                    OutputShape = new Shape3D(outputShape[1], outputShape[0]);
                    break;
                case 3:
                    OutputShape = new Shape3D(outputShape[1], outputShape[0], outputShape[2]);
                    break;
                default:
                    break;
            }

            LossFunction = loss;

            TrainingInternal = new List<DataSequence>();
            ValidationInternal = new List<DataSequence>();
            TestingInternal = new List<DataSequence>();
        }
    }
}