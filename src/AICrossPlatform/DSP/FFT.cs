using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using Vector = AI.DataStructs.Algebraic.Vector;
using System.Numerics;

namespace AI
{
    /// <summary>
    /// Fourier transform
    /// </summary>
    [Serializable]
    public class FFT
    {

        /// <summary>
        /// FFT Samples
        /// </summary>
        public readonly int SemplesCount;
        private readonly int HalfSemplesCount;
        private readonly Complex[] rotateCoefficiens;

        /// <summary>
        /// FFT (init rotate coef)
        /// </summary>
        public FFT(int len)
        {
            SemplesCount = Functions.NextPow2(len);
            HalfSemplesCount = SemplesCount / 2;
            rotateCoefficiens = new Complex[SemplesCount]; // [0 : HalfSemple-1] — exp(-jwt), [HalfSemple : SemplesCount-1] — exp(jwt) 

            CalcRotate(SemplesCount, HalfSemplesCount, rotateCoefficiens);
        }


        #region Core FFT

        // Поворотные множ. для обоих преобразований
        private static unsafe void CalcRotate(int semplesCount, int halfSemplesCount, Complex[] rotateCoefficiens)
        {
            fixed (Complex* rotateCoef = rotateCoefficiens)
            {
                Complex* pointerToRotateCoef = rotateCoef;

                for (int i = 0; i < halfSemplesCount; i++)
                {
                    *pointerToRotateCoef++ = Complex.Exp(-1 * 2 * Complex.ImaginaryOne * Math.PI * i / semplesCount); // exp(-jwt)
                }

                for (int i = 0; i < halfSemplesCount; i++)
                {
                    *pointerToRotateCoef++ = Complex.Exp(2 * Complex.ImaginaryOne * Math.PI * i / semplesCount); // exp(jwt)
                }
            }

        }

        // Поворотные множ. для одного типа преобразования
        private static unsafe void CalcRotate(int semplesCount, int halfSemplesCount, Complex[] rotateCoefficiens, bool canonic)
        {
            fixed (Complex* rotateCoef = rotateCoefficiens)
            {
                Complex* pointerToRotateCoef = rotateCoef;

                if (canonic)
                {
                    for (int i = 0; i < halfSemplesCount; i++)
                    {
                        *pointerToRotateCoef++ = Complex.Exp(-1 * 2 * Complex.ImaginaryOne * Math.PI * i / semplesCount); // exp(-jwt)
                    }
                }
                else
                {
                    for (int i = 0; i < halfSemplesCount; i++)
                    {
                        *pointerToRotateCoef++ = Complex.Exp(2 * Complex.ImaginaryOne * Math.PI * i / semplesCount); // exp(jwt)
                    }
                }
            }

        }

        // Вычисление бпф
        private static unsafe ComplexVector SimpleFFT(ComplexVector input, int lenght, int halfLenght, Complex[] rotateCoefficiens, bool cononic)
        {
            ComplexVector retData = input.CutAndZero(lenght);
            int offsetPointerRotate = cononic ? 0 : halfLenght;
            Complex[] complices = retData;

            fixed (Complex* ret = complices)
            {
                Complex* pRet = ret, pRetFixed = ret;
                for (int i = 1, revers = 0; i < lenght; i++, revers = 0)
                {
                    for (int j = i, movePointer = halfLenght; j > 0; j /= 2, movePointer /= 2)
                    {
                        if (j % 2 == 1)
                        {
                            revers += movePointer;
                        }
                    }

                    if (i < revers)
                    {
                        pRet = pRetFixed + revers;
                        Complex buffer = *pRet;
                        *pRet = *(pRet = pRetFixed + i); // смена адреса с присваиванием к прежднему адресу
                        *pRet = buffer;
                    }
                }

                fixed (Complex* rotateCoef = rotateCoefficiens)
                {
                    Complex* pointerToRotateCoef = rotateCoef + offsetPointerRotate, // Указатель на изменяемый элемент
                        pointerToRotateCoefFixed = rotateCoef + offsetPointerRotate; // Указатель на начало массива со смещением offset

                    for (int i = 1, maxI = halfLenght + 1; i < maxI; i += i)
                    {
                        for (int j = 0, maxJ = lenght - (i + i - 1); j < maxJ; j += i + i)
                        {
                            for (int k = j, maxK = j + i; k < maxK; k++)
                            {
                                pointerToRotateCoef = pointerToRotateCoefFixed + ((k - j) * (halfLenght / i)); // Перерасчет указателя на поворотные множители
                                Complex oddW = *(pRetFixed + k + i) * *pointerToRotateCoef;
                                pRet = pRetFixed + k;
                                Complex evenBuf = *pRet;
                                *pRet = *pRet + oddW;
                                *(pRet + i) = evenBuf - oddW;
                            }
                        }
                    }
                }
            }

            return complices;
        }

