using AI.DataStructs.Algebraic;

namespace AI.DataPrepaire.FeatureExtractors
{
    /// <summary>
    /// Извлечение признаков из объектов
    /// </summary>
    public abstract class FeaturesExtractor<T>
    {
        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        public abstract Vector GetFeatures(T data);

        /// <summary>
        /// Получение признаков из данных
        /// </summary>
        public virtual Vector[] GetFeatures(T[] data)
        {
            Vector[] features = new Vector[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                features[i] = GetFeatures(data[i]);
            }

            return features;
        }

    }
}
