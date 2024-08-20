using AI.DataStructs.WithComplexElements;
using System;

namespace AI.DSP.DSPCore
{
    [Serializable]
    public static class AIDSPSettings
    {
        /// <summary>
        /// Базовая функция БПФ
        /// </summary>
        public static Func<ComplexVector, bool, ComplexVector> FFTCore { get; set; } = FFT.BaseStaticFFT;
    }
}
