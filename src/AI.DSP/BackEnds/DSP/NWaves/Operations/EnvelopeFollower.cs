using AI.BackEnds.DSP.NWaves.Filters.Base;
using System;

namespace AI.BackEnds.DSP.NWaves.Operations
{
    /// <summary>
    /// Огибающая (detector)
    /// </summary>
    [Serializable]
    public class EnvelopeFollower : IOnlineFilter
    {
        /// <summary>
        /// Время атаки
        /// </summary>
        private float _attackTime;

        /// <summary>
        /// 
        /// </summary>
        public float AttackTime
        {
            get => _attackTime;
            set
            {
                _attackTime = value;
                _ga = (float)Math.Exp(-1.0 / (value * _fs));
            }
        }

        /// <summary>
        /// Release time
        /// </summary>
        private float _releaseTime;
        /// <summary>
        /// 
        /// </summary>
        public float ReleaseTime
        {
            get => _releaseTime;
            set
            {
                _releaseTime = value;
                _gr = (float)Math.Exp(-1.0 / (value * _fs));
            }
        }

        /// <summary>
        /// Current envelope sample
        /// </summary>
        private float _env;

        /// <summary>
        /// Attack coefficient
        /// </summary>
        private float _ga;

        /// <summary>
        /// Release coefficient
        /// </summary>
        private float _gr;

        /// <summary>
        /// Частота дискретизации
        /// </summary>
        private readonly int _fs;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="samplingRate"></param>
        /// <param name="attackTime"></param>
        /// <param name="releaseTime"></param>
        public EnvelopeFollower(int samplingRate, float attackTime = 0.01f, float releaseTime = 0.05f)
        {
            _fs = samplingRate;
            AttackTime = attackTime;
            ReleaseTime = releaseTime;
        }

        /// <summary>
        /// Envelope following is essentialy a Фильтр нижних частот filtering
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public float Process(float input)
        {
            float sample = Math.Abs(input);

            _env = _env < sample ? (_ga * _env) + ((1 - _ga) * sample) : (_gr * _env) + ((1 - _ga) * sample);

            return _env;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _env = 0;
        }
    }
}
