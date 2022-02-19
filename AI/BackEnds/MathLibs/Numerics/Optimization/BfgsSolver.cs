// <copyright file="BfgsSolver.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2017 Math.NET
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

using System;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;
using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra.Double;
using AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.LineSearch;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization
{
    /// <summary>
    /// Broyden-Fletcher-Goldfarb-Shanno solver for finding function minima
    /// See http://en.wikipedia.org/wiki/Broyden%E2%80%93Fletcher%E2%80%93Goldfarb%E2%80%93Shanno_algorithm
    /// Inspired by implementation: https://github.com/PatWie/CppNumericalSolvers/blob/master/src/BfgsSolver.cpp
    /// </summary>
    public static class BfgsSolver
    {
        const double GradientTolerance = 1e-5;
        const int MaxIterations = 100000;

        /// <summary>
        /// Finds a minimum of a function by the BFGS quasi-Newton method
        /// This uses the function and it's gradient (partial derivatives in each direction) and approximates the Hessian
        /// </summary>
        /// <param name="initialGuess">An initial guess</param>
        /// <param name="functionValue">Evaluates the function at a point</param>
        /// <param name="functionGradient">Evaluates the gradient of the function at a point</param>
        /// <returns>The minimum found</returns>
        public static VectorMathNet<double> Solve(VectorMathNet initialGuess, Func<VectorMathNet<double>, double> functionValue, Func<VectorMathNet<double>, VectorMathNet<double>> functionGradient)
        {
            var objectiveFunction = ObjectiveFunction.Gradient(functionValue, functionGradient);
            objectiveFunction.EvaluateAt(initialGuess);

            int dim = initialGuess.Count;
            int iter = 0;
            // H represents the approximation of the inverse hessian matrix
            // it is updated via the Sherman–Morrison formula (http://en.wikipedia.org/wiki/Sherman%E2%80%93Morrison_formula)
            MatrixMathNet<double> H = DenseMatrix.CreateIdentity(dim);

            VectorMathNet<double> x = initialGuess;
            VectorMathNet<double> x_old = x;
            VectorMathNet<double> grad;
            WolfeLineSearch wolfeLineSearch = new WeakWolfeLineSearch(1e-4, 0.9, 1e-5, 200);
            do
            {
                // search along the direction of the gradient
                grad = objectiveFunction.Gradient;
                VectorMathNet<double> p = -1 * H * grad;
                var lineSearchResult = wolfeLineSearch.FindConformingStep(objectiveFunction, p, 1.0);
                double rate = lineSearchResult.FinalStep;
                x = x + rate * p;
                VectorMathNet<double> grad_old = grad;

                // update the gradient
                objectiveFunction.EvaluateAt(x);
                grad = objectiveFunction.Gradient;// functionGradient(x);

                VectorMathNet<double> s = x - x_old;
                VectorMathNet<double> y = grad - grad_old;

                double rho = 1.0 / (y * s);
                if (iter == 0)
                {
                    // set up an initial hessian
                    H = (y * s) / (y * y) * DenseMatrix.CreateIdentity(dim);
                }

                var sM = s.ToColumnMatrix();
                var yM = y.ToColumnMatrix();

                // Update the estimate of the hessian
                H = H
                    - rho * (sM * (yM.TransposeThisAndMultiply(H)) + (H * yM).TransposeAndMultiply(sM))
                    + rho * rho * (y.DotProduct(H * y) + 1.0 / rho) * (sM.TransposeAndMultiply(sM));
                x_old = x;
                iter++;
            }
            while ((grad.InfinityNorm() > GradientTolerance) && (iter < MaxIterations));

            return x;
        }
    }
}
