using AI.BackEnds.DSP.NWaves.Signals;
using System;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Base class for all kinds of LTI filters.
    /// 
    /// Provides abstract TransferFunction property
    /// and leaves methods ApplyTo() and Process() abstract.
    /// </summary>
    [Serializable]
    public abstract class LtiFilter : IFilter, IOnlineFilter
    {
        /// <summary>
        /// Передаточная функция.
        /// 
        /// It's made abstract as of ver.0.9.2 to allow subclasses using memory more efficiently.
        /// It's supposed that subclasses will generate TransferFunction object on the fly from filter coeffs
        /// OR aggregate it in internal field (only if it was set specifically from outside).
        /// 
        /// The example of the latter case is when we really need double precision for FDA
        /// or when TF was generated from precomputed poles and zeros.
        /// 
        /// The general rule is:
        /// 
        /// "Use LtiFilter subclasses for FILTERING;
        ///  Use TransferFunction class for FILTER DESIGN AND ANALYSIS".
        ///  
        /// </summary>
        public abstract TransferFunction Tf { get; protected set; }

        /// <summary>
        /// The Фильтрация всего сигнала algorithm that should be implemented by particular subclass
        /// </summary>
        /// <param name="signal">Фильтруемый(исходный) сигнал</param>
        /// <param name="method">Общая стратегия фильтрации</param>
        /// <returns>Отфильтрованный сигнал</returns>
        public abstract DiscreteSignal ApplyTo(DiscreteSignal signal,
                                               FilteringMethod method = FilteringMethod.Auto);

        /// <summary>
        /// The online filtering algorithm should be implemented by particular subclass
        /// </summary>
        /// <param name="input">Входной отсчет</param>
        /// <returns>Выходной отсчет</returns>
        public abstract float Process(float input);

        /// <summary>
        /// Перезапуск фильтра (clear all internal buffers)
        /// </summary>
        public abstract void Reset();
    }
}
