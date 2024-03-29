﻿using System;

namespace AI.BackEnds.DSP.NWaves.Transforms
{
    /// <summary>
    /// Быстрое дискретно-косинусное преобразование 2, через преобразование Фурье
    /// </summary>
    [Serializable]
    public class FastDct2 : IDct
    {
        /// <summary>
        /// Внутренний алгоритм для выполнения БПФ
        /// </summary>
        private readonly Fft _fft;

        /// <summary>
        /// Внутренний буффер памяти
        /// </summary>
        private readonly float[] _temp;

        /// <summary>
        /// Size of DCT
        /// </summary>
        public int Size => _fft.Size;

        /// <summary>
        /// Быстрое дискретно-косинусное преобразование 3, через преобразование Фурье
        /// </summary>
        public FastDct2(int dctSize)
        {
            _fft = new Fft(dctSize);
            _temp = new float[dctSize];
        }

        /// <summary>
        /// Прямое преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Direct(float[] input, float[] output)
        {
            Array.Clear(output, 0, output.Length);

            for (int m = 0; m < _temp.Length / 2; m++)
            {
                _temp[m] = input[2 * m];
                _temp[_temp.Length - 1 - m] = input[(2 * m) + 1];
            }

            _fft.Direct(_temp, output);

            // mutiply by exp(-j * pi * n / 2N):

            int N = _fft.Size;
            for (int i = 0; i < N; i++)
            {
                output[i] = 2 * (float)((_temp[i] * Math.Cos(0.5 * Math.PI * i / N)) - (output[i] * Math.Sin(-0.5 * Math.PI * i / N)));
            }
        }

        /// <summary>
        /// Прямое нормированное преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void DirectNorm(float[] input, float[] output)
        {
            Array.Clear(output, 0, output.Length);

            for (int m = 0; m < _temp.Length / 2; m++)
            {
                _temp[m] = input[2 * m];
                _temp[_temp.Length - 1 - m] = input[(2 * m) + 1];
            }

            _fft.Direct(_temp, output);

            // mutiply by exp(-j * pi * n / 2N):

            int N = _fft.Size;
            float norm = (float)Math.Sqrt(0.5 / N);

            for (int i = 0; i < N; i++)
            {
                output[i] = 2 * norm * (float)((_temp[i] * Math.Cos(0.5 * Math.PI * i / N)) - (output[i] * Math.Sin(-0.5 * Math.PI * i / N)));
            }

            output[0] *= (float)Math.Sqrt(0.5);
        }

        /// <summary>
        /// Обратное преобразование
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void Inverse(float[] input, float[] output)
        {
            // multiply by exp(j * pi * n / 2N):

            int N = _fft.Size;
            for (int i = 0; i < N; i++)
            {
                _temp[i] = (float)(input[i] * Math.Cos(0.5 * Math.PI * i / N));
                output[i] = (float)(input[i] * Math.Sin(0.5 * Math.PI * i / N));
            }
            _temp[0] *= 0.5f;
            output[0] *= 0.5f;

            _fft.Inverse(_temp, output);

            for (int m = 0; m < _temp.Length / 2; m++)
            {
                output[2 * m] = 2 * _temp[m];
                output[(2 * m) + 1] = 2 * _temp[N - 1 - m];
            }
        }

        /// <summary>
        /// Обратное нормированное преобразование (нереализовано)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void InverseNorm(float[] input, float[] output)
        {
            throw new NotImplementedException();
        }
    }
}
