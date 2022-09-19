using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Пространство состояний
    /// </summary>
    [Serializable]
    public class StateSpace
    {
        /// <summary>
        /// Матрица состояний
        /// </summary>
        public double[][] A { get; set; }

        /// <summary>
        /// Input-to-Матрица состояний
        /// </summary>
        public double[] B { get; set; }

        /// <summary>
        /// State-to-output matrix
        /// </summary>
        public double[] C { get; set; }

        /// <summary>
        /// Feedthrough matrix
        /// </summary>
        public double[] D { get; set; }
    }
}
