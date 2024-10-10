using AI.DSP.DSPCore;

namespace AI.DSP.Modulation
{
    /// <summary>
    /// Интерфейс модулятора
    /// </summary>
    public interface IModulator
    {
        /// <summary>
        /// Модуляция сигнала
        /// </summary>
        /// <param name="channel">Channel</param>
        Channel Modulate(Channel channel);

        /// <summary>
        /// Демодуляция сигнала
        /// </summary>
        /// <param name="channel">Channel</param>
        Channel Demodulate(Channel channel);
    }
}
