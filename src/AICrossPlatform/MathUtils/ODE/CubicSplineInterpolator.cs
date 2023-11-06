using System;
using AI.DataStructs.Algebraic;
using AI.MathUtils.Algebra;

namespace AI.MathUtils.ODE
{
    [Serializable]
    public class CubicSplineInterpolator
    {
        private double[] xAxis, yAxis, coefficientsA, coefficientsB, coefficientsC, coefficientsD;

        /// <summary>
        /// Initializes a new instance of the <see cref="CubicSplineInterpolator"/> class.
        /// </summary>
        /// <param name="xValues">The x-coordinates of the data points.</param>
        /// <param name="yValues">The y-coordinates of the data points.</param>
        /// <exception cref="ArgumentException">Thrown when xValues and yValues have different lengths or there are not enough data points.</exception>
        public CubicSplineInterpolator(double[] xValues, double[] yValues)
        {
            if (xValues.Length != yValues.Length)
            {
                throw new ArgumentException("xValues and yValues must have the same length.");
            }

            if (xValues.Length < 3)
            {
                throw new ArgumentException("There must be at least three data points for cubic spline interpolation.");
            }

            int numberOfIntervals = xValues.Length - 1;
            xAxis = xValues;
            yAxis = yValues;
            coefficientsA = new double[numberOfIntervals];
            coefficientsB = new double[numberOfIntervals];
            coefficientsC = new double[numberOfIntervals + 1];
            coefficientsD = new double[numberOfIntervals];

            ComputeSplineCoefficients();
        }

        /// <summary>
        /// Computes spline coefficients using the tridiagonal system.
        /// </summary>
        private void ComputeSplineCoefficients()
        {
            int n = xAxis.Length - 1;
            double[] intervalLengths = new double[n];
            for (int i = 0; i < n; i++)
            {
                intervalLengths[i] = xAxis[i + 1] - xAxis[i];
            }

            double[,] matrixA = new double[n + 1, n + 1];
            double[] vectorB = new double[n + 1];

            for (int i = 1; i < n; i++)
            {
                matrixA[i, i - 1] = intervalLengths[i - 1];
                matrixA[i, i] = 2 * (intervalLengths[i - 1] + intervalLengths[i]);
                matrixA[i, i + 1] = intervalLengths[i];
                vectorB[i] = 3 * ((yAxis[i + 1] - yAxis[i]) / intervalLengths[i] - (yAxis[i] - yAxis[i - 1]) / intervalLengths[i - 1]);
            }

            // Natural spline boundary conditions
            matrixA[0, 0] = 1;
            matrixA[n, n] = 1;
            vectorB[0] = 0;
            vectorB[n] = 0;

            coefficientsC = Gauss.SolvingEquations(new Matrix(matrixA), vectorB);

            for (int i = 0; i < n; i++)
            {
                coefficientsA[i] = yAxis[i];
                coefficientsD[i] = (coefficientsC[i + 1] - coefficientsC[i]) / (3 * intervalLengths[i]);
                coefficientsB[i] = (yAxis[i + 1] - yAxis[i]) / intervalLengths[i] - intervalLengths[i] * (coefficientsC[i + 1] + 2 * coefficientsC[i]) / 3;
            }
        }

        /// <summary>
        /// Interpolates the value at a given x-coordinate using the cubic spline.
        /// </summary>
        /// <param name="x">The x-coordinate to interpolate at.</param>
        /// <returns>The interpolated value.</returns>
        /// <exception cref="ArgumentException">Thrown when the x-coordinate to interpolate is outside the domain of the x-values.</exception>
        public double Interpolate(double x)
        {
            if (x < xAxis[0] || x > xAxis[xAxis.Length - 1])
            {
                throw new ArgumentException("The x-coordinate to interpolate is outside the domain of the x-values.");
            }

            int i = Array.BinarySearch(xAxis, x);
            if (i < 0)
            {
                i = ~i - 1;
            }

            double deltaX = x - xAxis[i];
            return coefficientsA[i] + coefficientsB[i] * deltaX + coefficientsC[i] * Math.Pow(deltaX, 2) + coefficientsD[i] * Math.Pow(deltaX, 3);
        }
    }
}
