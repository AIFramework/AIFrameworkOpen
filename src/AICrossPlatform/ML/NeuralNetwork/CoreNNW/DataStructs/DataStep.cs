using AI.DataStructs.Algebraic;
using System;
using System.Diagnostics;

namespace AI.ML.NeuralNetwork.CoreNNW.DataStructs
{
    /// <summary>
    /// Данные элемента последовательности
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Форма входа = {Input?.Shape.ToString(),nq}, Форма выходных данных = {TargetOutput?.Shape.ToString(),nq}")]
    public class DataStep
    {
        /// <summary>
        /// Тензор входных данных
        /// </summary>
        public NNValue Input { get; }
        /// <summary>
        /// Ideal output tensor
        /// </summary>
        public NNValue TargetOutput { get; }

        /// <summary>
        /// Данные элемента последовательности
        /// </summary>
        /// <param name="input">Тензор входных данных</param>
        /// <param name="targetOutput">Тензор выходных данных</param>
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
        /// Данные элемента последовательности
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="targetOutput">Выход</param>
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
        /// Данные элемента последовательности
        /// </summary>
        /// <param name="input">Тензор входных данных</param>
        public DataStep(NNValue input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Input = input.Clone();
        }
        /// <summary>
        /// Данные элемента последовательности
        /// </summary>
        /// <param name="input">Вход</param>
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