using AI.BackEnds.DSP.NWaves.Operations;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.DataStructs.Algebraic;

namespace AI.DSP
{
    /// <summary>
    /// Методы расширения для ЦОС
    /// </summary>
    public static class ExDSP
    {
        /// <summary>
        /// Децимация, с использование фильтра, обертка под NWave
        /// </summary>
        public static Vector Decimation(this Vector vector, int n)
        {
            DiscreteSignal ds = new DiscreteSignal(vector.Count, (float[])vector);
            DiscreteSignal outSignal = Operation.Decimate(ds, n);
            return outSignal.Samples;
        }
    }
}
