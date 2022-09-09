using AI.BackEnds.DSP.NWaves.Operations;
using AI.BackEnds.DSP.NWaves.Operations.Convolution;
using AI.BackEnds.DSP.NWaves.Signals;
using AI.BackEnds.DSP.NWaves.Transforms;
using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AI.BackEnds.DSP.NWaves.Filters.Base
{
    /// <summary>
    /// Класс, предоставляющий методы, связанные с передаточной функцией фильтра LTI
    /// </summary>
    [Serializable]
    public class TransferFunction
    {
        /// <summary>
        /// Числитель передаточной функции
        /// </summary>
        public double[] Numerator { get; protected set; }

        /// <summary>
        /// Знаменатель передаточной функции
        /// </summary>
        public double[] Denominator { get; protected set; }

        /// <summary>
        /// Максимальное количество итераций для вычисления нулей/полюсов (корней многочленов): 25000 по умолчанию
        /// </summary>
        public int CalculateZpIterations { get; set; } = MathUtils.PolyRootsIterations;

        /// <summary>
        /// Нули передаточной функции
        /// </summary>
        protected ComplexDiscreteSignal _zeros;
        /// <summary>
        /// Нули передаточной функции
        /// </summary>
        public ComplexDiscreteSignal Zeros => _zeros ?? TfToZp(Numerator, CalculateZpIterations);

        /// <summary>
        /// Полюса передаточной функции
        /// </summary>
        protected ComplexDiscreteSignal _poles;
        /// <summary>
        /// Полюса передаточной функции
        /// </summary>
        public ComplexDiscreteSignal Poles => _poles ?? TfToZp(Denominator, CalculateZpIterations);

        /// <summary>
        /// Коэффициент усиления ('k' в 'zpk' нотации)
        /// </summary>
        public double Gain => Numerator[0];

        /// <summary>
        /// Передаточная функция вида numerator/denominator
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public TransferFunction(double[] numerator, double[] denominator = null)
        {
            Numerator = numerator;
            Denominator = denominator ?? new[] { 1.0 };
        }

        /// <summary>
        /// Передаточная функция вида zeros/poles
        /// </summary>
        /// <param name="zeros">Zeros</param>
        /// <param name="poles">Poles</param>
        /// <param name="gain">Gain</param>
        public TransferFunction(ComplexDiscreteSignal zeros, ComplexDiscreteSignal poles, double gain = 1)
        {
            _zeros = zeros;
            _poles = poles;

            Denominator = poles != null ? ZpToTf(poles) : new[] { 1.0 };
            Numerator = zeros != null ? ZpToTf(zeros) : new[] { 1.0 };

            for (int i = 0; i < Numerator.Length; i++)
            {
                Numerator[i] *= gain;
            }
        }

        /// <summary>
        /// TF constructor from state space
        /// </summary>
        /// <param name="stateSpace"></param>
        public TransferFunction(StateSpace stateSpace)
        {
            double[][] a = stateSpace.A;

            Denominator = new double[a.Length + 1];
            Denominator[0] = 1;
            for (int i = 1; i < Denominator.Length; i++)
            {
                Denominator[i] = -a[0][i - 1];
            }

            double[] c = stateSpace.C;
            double[] d = stateSpace.D;

            double[] num = new double[a.Length + 1];

            for (int i = 0; i < a.Length; i++)
            {
                num[i + 1] = -(a[0][i] - c[i]) + ((d[0] - 1) * Denominator[i + 1]);
            }

            const double ZeroTolerance = 1e-8;

            int index = 0;
            for (int i = 1; i < num.Length; i++)
            {
                if (Math.Abs(num[i]) > ZeroTolerance)
                {
                    index = i;
                    break;
                }
            }

            if (Math.Abs(d[0]) > ZeroTolerance)
            {
                index--;
            }

            Numerator = num.FastCopyFragment(num.Length - index, index);

            if (Math.Abs(d[0]) > ZeroTolerance)
            {
                Numerator[0] = d[0];
            }
        }

        /// <summary>
        /// Get state-space representation
        /// </summary>
        /// <returns></returns>
        public StateSpace StateSpace
        {
            get
            {
                int M = Numerator.Length;
                int K = Denominator.Length;

                if (M > K)
                {
                    throw new ArgumentException("Numerator size must not exceed denominator size");
                }

                double a0 = Denominator[0];    // normalize: all further results will be divided by a0

                if (K == 1)
                {
                    return new StateSpace
                    {
                        A = new MatrixNWaves(1).As2dArray(),
                        B = new double[M],
                        C = new double[M],
                        D = new double[1] { Numerator[0] / a0 }
                    };
                }

                double[] num = Numerator;

                if (M < K)
                {
                    num = new double[K];
                    Numerator.FastCopyTo(num, M, 0, K - M);
                }

                MatrixNWaves a = new MatrixNWaves(K - 1);
                for (int i = 0; i < K - 1; i++)
                {
                    a[0][i] = -Denominator[i + 1] / a0;
                }
                for (int i = 1; i < K - 1; i++)
                {
                    a[i][i - 1] = 1;
                }

                double[] b = new double[K - 1];
                b[0] = 1;

                double[] c = new double[K - 1];
                for (int i = 0; i < K - 1; i++)
                {
                    c[i] = (num[i + 1] - (num[0] * Denominator[i + 1] / a0)) / a0;
                }

                double[] d = new double[1] { num[0] / a0 };

                return new StateSpace
                {
                    A = a.As2dArray(),
                    B = b,
                    C = c,
                    D = d
                };
            }
        }

        /// <summary>
        /// Initial state 'zi' for filtering that corresponds to the steady state of the step response
        /// </summary>
        /// <returns>Initial state</returns>
        public double[] Zi
        {
            get
            {
                int size = Math.Max(Numerator.Length, Denominator.Length);

                double[] a = Denominator.PadZeros(size);
                double[] b = Numerator.PadZeros(size);

                double a0 = a[0];

                for (int i = 0; i < a.Length; a[i++] /= a0)
                {
                    ;
                }

                for (int i = 0; i < b.Length; b[i++] /= a0)
                {
                    ;
                }

                double[] B = new double[size - 1];

                for (int i = 1; i < size; i++)
                {
                    B[i - 1] = b[i] - (a[i] * b[0]);
                }

                MatrixNWaves m = MatrixNWaves.Eye(size - 1) - MatrixNWaves.Companion(a).T;

                double sum = 0.0;

                for (int i = 0; i < size - 1; i++)
                {
                    sum += m[i][0];
                }

                double[] zi = new double[size];

                zi[0] = B.Sum() / sum;

                double asum = 1.0;
                double csum = 0.0;
                for (int i = 1; i < size - 1; i++)
                {
                    asum += a[i];
                    csum += b[i] - (a[i] * b[0]);
                    zi[i] = (asum * zi[0]) - csum;
                }

                return zi;
            }
        }


        /// <summary>
        /// Evaluate impulse response
        /// </summary>
        /// <param name="length">Ignored for FIR filters (where IR is full copy of numerator)</param>
        /// <returns></returns>
        public double[] ImpulseResponse(int length = 512)
        {
            if (Denominator.Length == 1)
            {
                return Numerator.FastCopy();
            }

            double[] b = Numerator;
            double[] a = Denominator;

            double[] response = new double[length];

            for (int n = 0; n < response.Length; n++)
            {
                if (n < b.Length)
                {
                    response[n] = b[n];
                }

                for (int m = 1; m < a.Length; m++)
                {
                    if (n >= m)
                    {
                        response[n] -= a[m] * response[n - m];
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Evaluate frequency response
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public ComplexDiscreteSignal FrequencyResponse(int length = 512)
        {
            double[] ir = ImpulseResponse(length);

            double[] real = ir.Length == length ? ir :
                       ir.Length < length ? ir.PadZeros(length) :
                                             ir.FastCopyFragment(length);
            double[] imag = new double[length];

            Fft64 fft = new Fft64(length);
            fft.Direct(real, imag);

            return new ComplexDiscreteSignal(1, real.Take((length / 2) + 1),
                                                imag.Take((length / 2) + 1));
        }

        /// <summary>
        /// Group delay calculated from TF coefficients
        /// </summary>
        public double[] GroupDelay(int length = 512)
        {
            double[] cc = new ComplexConvolver()
                            .CrossCorrelate(new ComplexDiscreteSignal(1, Numerator),
                                            new ComplexDiscreteSignal(1, Denominator)).Real;

            double[] cr = Enumerable.Range(0, cc.Length)
                               .Zip(cc, (r, c) => r * c)
                               .Reverse()
                               .ToArray();

            cc = cc.Reverse().ToArray();    // reverse cc and cr (above) for EvaluatePolynomial()

            double step = Math.PI / length;
            double omega = 0.0;

            int dn = Denominator.Length - 1;

            double[] gd = new double[length];

            for (int i = 0; i < gd.Length; i++)
            {
                Complex z = Complex.FromPolarCoordinates(1, -omega);
                Complex num = MathUtils.EvaluatePolynomial(cr, z);
                Complex den = MathUtils.EvaluatePolynomial(cc, z);

                gd[i] = Complex.Abs(den) < 1e-30 ? 0 : (num / den).Real - dn;

                omega += step;
            }

            return gd;
        }

        /// <summary>
        /// Phase delay calculated from TF coefficients
        /// </summary>
        public double[] PhaseDelay(int length = 512)
        {
            double[] gd = GroupDelay(length);

            double[] pd = new double[gd.Length];
            double acc = 0.0;
            for (int i = 0; i < pd.Length; i++)     // integrate group delay
            {
                acc += gd[i];
                pd[i] = acc / (i + 1);
            }

            return pd;
        }

        /// <summary>
        /// Normalize frequency response at given frequency
        /// (normalize coefficients to map frequency response onto [0, 1])
        /// </summary>
        /// <param name="freq"></param>
        public void NormalizeAt(double freq)
        {
            Complex w = Complex.FromPolarCoordinates(1, freq);

            double gain = Complex.Abs(MathUtils.EvaluatePolynomial(Denominator, w) /
                                   MathUtils.EvaluatePolynomial(Numerator, w));

            for (int i = 0; i < Numerator.Length; i++)
            {
                Numerator[i] *= gain;
            }
        }

        /// <summary>
        /// Normalize numerator and denominator by Denominator[0]
        /// </summary>
        public void Normalize()
        {
            double a0 = Denominator[0];

            if (Math.Abs(a0) < 1e-10)
            {
                throw new ArgumentException("The first denominator coefficient can not be zero!");
            }

            for (int i = 0; i < Denominator.Length; i++)
            {
                Denominator[i] /= a0;
            }

            for (int i = 0; i < Numerator.Length; i++)
            {
                Numerator[i] /= a0;
            }
        }

        /// <summary>
        /// Method for converting zeros(poles) to TF numerator(denominator)
        /// </summary>
        /// <param name="zp"></param>
        /// <returns></returns>
        public static double[] ZpToTf(ComplexDiscreteSignal zp)
        {
            Complex[] poly = new Complex[] { 1, new Complex(-zp.Real[0], -zp.Imag[0]) };

            for (int k = 1; k < zp.Length; k++)
            {
                Complex[] poly1 = new Complex[] { 1, new Complex(-zp.Real[k], -zp.Imag[k]) };
                poly = MathUtils.MultiplyPolynomials(poly, poly1);
            }

            return poly.Select(p => p.Real).ToArray();
        }

        /// <summary>
        /// Method for converting zeros(poles) to TF numerator(denominator).
        /// Zeros and poles are given as double arrays of real and imaginary parts of zeros(poles).
        /// </summary>
        /// <param name="re"></param>
        /// <param name="im"></param>
        /// <returns></returns>
        public static double[] ZpToTf(double[] re, double[] im = null)
        {
            return ZpToTf(new ComplexDiscreteSignal(1, re, im));
        }

        /// <summary>
        /// Method for converting TF numerator(denominator) to zeros(poles)
        /// </summary>
        /// <param name="tf"></param>
        /// <returns></returns>
        public static ComplexDiscreteSignal TfToZp(double[] tf, int maxIterations = MathUtils.PolyRootsIterations)
        {
            if (tf.Length <= 1)
            {
                return null;
            }

            Complex[] roots = MathUtils.PolynomialRoots(tf, maxIterations);

            return new ComplexDiscreteSignal(1, roots.Select(r => r.Real),
                                                roots.Select(r => r.Imaginary));
        }

        /// <summary>
        /// Sequential connection
        /// </summary>
        /// <param name="tf1"></param>
        /// <param name="tf2"></param>
        /// <returns></returns>
        public static TransferFunction operator *(TransferFunction tf1, TransferFunction tf2)
        {
            double[] num = Operation.Convolve(tf1.Numerator, tf2.Numerator);
            double[] den = Operation.Convolve(tf1.Denominator, tf2.Denominator);

            return new TransferFunction(num, den);
        }

        /// <summary>
        /// Parallel connection
        /// </summary>
        /// <param name="tf1"></param>
        /// <param name="tf2"></param>
        /// <returns></returns>
        public static TransferFunction operator +(TransferFunction tf1, TransferFunction tf2)
        {
            double[] num1 = Operation.Convolve(tf1.Numerator, tf2.Denominator);
            double[] num2 = Operation.Convolve(tf2.Numerator, tf1.Denominator);

            double[] num = num1;
            double[] add = num2;

            if (num1.Length < num2.Length)
            {
                num = num2;
                add = num1;
            }

            for (int i = 0; i < add.Length; i++)
            {
                num[i] += add[i];
            }

            double[] den = Operation.Convolve(tf1.Denominator, tf2.Denominator);

            return new TransferFunction(num, den);
        }

        /// <summary>
        /// Load TF numerator and denominator from csv file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        public static TransferFunction FromCsv(Stream stream, char delimiter = ',')
        {
            using StreamReader reader = new StreamReader(stream);
            string content = reader.ReadLine();
            double[] numerator = content.Split(delimiter)
                                   .Select(s => double.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture))
                                   .ToArray();

            content = reader.ReadLine();
            double[] denominator = content.Split(delimiter)
                                     .Select(s => double.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture))
                                     .ToArray();

            return new TransferFunction(numerator, denominator);
        }

        /// <summary>
        /// Serialize TF numerator and denominator to csv file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        public void ToCsv(Stream stream, char delimiter = ',')
        {
            using StreamWriter writer = new StreamWriter(stream);
            string content = string.Join(delimiter.ToString(), Numerator.Select(k => k.ToString(CultureInfo.InvariantCulture)));
            writer.WriteLine(content);

            content = string.Join(delimiter.ToString(), Denominator.Select(k => k.ToString(CultureInfo.InvariantCulture)));
            writer.WriteLine(content);
        }
    }
}
