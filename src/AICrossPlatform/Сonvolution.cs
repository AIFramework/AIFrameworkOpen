using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using System;
using System.Numerics;
using Vector = AI.DataStructs.Algebraic.Vector;


namespace AI
{
    /// <summary>
    /// A class that implements the folding of sequences
    /// </summary>
    [Serializable]
    public static class Convolution
    {
        /// <summary>
        /// Свертка
        /// </summary>
        /// <param name="signal">Signal</param>
        /// <param name="Ht">Impulse response</param>
        public static Vector DirectConvolution(Vector signal, Vector Ht)
        {
            Vector ht = Ht.Revers();
            int nMax = signal.Count + ht.Count - 1;
            Vector st = StWithHt(signal, ht.Count);
            Vector outp = new Vector(nMax);


            for (int i = 0; i < nMax; i++)
            {
                for (int j = 0; j < ht.Count; j++)
                {
                    outp[i] += st[i + j] * ht[j];
                }
            }

            return outp;
        }
        /// <summary>
        /// Свертка
        /// </summary>
        /// <param name="signal">Signal</param>
        /// <param name="Ht">Impulse response</param>
        public static Vector ConvolutionNormal(Vector signal, Vector Ht)
        {
            Vector ht = Ht.Revers();
            int nMax = signal.Count + ht.Count - 1;
            Vector st = StWithHt(signal, ht.Count);
            Vector outp = new Vector(nMax);


            for (int i = 0; i < nMax; i++)
            {
                for (int j = 0; j < ht.Count; j++)
                {
                    outp[i] += st[i + j] * ht[j];
                }
            }

            double e1 = AnalyticGeometryFunctions.NormVect(signal);
            double e2 = AnalyticGeometryFunctions.NormVect(ht);

            return outp / (e1 * e2);
        }
        /// <summary>
        /// Свертка
        /// </summary>
        /// <param name="signal">Signal</param>
        /// <param name="ht">Impulse response</param>
        /// <param name="fd">Частота дискретизации</param>
    	public static Vector DirectConvolution(Vector signal, Vector ht, double fd)
        {
            return DirectConvolution(signal, ht) / fd;
        }
        /// <summary>
        /// Creating a new signal reference vector
        /// </summary>
        /// <param name="st">Signal</param>
        /// <param name="htLen">Impulse response length</param>
        public static Vector StWithHt(Vector st, int htLen)
        {
            int nMax = st.Count + (2 * htLen);
            double[] stN = new double[nMax];

            for (int j = htLen, max = j + st.Count, i = 0; j < max; j++)
            {
                stN[j] = st[i++];
            }

            return new Vector(stN);
        }
        /// <summary>
        /// Creating a new signal reference vector
        /// </summary>
        /// <param name="st">Signal</param>
        /// <param name="htLen">Impulse response length</param>
        public static ComplexVector StWithHt(ComplexVector st, int htLen)
        {
            int nMax = st.Count + (2 * htLen);
            Complex[] stN = new Complex[nMax];

            for (int j = htLen, max = j + st.Count, i = 0; j < max; j++)
            {
                stN[j] = st[i++];
            }

            return new ComplexVector(stN);
        }
        /// <summary>
        /// Direct convolution complex vector
        /// </summary>
        public static ComplexVector DirectConvolution(ComplexVector A, ComplexVector B)
        {
            int nMax = A.Count + B.Count - 1;
            ComplexVector st, ht;
            Complex[] convolut = new Complex[nMax];


            for (int n = 0; n < nMax; n++)
            {
                st = A.CutAndZero(n);
                ht = B.CutAndZero(n);
                ht.ComplexConjugateSelf();
                convolut[n] = Functions.Summ(st * ht);
            }
            return new ComplexVector(convolut);
        }
    }
}