namespace AI.DataPrepaire.Pipelines.Utils
{
    /// <summary>
    /// Детектор событий/объектов
    /// </summary>
    public interface IDetector<T>
    {
        /// <summary>
        /// Является ли данный объект элементом целевой группы
        /// </summary>
        bool IsDetected(T obj);
    }
}
