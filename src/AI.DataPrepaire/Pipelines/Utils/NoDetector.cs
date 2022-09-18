using System;

namespace AI.DataPrepaire.Pipelines.Utils
{

    /// <summary>
    /// Не детектор (простой детектор - заглушка, который всегда возвращает false)
    /// </summary>
    [Serializable]
    public class NoDetector<T> : IDetector<T>
    {
        /// <summary>
        /// Всегла возвращает false
        /// </summary>
        public bool IsDetected(T obj)
        {
            return false;
        }
    }
}
