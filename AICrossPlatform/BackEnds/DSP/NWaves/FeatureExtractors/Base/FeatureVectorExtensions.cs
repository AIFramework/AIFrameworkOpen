using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Base
{
    /// <summary>
    /// Извлечение статистических признаков
    /// </summary>
    [Serializable]
    public static class FeatureVectorExtensions
    {
        /// <summary>
        /// Словарь со статистиками
        /// </summary>
        public static Dictionary<string, float> Statistics(this float[] vector)
        {
            float mean = vector.Average();

            return new Dictionary<string, float>
            {
                { "min",  vector.Min() },
                { "max",  vector.Max() },
                { "mean", mean },
                { "var",  vector.Average(v => (v - mean) * (v - mean)) },
            };
        }
    }
}
