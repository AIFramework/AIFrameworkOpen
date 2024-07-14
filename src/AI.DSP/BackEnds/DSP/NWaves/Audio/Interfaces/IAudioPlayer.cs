using AI.BackEnds.DSP.NWaves.Signals;
using System.Threading.Tasks;

namespace AI.BackEnds.DSP.NWaves.Audio.Interfaces
{
    /// <summary>
    /// Интерфейс для воспроизведения звука
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Громкость звука в диапазоне [0.0f, 1.0f]
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Асинхронное воспроизведение звука
        /// </summary>
        /// <param name="signal">Сигнал для проигрывания</param>
        /// <param name="startPos">Начальная позиция для воспроизведения</param>
        /// <param name="endPos">Конечная позиция для проигрывания (-1 — проигрывать весь файл)</param>
        /// <param name="bitDepth">Number of bits per one sample</param>
        Task PlayAsync(DiscreteSignal signal, int startPos = 0, int endPos = -1, short bitDepth = 16);

        /// <summary>
        /// Воспроизведение сигналов, содержащихся в файле WAV (или другом источнике), асинхронно
        /// </summary>
        /// <param name="source">WAV-файл (или другой источник) для воспроизведения</param>
        /// <param name="startPos">Начальная позиция для воспроизведения</param>
        /// <param name="endPos">Конечная позиция для проигрывания (-1 — проигрывать весь файл)</param>
        Task PlayAsync(string source, int startPos = 0, int endPos = -1);

        /// <summary>
        /// Пауза
        /// </summary>
        void Pause();

        /// <summary>
        /// Возобновление воспроизведения аудио
        /// </summary>
        void Resume();

        /// <summary>
        /// Остановка воспроизведения
        /// </summary>
        void Stop();
    }
}
