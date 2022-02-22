using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;
using System;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.TrustRegion.Subproblems
{
    internal class NewtonCGSubproblem : ITrustRegionSubproblem
    {
        public VectorMathNet<double> Pstep { get; private set; }

        public bool HitBoundary { get; private set; }

        public void Solve(IObjectiveModel objective, double delta)
        {
            VectorMathNet<double> Gradient = objective.Gradient;
            MatrixMathNet<double> Hessian = objective.Hessian;

            // define tolerance
            double gnorm = Gradient.L2Norm();
            double tolerance = Math.Min(0.5, Math.Sqrt(gnorm)) * gnorm;

            // initialize internal variables
            VectorMathNet<double> z = VectorMathNet<double>.Build.Dense(Hessian.RowCount);
            VectorMathNet<double> r = Gradient;
            VectorMathNet<double> d = -r;

            while (true)
            {
                VectorMathNet<double> Bd = Hessian * d;
                double dBd = d.DotProduct(Bd);

                if (dBd <= 0)
                {
                    (double, double) t = Util.FindBeta(1, z, d, delta);
                    Pstep = z + t.Item1 * d;
                    HitBoundary = true;
                    return;
                }

                double r_sq = r.DotProduct(r);
                double alpha = r_sq / dBd;
                VectorMathNet<double> znext = z + alpha * d;
                if (znext.L2Norm() >= delta)
                {
                    (double, double) t = Util.FindBeta(1, z, d, delta);
                    Pstep = z + t.Item2 * d;
                    HitBoundary = true;
                    return;
                }

                VectorMathNet<double> rnext = r + alpha * Bd;
                double rnext_sq = rnext.DotProduct(rnext);
                if (Math.Sqrt(rnext_sq) < tolerance)
                {
                    Pstep = znext;
                    HitBoundary = false;
                    return;
                }

                z = znext;
                r = rnext;
                d = -rnext + rnext_sq / r_sq * d;
            }
        }
    }
}
