using AI.DataStructs.WithComplexElements;
using System;
using System.Globalization;

namespace AI
{
    /// <summary>
    /// Global settings
    /// </summary>
    public static class AISettings
    {
        private static readonly NumberFormatInfo s_provider;
        private static readonly NumberFormatInfo s_providerComa;
        /// <summary>
        /// Global epsilon (default = 1e-8)
        /// </summary>
        public static double GlobalEps { get; set; } = 1e-8;

        /// <summary>
        /// Basic function for fft
        /// </summary>
        public static Func<ComplexVector, bool, ComplexVector> FFTCore { get; set; } = FFT.BaseStaticFFT;

        static AISettings()
        {
            s_provider = new NumberFormatInfo
            {
                NumberDecimalSeparator = ".",
                NumberGroupSeparator = string.Empty,
            };

            s_providerComa = new NumberFormatInfo
            {
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = string.Empty,
            };
        }

        /// <summary>
        /// Get a provider for a dot as decimal separator conversion 
        /// </summary>
        public static NumberFormatInfo GetProvider()
        {
            return s_provider;
        }
        /// <summary>
        /// Get a comma conversion provider
        /// </summary>
        public static NumberFormatInfo GetProviderComa()
        {
            return s_providerComa;
        }
    }
}