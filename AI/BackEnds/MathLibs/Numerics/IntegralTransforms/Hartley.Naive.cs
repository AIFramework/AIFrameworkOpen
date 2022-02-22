// <copyright file="Hartley.Naive.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using AI.BackEnds.MathLibs.MathNet.Numerics.Threading;
using System;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.IntegralTransforms
{
    /// <summary>
    /// Fast (FHT) Implementation of the Discrete Hartley Transform (DHT).
    /// </summary>
    public static partial class Hartley
    {
        /// <summary>
        /// Naive generic DHT, useful e.g. to verify faster algorithms.
        /// </summary>
        /// <param name="samples">Time-space sample vector.</param>
        /// <returns>Corresponding frequency-space vector.</returns>
        internal static double[] Naive(double[] samples)
        {
            double w0 = Constants.Pi2 / samples.Length;
            double[] spectrum = new double[samples.Length];

            CommonParallel.For(0, samples.Length, (u, v) =>
                {
                    for (int i = u; i < v; i++)
                    {
                        double wk = w0 * i;
                        double sum = 0.0;
                        for (int n = 0; n < samples.Length; n++)
                        {
                            double w = n * wk;
                            sum += samples[n] * Constants.Sqrt2 * Math.Cos(w - Constants.PiOver4);
                        }

                        spectrum[i] = sum;
                    }
                });

            return spectrum;
        }
    }
}
