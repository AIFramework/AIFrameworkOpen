using System;

namespace AI.Logic.Data
{
    /// <summary>
    /// Система мутации
    /// </summary>
    /// <typeparam name="ImageType"></typeparam>
    /// <typeparam name="ElementType"></typeparam>
    [Serializable]
    public abstract class DataMutation<ImageType, ElementType> 
    {
        /// <summary>
        /// Целевой образ
        /// </summary>
        public ImageType Image { get; set; }

        /// <summary>
        /// Мутация элемента данных
        /// </summary>
        public abstract ElementType Transformer(ElementType dataInput);
    }
}
