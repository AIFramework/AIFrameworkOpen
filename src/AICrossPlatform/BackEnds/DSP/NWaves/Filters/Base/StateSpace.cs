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
        /// Входная матрица
        /// </summary>
        public double[] B { get; set; }

        /// <summary>
        /// Выходная матрица
        /// </summary>
        public double[] C { get; set; }

        /// <summary>
        /// Проходная матрица
        /// </summary>
        public double[] D { get; set; }
    }
}
