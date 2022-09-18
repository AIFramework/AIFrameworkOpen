using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Быстрое дискретно-косинусное преобразование 3, через преобразование Фурье
    /// </summary>
    [Serializable]
    public class FastDct3 : IDct
    {
        /// <summary>
        /// Internal DCT-II transformer
        /// </summary>
        private readonly FastDct2 _dct2;

        /// <summary>
        /// Число отсчетов ДКП
        /// </summary>
        public int Size => _dct2.Size;


        /// <summary>
        /// Быстрое дискретно-косинусное преобразование 3, через преобразование Фурье
        /// </summary>
        public FastDct3(int dctSize)
        {
            _dct2 = new FastDct2(dctSize);
        }
        /// <summary>
        /// Прямое преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Direct(float[] input, float[] output)
        {
            _dct2.Inverse(input, output);
        }

        /// <summary>
        /// Прямое нормированное преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void DirectNorm(float[] input, float[] output)
        {
            _dct2.InverseNorm(input, output);
        }

        /// <summary>
        /// Обратное преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Inverse(float[] input, float[] output)
        {
            _dct2.Direct(input, output);
        }

        /// <summary>
        /// Обратное нормированное преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void InverseNorm(float[] input, float[] output)
        {
            _dct2.DirectNorm(input, output);
        }
    }
}
