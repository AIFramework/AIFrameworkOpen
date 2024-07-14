using AI.BackEnds.DSP.NWaves.Signals;
using System;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Фильтр для фильтрации данных в чередующихся стереофонических буферах
    /// </summary>
    [Serializable]
    public class StereoFilter : IFilter, IOnlineFilter
    {
        /// <summary>
        /// Фильтр для левого канала
        /// </summary>
        private readonly IOnlineFilter _filterLeft;

        /// <summary>
        /// Фильтр для правого канала
        /// </summary>
        private readonly IOnlineFilter _filterRight;

        /// <summary>
        /// Внутренний флаг для переключения между левым и правым каналами
        /// </summary>
        private bool _isRight;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="filterLeft"></param>
        /// <param name="filterRight"></param>
        public StereoFilter(IOnlineFilter filterLeft, IOnlineFilter filterRight)
        {
            _filterLeft = filterLeft;
            _filterRight = filterRight;
        }

        /// <summary>
        /// Онлайн-фильтрация
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public float Process(float input)
        {
            if (_isRight)
            {
                _isRight = false;
                return _filterRight.Process(input);
            }
            else
            {
                _isRight = true;
                return _filterLeft.Process(input);
            }
        }

        /// <summary>
        /// Перезапуск фильтраs
        /// </summary>
        public void Reset()
        {
            _filterLeft.Reset();
            _filterRight.Reset();
        }

        /// <summary>
        /// Фильтрация всего сигнала
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public DiscreteSignal ApplyTo(DiscreteSignal signal, FilteringMethod method = FilteringMethod.Auto)
        {
            return new DiscreteSignal(signal.SamplingRate, signal.Samples.Select(s => Process(s)));
        }
    }
}
