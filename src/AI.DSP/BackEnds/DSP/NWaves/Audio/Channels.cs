using System;

namespace AI.BackEnds.DSP.NWaves.Audio
{
    /// <summary>
    /// Наиболее используемые каналы: левый и правый.
    /// Также мы добавляем особый случай: чередующиеся каналы
    /// </summary>
    [Serializable]
    public enum Channels
    {
        /// <summary>
        /// Левый канал (=0)
        /// </summary>
        Left,

        /// <summary>
        /// Правый канал (=1)
        /// </summary>
        Right,

        /// <summary>
        /// Моно как сумма всех каналов
        /// </summary>
        Sum = 253,

        /// <summary>
        /// Моно как среднее всех каналов
        /// </summary>
        Average = 254,

        /// <summary>
        /// Перемешаный канал
        /// </summary>
        Interleave = 255
    }
}
