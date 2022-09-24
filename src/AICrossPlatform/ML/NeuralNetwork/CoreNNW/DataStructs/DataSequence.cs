using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Данные sequence used in network training
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Step count = {Steps.Count}")]
    public class DataSequence
    {
        /// <summary>
        /// Sequence of steps
        /// </summary>
        public IReadOnlyList<DataStep> Steps { get; }

        /// <summary>
        /// Данные sequence
        /// </summary>
        /// <param name="steps">Array of steps</param>
        public DataSequence(params DataStep[] steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            if (steps.Length == 0)
            {
                throw new ArgumentException("Step count can't be 0", nameof(steps));
            }

            Steps = new List<DataStep>(steps).AsReadOnly();
        }
    }
}