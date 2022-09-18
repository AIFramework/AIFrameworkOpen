using AI.DataStructs.Algebraic;
using System;

namespace AI.DataPrepaire.FeatureExtractors.TimeSeq
{
    /// <summary>
    /// Извлечение признаков из временных рядов
    /// </summary>
    [Serializable]
    public abstract class TSExtractor : FeaturesExtractor<Vector>
    {

        /// <summary>
        /// Получение признаков из временной последовательности
        /// </summary>
        /// <param name="timeSeq">Последовательность</param>
        /// <param name="cropSize">Размер участка данных</param>
        public virtual Vector[] GetFeatures(Vector timeSeq, int cropSize)
        {
            Vector[] features = Vector.GetWindowsWithFunc(GetFeatures, timeSeq, cropSize, cropSize);
            return features;
        }

    }
}
