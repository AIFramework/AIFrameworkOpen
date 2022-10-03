using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Данные последовательности
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Step count = {Steps.Count}")]
    public class DataSequence
    {
        /// <summary>
        /// Элементы последовательности
        /// </summary>
        public IReadOnlyList<DataStep> Steps { get; }

        /// <summary>
        /// Данные последовательности
        /// </summary>
        /// <param name="steps">Массив шагов</param>
        public DataSequence(params DataStep[] steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            if (steps.Length == 0)
            {
                throw new ArgumentException("Число шагов не может быть 0", nameof(steps));
            }

            Steps = new List<DataStep>(steps).AsReadOnly();
        }
    }
}