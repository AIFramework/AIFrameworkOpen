using AI.BackEnds.DSP.NWaves.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Options
{
    /// <summary>
    /// Настройки для извлечения признаков из сигнала
    /// </summary>
    [Serializable]
    [DataContract]
    public class FeatureExtractorOptions
    {
        /// <summary>
        /// Число признаков
        /// </summary>
        [DataMember]
        public int FeatureCount { get; set; }

        /// <summary>
        /// Частота дискретизации
        /// </summary>
        [DataMember]
        public int SamplingRate { get; set; }

        /// <summary>
        /// Длительность участка
        /// </summary>
        [DataMember]
        public double FrameDuration { get; set; } = 0.025;
        /// <summary>
        /// Длительность прыжка
        /// </summary>
        [DataMember]
        public double HopDuration { get; set; } = 0.01;
        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public double PreEmphasis { get; set; } = 0;
        /// <summary>
        /// Весовое окно
        /// </summary>
        [DataMember]
        public WindowTypes Window { get; set; } = WindowTypes.Rectangular;

        /// <summary>
        /// Список ошибок
        /// </summary>
        public virtual List<string> Errors
        {
            get
            {
                List<string> errors = new List<string>();

                if (SamplingRate <= 0)
                {
                    errors.Add("Должна быть указана положительная частота дискретизации (указана отрицательная либо 0).");
                }

                if (FrameDuration <= 0)
                {
                    errors.Add("Должна быть указана положительная продолжительность кадра (указана отрицательная либо 0).");
                }

                if (HopDuration <= 0)
                {
                    errors.Add("Должна быть указана продолжительность положительного прыжка (указана отрицательная либо 0).");
                }

                return errors;
            }
        }
    }
}
