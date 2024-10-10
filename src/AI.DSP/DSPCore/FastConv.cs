
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vector = AI.DataStructs.Algebraic.Vector;


namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Реализация быстрых сверток
    /// </summary>
    [Serializable]
    public static class FastConv
    {
        /// <summary>
        /// Быстрая свертка
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="ht">Импульсивный ответ</param>
        /// <returns></returns>
        public static Vector FastConvolution(Vector signal, Vector ht)
        {
            int nMax = signal.Count + ht.Count - 1;
            FFT furie = new FFT(nMax);

            ComplexVector cs = furie.CalcFFT(signal)
                * furie.CalcFFT(ht);


            Vector outp = furie.RealIFFT(cs).CutAndZero(nMax);
            return outp;
        }

        /// <summary>
        /// Быстрая свертка
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="ht">Импульсивный ответ</param>
        /// <param name="fd">Частота дискретизации</param>
        /// <returns></returns>
        public static Vector FastConvolution(Vector signal, Vector ht, double fd)
        {
            int nMax = signal.Count + ht.Count - 1;
            FFT furie = new FFT(nMax);

            ComplexVector cs = furie.CalcFFT(signal)
                * furie.CalcFFT(ht);

            Vector outp = furie.RealIFFT(cs).CutAndZero(nMax);

            double dt = 1 / fd;

            return outp * dt;
        }

        /// <summary>
        /// Быстрая нормированная свертка
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="ht">Импульсивный ответ</param>
        /// <returns></returns>
        public static Vector FastConvolutionNorm(Vector signal, Vector ht)
        {

            Vector outp = FastConvolution(signal, ht);

            double e1 = AnalyticGeometryFunctions.NormVect(signal);
            double e2 = AnalyticGeometryFunctions.NormVect(ht);
            outp /= e1 * e2;

            return outp;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe double Expend(double[] vector)
        {
            double summ = 0;
            int size = vector.Length;

            fixed (double* pV = vector)
            {

                for (int i = 0; i < size; i++)
                    summ += pV[i];
                return summ / size;
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe double Dp(double[] vector)
        {
            double summ = 0;
            int size = vector.Length;

            fixed (double* pV = vector)
            {

                for (int i = 0; i < size; i++)
                    summ += pV[i] * pV[i];
                return summ;
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe double[] Fastdif(double[] a, double b)
        {
            int size = a.Length;
            double[] dbs = new double[size];

            fixed (double* resPointer = dbs)
            {
                fixed (double* pointerA = a)
                {
                    for (int i = 0; i < size; i++)
                        resPointer[i] = pointerA[i] - b;
                }
            }

            return dbs;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void fastdiv(double[] a, double b)
        {
            int size = a.Length;
            fixed (double* pointerA = a)
            {
                for (int i = 0; i < size; i++)
                    pointerA[i] = pointerA[i] / b;
            }


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe Complex[] fastMult(Complex[] a, Complex[] b)
        {
            int size = a.Length;
            Complex[] dbs = new Complex[size];

            fixed (Complex* resPointer = dbs)
            {
                fixed (Complex* pointerA = a)
                {
                    fixed (Complex* pointerB = b)
                    {
                        for (int i = 0; i < size; i++)
                            resPointer[i] = pointerA[i] * pointerB[i];
                    }
                }
            }

            return dbs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe double[] fastRevers(double[] inp)
        {
            int size = inp.Length;
            double[] dbs = new double[size];

            fixed (double* resPointer = dbs)
            {
                fixed (double* pointer = inp)
                {
                    for (int i = 0; i < size; i++)
                        resPointer[i] = pointer[size - 1 - i];
                }
            }

            return dbs;
        }



        /// <summary>
        /// Быстрая корреляция
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <returns></returns>
        public static double[] FastCorrelation(double[] signal)
        {
            int nMax = (2 * signal.Length) - 1;

            double[] signal2 = Fastdif(signal, Expend(signal));

            double[] res = FFT.CalcIFFTReal(
            fastMult(
                FFT.CalcFFT(signal2, nMax),
                FFT.CalcFFT(fastRevers(signal2), nMax)), nMax
            );

            double d = Dp(signal2);
            fastdiv(res, d);

            return res;

        }

        /// <summary>
        /// Быстрый алгоритм расчета нормированной ВКФ
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static Vector FastNormCrossCorrelation(Vector signal, Vector ht)
        {
            int nMax = signal.Count + ht.Count - 1;
            FFT furie = new FFT(nMax);
            Vector signal2 = signal - signal.Mean();
            signal2 = signal2.Shift(1);
            Vector ht2 = ht.Revers() - ht.Mean();

            ComplexVector cs = furie.CalcFFT(signal2)
               * furie.CalcFFT(ht2);

            double e1 = AnalyticGeometryFunctions.NormVect(signal2);
            double e2 = AnalyticGeometryFunctions.NormVect(ht2);

            Vector outp = furie.RealIFFT(cs).CutAndZero(nMax);

            outp /= e1 * e2;
            return outp;
        }





        /// <summary>
        /// Быстрая нормированная корреляция
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="ht">Патерн</param>
        /// <returns></returns>
        public static Vector FastCorrelation(Vector signal, Vector ht)
        {
            int nMax = signal.Count + ht.Count - 1;
            FFT furie = new FFT(nMax);

            ComplexVector cs = furie.CalcFFT(signal.Revers() - signal.Mean())
                * furie.CalcFFT(ht - ht.Mean());

            Vector outp = furie.RealIFFT(cs).CutAndZero(nMax);

            return outp;
        }

        /// <summary>
        /// Быстрая секционная свертка
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="pattern">Паттерн (участок в разы меньше сигнала)</param>
        /// <param name="num">Число секций</param>
        public static Vector SectionalConvolutionNorm(Vector pattern, Vector signal, int num = 2)
        {
            Vector sig = signal.CutAndZero(signal.Count + 4);

            int w = signal.Count / num;

            Vector[] data = Vector.GetWindows(sig, w, w);

            _ = Parallel.For(0, data.Length, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
            {
                data[i] = FastConvolution(data[i], pattern);
            });


            double e1 = AnalyticGeometryFunctions.NormVect(signal);
            double e2 = AnalyticGeometryFunctions.NormVect(pattern);

            Vector outp = Vector.SummWithCollision(data, pattern.Count / 2);

            outp /= e1 * e2;

            return outp;
        }

        /// <summary>
        /// Быстрая секционная свертка
        /// </summary>
        /// <param name="signal">Сигнал</param>
        /// <param name="pattern">Паттерн (участок в разы меньше сигнала)</param>
        /// <param name="num">Число секций</param>
        public static Vector SectionalConvolution(Vector pattern, Vector signal, int num = 2)
        {
            Vector sig = signal.CutAndZero(signal.Count + 4);

            int w = signal.Count / num;

            Vector[] data = Vector.GetWindows(sig, w, w);



            _ = Parallel.For(0, data.Length, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, i =>
            {
                data[i] = FastConvolution(data[i], pattern);
            });


            Vector outp = Vector.SummWithCollision(data, pattern.Count / 2);

            return outp;
        }
    }
}
