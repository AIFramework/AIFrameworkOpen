using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Text;

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
