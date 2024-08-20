using AI.BackEnds.DSP.NWaves.Filters.Base;

namespace AI.BackEnds.DSP.NWaves.Filters.Base64
{
    /// <summary>
    /// Интерфейс для любого типа фильтра:
    /// фильтр может применяться к любому сигналу, преобразуя его в некоторый выходной сигнал.
    /// </summary>
    public interface IFilter64
    {
        /// <summary>
        /// Фильтрация всего сигнала
        /// </summary>
        /// <param name="signal">Фильтруемый(исходный) сигнал</param>
        /// <param name="method">Общая стратегия фильтрации</param>
        /// <returns>Отфильтрованный сигнал</returns>
        double[] ApplyTo(double[] signal, FilteringMethod method = FilteringMethod.Auto);
    }
}
