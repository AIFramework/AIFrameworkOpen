namespace AI.BackEnds.DSP.NWaves.Filters.Base64
{
    /// <summary>
    /// Интерфейс для всех объектов, поддерживающих онлайн-фильтрацию
    /// </summary>
    public interface IOnlineFilter64
    {
        /// <summary>
        /// Метод реализует онлайн фильтрацию (отсчет за отсчетом)
        /// </summary>
        /// <param name="input">Входной отсчет</param>
        /// <returns>Выходной отсчет</returns>
        double Process(double input);

        /// <summary>
        /// Сброс состояния
        /// </summary>
        void Reset();
    }
}
