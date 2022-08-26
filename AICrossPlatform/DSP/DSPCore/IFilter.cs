using AI.DataStructs.Algebraic;

namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Интерфейс фильтра
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Имя фильтра
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Запуск фильтрации
        /// </summary>
        /// <param name="signal">Исходный сигнал</param>
        /// <returns>Отфильтрованный</returns>
        Vector FilterOutp(Vector signal);
    }
}
