using AI.BackEnds.DSP.NWaves.Signals;
using System.Collections.Generic;

namespace AI.BackEnds.DSP.NWaves.Audio.Interfaces
{
    /// <summary>
    /// Интерфейс для контейнеров данных аудиофайлов
    /// </summary>
    public interface IAudioContainer
    {
        /// <summary>
        /// Дискретные сигналы (каналы)
        /// </summary>
        List<DiscreteSignal> Signals { get; }

        /// <summary>
        /// Индексация по типу канала
        /// </summary>
        /// <param name="channel">Типы каналов (правый, левый или чередующийся)</param>
        /// <returns></returns>
        DiscreteSignal this[Channels channel] { get; }
    }
}
