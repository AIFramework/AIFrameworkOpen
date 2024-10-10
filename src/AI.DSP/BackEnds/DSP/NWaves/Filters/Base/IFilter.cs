using AI.BackEnds.DSP.NWaves.Signals;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Интерфейс для любого типа фильтра:
    /// фильтр может применяться к любому сигналу, преобразуя его в некоторый выходной сигнал.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Метод реализует алгоритм фильтрации всего сигнала
        /// </summary>
        /// <param name="signal">Фильтруемый(исходный) сигнал</param>
        /// <param name="method">Общая стратегия фильтрации</param>
        /// <returns>Отфильтрованный сигнал</returns>
        DiscreteSignal ApplyTo(DiscreteSignal signal, FilteringMethod method = FilteringMethod.Auto);
    }
}
