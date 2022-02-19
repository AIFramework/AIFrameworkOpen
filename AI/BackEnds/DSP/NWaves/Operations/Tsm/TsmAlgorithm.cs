﻿namespace AI.BackEnds.DSP.NWaves.Operations.Tsm
{
    /// <summary>
    /// Algorithm for time scale modification
    /// </summary>
    public enum TsmAlgorithm
    {
        /// <summary>
        /// Phase vocoder
        /// </summary>
        PhaseVocoder = 0,

        /// <summary>
        /// Phase vocoder with phase-locking
        /// </summary>
        PhaseVocoderPhaseLocking = 1,

        /// <summary>
        /// Waveform similarity-based Synchrnoized Overlap-Add
        /// </summary>
        Wsola = 2,

        /// <summary>
        /// Paul stretch
        /// </summary>
        PaulStretch = 3
    }
}