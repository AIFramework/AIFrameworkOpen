using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.TrustRegion.Subproblems
{
    internal class DogLegSubproblem : ITrustRegionSubproblem
    {
        public VectorMathNet<double> Pstep { get; private set; }

        public bool HitBoundary { get; private set; }

        public void Solve(IObjectiveModel objective, double delta)
        {
            VectorMathNet<double> Gradient = objective.Gradient;
            MatrixMathNet<double> Hessian = objective.Hessian;

            // newton point, the Gauss–Newton step by solving the normal equations
            VectorMathNet<double> Pgn = -Hessian.PseudoInverse() * Gradient; // Hessian.Solve(Gradient) fails so many times...

            // cauchy point, steepest descent direction is given by
            double alpha = Gradient.DotProduct(Gradient) / (Hessian * Gradient).DotProduct(Gradient);
            VectorMathNet<double> Psd = -alpha * Gradient;

            // update step and prectted reduction
            if (Pgn.L2Norm() <= delta)
            {
                // Pgn is inside trust region radius
                HitBoundary = false;
                Pstep = Pgn;
            }
            else if (alpha * Psd.L2Norm() >= delta)
            {
                // Psd is outside trust region radius
                HitBoundary = true;
                Pstep = delta / Psd.L2Norm() * Psd;
            }
            else
            {
                // Pstep is intersection of the trust region boundary
                HitBoundary = true;
                double beta = Util.FindBeta(alpha, Psd, Pgn, delta).Item2;
                Pstep = alpha * Psd + beta * (Pgn - alpha * Psd);
            }
        }
    }
}
