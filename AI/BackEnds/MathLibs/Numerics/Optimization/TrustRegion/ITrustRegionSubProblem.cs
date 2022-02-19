using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization.TrustRegion
{
    public interface ITrustRegionSubproblem
    {
        VectorMathNet<double> Pstep { get; }
        bool HitBoundary { get; }

        void Solve(IObjectiveModel objective, double radius);
    }
}