        // Для не стат. вызова
        private ComplexVector BaseFFT(ComplexVector input, bool canonic)
        {
            return SimpleFFT(input, SemplesCount, HalfSemplesCount, rotateCoefficiens, canonic);
        }

        // Статический вызов
        private static ComplexVector FftS(ComplexVector input, bool canonic)
        {
            return AISettings.FFTCore(input, canonic);
        }


        public static ComplexVector BaseStaticFFT(ComplexVector input, bool canonic)
        {
            int lenght = Functions.NextPow2(input.Count);
            int halfLenght = lenght / 2;
            Complex[] w = new Complex[halfLenght]; // только для одного типа преобразования
            CalcRotate(lenght, halfLenght, w, canonic);
            return SimpleFFT(input, lenght, halfLenght, w, true); // Начинать с 0 позиции поворотных множ.
        }

        #endregion

        /// <summary>
        /// FFT
        /// </summary>
        public ComplexVector CalcFFT(Vector inp, bool canonic = true)
        {
            ComplexVector compInp = new ComplexVector(SemplesCount);

            for (int i = 0; i < inp.Count; i++)
            {
                compInp[i] = new Complex(inp[i], 0);
            }

            return BaseFFT(compInp, canonic);
        }





        /// <summary>
        /// FFT
        /// </summary>
        public static unsafe Complex[] CalcFFT(double[] inp, int minCount = -1, bool canonic = true)
        {
            int size = Functions.NextPow2(minCount < 0 ? inp.Length : minCount);
            int sizeOld = inp.Length;
            Complex[] complex = new Complex[size];


            for (int i = 0; i < sizeOld; i++)
            {
                complex[i] = new Complex(inp[i], 0);
            }


            return FftS(new ComplexVector(complex), canonic);
        }


        /// <summary>
        /// FFT
        /// </summary>
        public static double[] CalcIFFTReal(Complex[] inp, int size = -1)
        {
            if (size < 0)
            {
                size = inp.Length;
            }

            Complex[] cs = Fft(inp, false);
            double[] dbs = new double[size];
            double oldSize = cs.Length;

            for (int i = 0; i < size; i++)
            {
                dbs[i] = (cs[i] / oldSize).Real;
            }


            return dbs;
        }


        private static Complex[] Fft(Complex[] data, bool canon)
        {
            return FftS(new ComplexVector(data), canon);
        }

        /// <summary>
        /// The real part of the Inverse FFT
        /// </summary>
        public Vector RealIFFT(ComplexVector cInp)
        {
            return IFFT(cInp).RealVector;
        }

        /// <summary>
        /// The real part of the FFT(not normalized to the number of samples)
        /// </summary>
        public Vector RealIFFT2(ComplexVector inp)
        {
            return BaseFFT(inp, false).RealVector;
        }

