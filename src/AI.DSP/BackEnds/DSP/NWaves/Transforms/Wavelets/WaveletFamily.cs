using System;


namespace AI.BackEnds.DSP.NWaves.Transforms.Wavelets
{
    /// <summary>
    /// Wavelet family type
    /// </summary>
    [Serializable]
    public enum WaveletFamily
    {
        /// <summary>
        /// Haar wavelet
        /// </summary>
        Haar,
        /// <summary>
        /// Daubechies wavelet
        /// </summary>
        Daubechies,
        /// <summary>
        /// Coiflet wavelet
        /// </summary>
        Coiflet,
        /// <summary>
        /// Symlet wavelet
        /// </summary>
        Symlet
    }
}
