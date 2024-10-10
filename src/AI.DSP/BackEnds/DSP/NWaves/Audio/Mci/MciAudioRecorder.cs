using AI.BackEnds.DSP.NWaves.Audio.Interfaces;
using System;

namespace AI.BackEnds.DSP.NWaves.Audio.Mci
{
    /// <summary>
    /// Audio recorder based on MCI.
    /// 
    /// MciAudioRecorder works only with Windows, since it uses winmm.dll and MCI commands.
    /// </summary>
    [Serializable]
    public class MciAudioRecorder : IAudioRecorder
    {
        /// <summary>
        /// Начать запись звука с определенными настройками
        /// </summary>
        /// <param name="samplingRate">Частота дискретизации</param>
        /// <param name="channelCount">Число каналов (1-моно, 2-стерео)</param>
        /// <param name="bitsPerSample">Количество бит на отсчет (8, 16, 24 or 32)</param>
        public void StartRecording(int samplingRate = 44100, short channelCount = 1, short bitsPerSample = 16)
        {
            string mciCommand = "open new type waveaudio alias capture";
            int result = Mci.SendString(mciCommand, null, 0, 0);

            if (result != 0)
            {
                throw new InvalidOperationException("Could not open device for recording!");
            }

            mciCommand = string.Format("set capture alignment {0} bitspersample {1} samplespersec {2} " +
                                       "channels {3} bytespersec {4} time format samples format tag pcm",
                                       channelCount * bitsPerSample / 8,
                                       bitsPerSample,
                                       samplingRate,
                                       channelCount,
                                       samplingRate * channelCount * bitsPerSample / 8);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            mciCommand = "record capture";
            _ = Mci.SendString(mciCommand, null, 0, 0);
        }

        /// <summary>
        /// Остановка запись и сохранение в wave
        /// </summary>
        /// <param name="destination">Выходной WAV-файл, содержащий записанный звук</param>
        public void StopRecording(string destination)
        {
            string mciCommand = "stop capture";
            _ = Mci.SendString(mciCommand, null, 0, 0);

            mciCommand = string.Format("save capture {0}", destination);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            mciCommand = "close capture";
            _ = Mci.SendString(mciCommand, null, 0, 0);
        }
    }
}
