using System;

namespace AI.BackEnds.DSP.NWaves.Features
{
    /// <summary>
    /// Perceptual features
    /// </summary>
    public static class Perceptual
    {
        /// <summary>
        /// Perceptual loudness (is the sum of specific loudnesses: N'(z) = E(z)^0.23)
        /// </summary>
        /// <param name="spectralBands"></param>
        /// <returns></returns>
        public static float Loudness(float[] spectralBands)
        {
            double loudness = 0.0;

            for (int i = 0; i < spectralBands.Length; i++)
            {
                loudness += Math.Pow(spectralBands[i], 0.23);
            }

            return (float)loudness;
        }

        /// <summary>
        /// Perceptual sharpness (is essentially the equivalent of spectral centroid).
        /// 
        /// According to the original formula, the weights are slightly different 
        /// for bark bands with index >= 15, but this implementation assumes
        /// that there will be no more than 15 bands.
        /// </summary>
        /// <param name="spectralBands"></param>
        /// <returns></returns>
        public static float Sharpness(float[] spectralBands)
        {
            double sharpness = 0.0;
            double total = 0.0;

            for (int i = 0; i < spectralBands.Length; i++)
            {
                double loudness = Math.Pow(spectralBands[i], 0.23);
                sharpness += (i + 1) * loudness;
                total += loudness;
            }

            return (float)(sharpness / total * 0.11);
        }
    }
}
