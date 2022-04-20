using AI.DataStructs.Algebraic;
using System;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Data step
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Input shape = {Input?.Shape.ToString(),nq}, Output shape = {TargetOutput?.Shape.ToString(),nq}")]
    public class DataStep
    {
        /// <summary>
        /// Input data tensor
        /// </summary>
        public NNValue Input { get; }
        /// <summary>
        /// Ideal output tensor
        /// </summary>
        public NNValue TargetOutput { get; }

        /// <summary>
        /// Data step
        /// </summary>
        /// <param name="input">Input data tensor</param>
        /// <param name="targetOutput">Output data tensor</param>
        public DataStep(NNValue input, NNValue targetOutput)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (targetOutput == null)
            {
                throw new ArgumentNullException(nameof(targetOutput));
            }

            Input = input.Clone();
            TargetOutput = targetOutput.Clone();
        }
        /// <summary>
        /// Data step
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="targetOutput">Output</param>
        public DataStep(double[] input, double[] targetOutput)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Input = new NNValue((Vector)input);

            if (targetOutput != null)
            {
                TargetOutput = new NNValue((Vector)targetOutput);
            }
        }
        /// <summary>
        /// Data step
        /// </summary>
        /// <param name="input">Input data tensor</param>
        public DataStep(NNValue input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Input = input.Clone();
        }
        /// <summary>
        /// Data step
        /// </summary>
        /// <param name="input">Input</param>
        public DataStep(double[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Input = new NNValue((Vector)input);
        }
    }
}