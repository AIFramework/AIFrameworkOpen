namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Интерфейс для всех объектов, поддерживающих онлайн-фильтрацию
    /// </summary>
    public interface IOnlineFilter
    {
        /// <summary>
        /// Метод реализует онлайн фильтрацию (отсчет за отсчетом)
        /// </summary>
        /// <param name="input">Входной отсчет</param>
        /// <returns>Выходной отсчет</returns>
        float Process(float input);

        /// <summary>
        /// Сброс состояния
        /// </summary>
        void Reset();
    }
}
