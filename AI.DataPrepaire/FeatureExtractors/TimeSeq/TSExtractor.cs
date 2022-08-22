using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.FeatureExtractors.TimeSeq
{
    /// <summary>
    /// Извлечение признаков из временных рядов
    /// </summary>
    [Serializable]
    public abstract class TSExtractor : IFeaturesExtractor<Vector>
    {
        /// <summary>
        /// Получение признаков из участка данных
        /// </summary>
        /// <param name="crop">Участок данных</param>
        public abstract Vector GetFeatures(Vector crop);

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


        /// <summary>
        /// Получение признаков из временной последовательности
        /// </summary>
        public virtual Vector[] GetFeatures(Vector[] data)
        {
            Vector[] ret = new Vector[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                ret[i] = GetFeatures(data[i]);
            }

            return ret;
        }
    }
}
