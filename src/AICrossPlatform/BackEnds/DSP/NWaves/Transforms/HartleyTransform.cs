using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{

    /// <summary>
    /// Преобразование Гильберта
    /// </summary>
    [Serializable]

    public class HartleyTransform
    {
        /// <summary>
        /// Size of transform
        /// </summary>
        /// 
        public int Size { get; private set; }

        /// <summary>
        /// Внутренний алгоритм для выполнения БПФ
        /// </summary>
        private readonly Fft _fft;

        /// <summary>
        /// Internal array for imaginary parts
        /// </summary>
        private readonly float[] _im;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="size"></param>
        public HartleyTransform(int size)
        {
            Size = size;
            _fft = new Fft(size);
            _im = new float[size];
        }

        /// <summary>
        /// Direct transform
        /// </summary>
        /// <param name="re"></param>
        public void Direct(float[] re)
        {
            for (int i = 0; i < _im.Length; i++)
            {
                _im[i] = 0;
            }

            _fft.Direct(re, _im);

            for (int i = 0; i < re.Length; i++)
            {
                re[i] -= _im[i];
            }
        }

        /// <summary>
        /// Inverse transform
        /// </summary>
        /// <param name="re"></param>
        public void Inverse(float[] re)
        {
            _im[0] = 0;

            for (int i = 1; i <= re.Length / 2; i++)
            {
                float x = (re[Size - i] - re[i]) * 0.5f;
                _im[i] = x;
                _im[Size - i] = -x;

                x = (re[i] + re[Size - i]) * 0.5f;
                re[i] = re[Size - i] = x;
            }

            _fft.Inverse(re, _im);

            for (int i = 0; i < re.Length; i++)
            {
                re[i] /= Size;
            }
        }
    }
}
