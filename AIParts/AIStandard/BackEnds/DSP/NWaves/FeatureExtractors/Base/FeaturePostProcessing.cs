using AI.BackEnds.DSP.NWaves.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.BackEnds.DSP.NWaves.FeatureExtractors.Base
{
    /// <summary>
    /// Class providing methods for additional processing of feature vector sequences.
    /// </summary>
    public static class FeaturePostProcessing
    {
        /// <summary>
        /// Method for mean subtraction (in particular, CMN).
        /// </summary>
        /// <param name="vectors">Sequence of feature vectors</param>
        public static void NormalizeMean(IList<float[]> vectors)
        {
            if (vectors.Count < 2)
            {
                return;
            }

            int featureCount = vectors[0].Length;

            for (int i = 0; i < featureCount; i++)
            {
                float mean = vectors.Average(t => t[i]);

                foreach (float[] vector in vectors)
                {
                    vector[i] -= mean;
                }
            }
        }

        /// <summary>
        /// Variance normalization (divide by unbiased estimate of stdev)
        /// </summary>
        /// <param name="vectors">Sequence of feature vectors</param>
        public static void NormalizeVariance(IList<float[]> vectors, int bias = 1)
        {
            int n = vectors.Count;

            if (n < 2)
            {
                return;
            }

            int featureCount = vectors[0].Length;

            for (int i = 0; i < featureCount; i++)
            {
                float mean = vectors.Average(t => t[i]);
                float std = vectors.Sum(t => (t[i] - mean) * (t[i] - mean) / (n - bias));

                if (std < Math.Abs(1e-30f))      // avoid dividing by zero
                {
                    std = 1;
                }

                foreach (float[] vector in vectors)
                {
                    vector[i] /= (float)Math.Sqrt(std);
                }
            }
        }

        /// <summary>
        /// Method for complementing feature vectors with 1st and (by default) 2nd order derivatives.
        /// </summary>
        /// <param name="vectors"></param>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <param name="includeDeltaDelta"></param>
        /// <param name="N"></param>
        public static void AddDeltas(IList<float[]> vectors,
                                     IList<float[]> previous = null,
                                     IList<float[]> next = null,
                                     bool includeDeltaDelta = true,
                                     int N = 2)
        {
            if (previous == null)
            {
                previous = new List<float[]>(N);
                for (int n = 0; n < N; n++)
                {
                    previous.Add(vectors[0]);
                }
            }
            if (next == null)
            {
                next = new List<float[]>(N);
                for (int n = 0; n < N; n++)
                {
                    next.Add(vectors.Last());
                }
            }

            int featureCount = vectors[0].Length;

            float[][] sequence = previous.Concat(vectors).Concat(next).ToArray();

            // deltas:

            int M = 2 * Enumerable.Range(1, N).Sum(x => x * x);  // scaling in denominator

            int newSize = includeDeltaDelta ? 3 * featureCount : 2 * featureCount;

            for (int i = N; i < sequence.Length - N; i++)
            {
                float[] f = new float[newSize];

                for (int j = 0; j < featureCount; j++)
                {
                    f[j] = vectors[i - N][j];
                }
                for (int j = 0; j < featureCount; j++)
                {
                    float num = 0.0f;
                    for (int n = 1; n <= N; n++)
                    {
                        num += n * (sequence[i + n][j] - sequence[i - n][j]);
                    }
                    f[j + featureCount] = num / M;
                }
                sequence[i] = vectors[i - N] = f;
            }

            if (!includeDeltaDelta)
            {
                return;
            }

            // delta-deltas:

            for (int n = 1; n <= N; n++)
            {
                sequence[n - 1] = vectors[0];
                sequence[sequence.Length - n] = vectors.Last();
            }

            for (int i = N; i < sequence.Length - N; i++)
            {
                for (int j = 0; j < featureCount; j++)
                {
                    float num = 0.0f;
                    for (int n = 1; n <= N; n++)
                    {
                        num += n * (sequence[i + n][j + featureCount] -
                                    sequence[i - n][j + featureCount]);
                    }
                    vectors[i - N][j + 2 * featureCount] = num / M;
                }
            }
        }

        /// <summary>
        /// Join different collections of feature vectors.
        /// Time positions must coincide.
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public static float[][] Join(params IList<float[]>[] vectors)
        {
            int vectorCount = vectors.Length;

            switch (vectorCount)
            {
                case 0:
                    throw new ArgumentException("Empty collection of feature vectors!");
                case 1:
                    return vectors.ElementAt(0).ToArray();
            }

            int totalVectors = vectors[0].Count;
            if (vectors.Any(v => v.Count != totalVectors))
            {
                throw new InvalidOperationException("All sequences of feature vectors must have the same length!");
            }

            int length = vectors.Sum(v => v[0].Length);
            float[][] joined = new float[totalVectors][];

            for (int i = 0; i < joined.Length; i++)
            {
                float[] features = new float[length];

                for (int offset = 0, j = 0; j < vectorCount; j++)
                {
                    int size = vectors[j][i].Length;
                    vectors[j][i].FastCopyTo(features, size, 0, offset);
                    offset += size;
                }

                joined[i] = features;
            }

            return joined;
        }
    }
}
