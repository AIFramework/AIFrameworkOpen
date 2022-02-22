using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;
using System;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.TrustRegion.Subproblems
{
    internal static class Util
    {
        public static (double, double) FindBeta(double alpha, VectorMathNet<double> sd, VectorMathNet<double> gn, double delta)
        {
            // Pstep is intersection of the trust region boundary
            // Pstep = α*Psd + β*(Pgn - α*Psd)
            // find r so that ||Pstep|| = Δ
            // z = α*Psd, d = (Pgn - z)
            // (d^2)β^2 + (2*z*d)β + (z^2 - Δ^2) = 0
            //
            // positive β is used for the quadratic formula

            VectorMathNet<double> z = alpha * sd;
            VectorMathNet<double> d = gn - z;

            double a = d.DotProduct(d);
            double b = 2.0 * z.DotProduct(d);
            double c = z.DotProduct(z) - delta * delta;

            double aux = b + ((b >= 0) ? 1.0 : -1.0) * Math.Sqrt(b * b - 4.0 * a * c);
            double beta1 = -aux / 2.0 / a;
            double beta2 = -2.0 * c / aux;

            // return sorted beta
            return beta1 < beta2 ? (beta1, beta2) : (beta2, beta1);
        }
    }
}
