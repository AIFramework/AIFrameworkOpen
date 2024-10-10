namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDct
    {
        /// <summary>
        /// Число отсчетов ДКП
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Direct DCT
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        void Direct(float[] input, float[] output);

        /// <summary>
        /// Direct normalized DCT
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        void DirectNorm(float[] input, float[] output);

        /// <summary>
        /// Inverse DCT
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        void Inverse(float[] input, float[] output);

        /// <summary>
        /// Inverse normalized DCT
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        void InverseNorm(float[] input, float[] output);
    }
}
