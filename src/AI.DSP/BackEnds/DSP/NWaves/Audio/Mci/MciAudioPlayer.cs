using AI.BackEnds.DSP.NWaves.Audio.Interfaces;
using AI.BackEnds.DSP.NWaves.Signals;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AI.BackEnds.DSP.NWaves.Audio.Mci
{
    /// <summary>
    /// Аудиоплеер, основанный на MCI. 
    /// MciAudioPlayer работает только в Windows, поскольку использует winmm.dll и команды MCI. 
    /// MciAudioPlayer позволяет MCI выполнять всю тяжелую работу по воспроизведению звука. 
    /// Он запускает команду MCI и просто ожидает некоторое время, соответствующее длительности заданного сегмента. 
    /// Если воспроизведение было приостановлено, плеер запоминает, сколько миллисекунд он "простаивал", а затем добавляет это время к общему времени ожидания.
    /// </summary>
    [Serializable]
    public class MciAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// Скрытый псевдоним для устройства MCI waveaudio
        /// </summary>
        private string _alias;

        /// <summary>
        /// Длительность паузы в миллисекундах
        /// </summary>
        private int _pauseDuration;

        /// <summary>
        /// Точное время, когда воспроизведение было приостановлено
        /// </summary>
        private DateTime _pauseTime;

        /// <summary>
        /// Флаг, указывающий, приостановлено ли в данный момент воспроизведение аудио.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Громкость (измеряется в процентах из диапазона [0.0f, 1.0f])
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        /// Асинхронное воспроизведение аудио из WAV-файла
        /// </summary>
        /// <param name="source">WAV-файл</param>
        /// <param name="startPos">Начальная позиция для воспроизведения</param>
        /// <param name="endPos">Конечная позиция для проигрывания (-1 — проигрывать весь файл)</param>
        public async Task PlayAsync(string source, int startPos = 0, int endPos = -1)
        {
            if (_isPaused)
            {
                Resume();
                return;
            }

            Stop();

            _alias = Guid.NewGuid().ToString();

            string mciCommand = string.Format("open \"{0}\" type waveaudio alias {1}", source, _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            mciCommand = string.Format("set {0} time format samples", _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            StringBuilder durationBuffer = new StringBuilder(255);
            mciCommand = string.Format("status {0} length", _alias);
            _ = Mci.SendString(mciCommand, durationBuffer, 255, 0);
            int duration = int.Parse(durationBuffer.ToString());

            StringBuilder samplingRateBuffer = new StringBuilder(255);
            mciCommand = string.Format("status {0} samplespersec", _alias);
            _ = Mci.SendString(mciCommand, samplingRateBuffer, 255, 0);
            int samplingRate = int.Parse(samplingRateBuffer.ToString());

            mciCommand = string.Format("play {2} from {0} to {1} notify", startPos, endPos, _alias);
            mciCommand = mciCommand.Replace(" to -1", "");
            _ = Mci.SendString(mciCommand, null, 0, 0);

            // ======= here's how we do asynchrony with old technology from 90's )) ========

            string currentAlias = _alias;

            await Task.Delay((int)(duration * 1000.0 / samplingRate));

            // During this time someone could Pause player.
            // In this case we add pause duration to awaiting time.

            while (_isPaused || _pauseDuration > 0)
            {
                // first, we check if the pause is right now
                if (_isPaused)
                {
                    // then just await one second more
                    // (the stupidest wait spin I wrote in years ))))
                    await Task.Delay(1000);
                }

                if (_pauseDuration > 0)
                {
                    await Task.Delay(_pauseDuration);

                    _pauseDuration = 0;
                }
            }

            // During this time someone could stop and run player again, so _alias is already different.
            // In this case we don't stop player here because it was stopped some time before.

            if (currentAlias == _alias)
            {
                Stop();
            }
            // =============================================================================
        }

        /// <summary>
        ///К сожалению, MCI не предоставляет средств для воспроизведения аудио из буферов в памяти.Более того, поскольку библиотека NWaves является переносимой, 
        /// нет даже простого способа записать буфер во временный файл и воспроизвести его здесь (это может быть обходным решением проблемы).
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="bitDepth"></param>
        public Task PlayAsync(DiscreteSignal signal, int startPos = 0, int endPos = -1, short bitDepth = 16)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Пауза
        /// </summary>
        public void Pause()
        {
            if (_alias == null)
            {
                return;
            }

            string mciCommand = string.Format("pause {0}", _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            _pauseTime = DateTime.Now;
            _isPaused = true;
        }

        /// <summary>
        /// Возобновление воспроизведения аудио
        /// </summary>
        public void Resume()
        {
            if (_alias == null || !_isPaused)
            {
                return;
            }

            string mciCommand = string.Format("resume {0}", _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            TimeSpan pause = DateTime.Now - _pauseTime;

            _pauseDuration += (pause.Duration().Seconds * 1000) + pause.Duration().Milliseconds;
            _isPaused = false;
        }

        /// <summary>
        /// Остановить проигрывание и закрыть MCI девайс
        /// </summary>
        public void Stop()
        {
            if (_alias == null)
            {
                return;
            }

            if (_isPaused)
            {
                Resume();
            }

            string mciCommand = string.Format("stop {0}", _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            mciCommand = string.Format("close {0}", _alias);
            _ = Mci.SendString(mciCommand, null, 0, 0);

            _alias = null;
        }
    }
}
