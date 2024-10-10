using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Utils;

namespace AI.BackEnds.DSP.NWaves.Windows
{
    /// <summary>
    /// A few helper functions for applying windows to signals and arrays of samples
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Mutable function that applies window array to array of float samples
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="windowSamples"></param>
        public static void ApplyWindow(this float[] samples, float[] windowSamples)
        {
            for (int k = 0; k < windowSamples.Length; k++)
            {
                samples[k] *= windowSamples[k];
            }
        }

        /// <summary>
        /// Mutable function that applies window array to array of double samples
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="windowSamples"></param>
        public static void ApplyWindow(this double[] samples, double[] windowSamples)
        {
            for (int k = 0; k < windowSamples.Length; k++)
            {
                samples[k] *= windowSamples[k];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="windowSamples"></param>
        public static void ApplyWindow(this DiscreteSignal signal, float[] windowSamples)
        {
            signal.Samples.ApplyWindow(windowSamples);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplyWindow(this float[] samples, WindowTypes window, params object[] parameters)
        {
            float[] windowSamples = Window.OfType(window, samples.Length, parameters);
            samples.ApplyWindow(windowSamples);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplyWindow(this double[] samples, WindowTypes window, params object[] parameters)
        {
            double[] windowSamples = Window.OfType(window, samples.Length, parameters).ToDoubles();
            samples.ApplyWindow(windowSamples);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplyWindow(this DiscreteSignal signal, WindowTypes window, params object[] parameters)
        {
            float[] windowSamples = Window.OfType(window, signal.Length, parameters);
            signal.Samples.ApplyWindow(windowSamples);
        }
    }
}