        /// <summary>
        /// Gives a signal spectrum from 0 to fd / 2
        /// </summary>
        /// <param name="input">Input data vector</param>
        /// <param name="window">Window function</param>
        public Vector GetSpectrum(Vector input, Func<int, Vector> window)
        {
            Vector vect = input * window(input.Count); // Применение оконной функции
            vect = this.CalcFFT(vect).MagnitudeVector / (SemplesCount / 2.0); // Амплитуды
            return vect.CutAndZero(SemplesCount / 2); // Половина вектора
        }





        /// <summary>
        /// Inverse FFT
        /// </summary>
        /// <param name="inp">Input</param>
        public ComplexVector IFFT(ComplexVector inp)
        {
            return BaseFFT(inp, false) / SemplesCount;
        }

        #region БПФ

        /// <summary>
        /// Возвращает спектр сигнала
        /// </summary>
        /// <param name="inp">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] CalcIFFT(Complex[] inp)
        {
            ComplexVector cV = new ComplexVector(inp);
            return FftS(cV, false) / cV.Count;
        }



        /// <summary>
        /// Возвращает комплексный вектор спектра сигнала
        /// </summary>
        /// <param name="inp">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static ComplexVector CalcFFT(ComplexVector inp)
        {
            return FftS(inp, true);
        }


        /// <summary>
        /// Быстрое преобразование Фурье(БПФ)
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        public static ComplexVector CalcFFT(Vector inp)
        {
            ComplexVector cv = new ComplexVector(inp);
            return CalcFFT(cv);
        }


        #endregion

        #region ОБПФ
        /// <summary>
        /// ОБПФ
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        public static ComplexVector CalcIFFT(ComplexVector inp)
        {
            return FftS(inp, false) / inp.Count;
        }


        /// <summary>
        /// ОБПФ
        /// </summary>
        /// <param name="inp">Входной вектор</param>
        public static ComplexVector CalcIFFT(Vector inp)
        {
            ComplexVector cv = new ComplexVector(inp);
            return CalcIFFT(cv);
        }
        #endregion

        /// <summary>
        /// Time-frequency conversion
        /// </summary>
        /// <param name="vect">Vector</param>
        /// <param name="lenFr">Frame size</param>
        public static Matrix TimeFrTransform(Vector vect, int lenFr = 1000)
        {
            double[] data = vect;
            int lenTime = vect.Count / lenFr;
            Vector[] vects = new Vector[lenTime];
            double[,] matr = new double[lenFr, lenTime];

            for (int i = 0; i < lenTime; i++)
            {
                vects[i] = Vector.GetIntervalDouble(i * lenFr, (i + 1) * lenFr, data);
            }

            for (int i = 0; i < lenTime; i++)
            {
                vects[i] = CalcFFT(vects[i]).MagnitudeVector / lenFr;

                for (int j = 0; j < lenFr; j++)
                {
                    matr[j, i] = vects[i][j];
                }
            }

            return new Matrix(matr);
        }


        /// <summary>
        /// Time-frequency conversion
        /// </summary>
        /// <param name="vect">Vector</param>
        /// <param name="lenFr">Frame size</param>
        public static Matrix TimeFrTransformHalf(Vector vect, int lenFr = 1024)
        {
            double[] data = vect;
            int lenTime = vect.Count / lenFr;
            Vector[] vects = new Vector[lenTime];
            double[,] matr = new double[lenFr / 2, lenTime];

            for (int i = 0; i < lenTime; i++)
            {
                vects[i] = Vector.GetIntervalDouble(i * lenFr, (i + 1) * lenFr, data);
                vects[i] *= WindowForFFT.BlackmanWindow(vects[i].Count);
            }

            for (int i = 0; i < lenTime; i++)
            {
                vects[i] = CalcFFT(vects[i]).MagnitudeVector;
                vects[i] = vects[i].CutAndZero(vects[i].Count / 2);
                vects[i] /= vects[i].Count;

                for (int j = 0; j < vects[i].Count; j++)
                {
                    matr[j, i] = vects[i][j];
                }
            }

            return new Matrix(matr);
        }

    }
}

