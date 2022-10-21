using System;

namespace AI.ML.HMM
{
    /// <summary>
    /// Слово
    /// </summary>
    [Serializable]
    public class MCNextToken
    {
        /// <summary>
        /// Токен
        /// </summary>
		public int Value { get; }
        /// <summary>
        /// Вероятность
        /// </summary>
		public double Probability { get; }

        /// <summary>
        /// Слово
        /// </summary>
        public MCNextToken(int val, double pr)
        {
            Value = val;
            Probability = pr;
        }
    }
}
