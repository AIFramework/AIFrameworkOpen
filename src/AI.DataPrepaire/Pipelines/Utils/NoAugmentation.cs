using System;

namespace AI.DataPrepaire.Pipelines.Utils
{
    /// <summary>
    /// Заглушка для аугментации
    /// </summary>
    [Serializable]
    public class NoAugmentation<T> : DataAugmetation<T>
    {
        /// <summary>
        /// Заглушка для аугментации
        /// </summary>
        public NoAugmentation() : base(1)
        {

        }

        /// <summary>
        /// Аугментация
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public override T[] Augmetation(T sample)
        {
            return new[] { sample };
        }
    }
}
