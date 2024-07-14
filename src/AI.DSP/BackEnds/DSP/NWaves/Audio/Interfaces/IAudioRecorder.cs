namespace AI.BackEnds.DSP.NWaves.Audio.Interfaces
{
    /// <summary>
    /// Интерфейс для записи звука
    /// </summary>
    public interface IAudioRecorder
    {
        /// <summary>
        /// Начать запись звука с определенными настройками
        /// </summary>
        /// <param name="samplingRate">Частота дискретизации</param>
        /// <param name="channelCount">Число каналов (1-моно, 2-стерео)</param>
        /// <param name="bitsPerSample">Количество бит на отсчет (8, 16, 24 or 32)</param>
        void StartRecording(int samplingRate, short channelCount, short bitsPerSample);

        /// <summary>
        /// Остановка записи
        /// </summary>
        /// <param name="destination">Путь для сохранения</param>
        void StopRecording(string destination);
    }
}
