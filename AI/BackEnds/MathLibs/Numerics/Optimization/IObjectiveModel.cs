using AI.BackEnds.MathLibs.MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace AI.BackEnds.MathLibs.MathNet.Numerics.Optimization
{
    public interface IObjectiveModelEvaluation
    {
        IObjectiveModel CreateNew();

        /// <summary>
        /// Get the y-values of the observations.
        /// </summary>
        VectorMathNet<double> ObservedY { get; }

        /// <summary>
        /// Get the values of the weights for the observations.
        /// </summary>
        MatrixMathNet<double> Weights { get; }

        /// <summary>
        /// Get the y-values of the fitted model that correspond to the independent values.
        /// </summary>
        VectorMathNet<double> ModelValues { get; }

        /// <summary>
        /// Get the values of the parameters.
        /// </summary>
        VectorMathNet<double> Point { get; }

        /// <summary>
        /// Get the residual sum of squares.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Get the Gradient vector. G = J'(y - f(x; p))
        /// </summary>
        VectorMathNet<double> Gradient { get; }

        /// <summary>
        /// Get the approximated Hessian matrix. H = J'J
        /// </summary>
        MatrixMathNet<double> Hessian { get; }

        /// <summary>
        /// Get the number of calls to function.
        /// </summary>
        int FunctionEvaluations { get; set; }

        /// <summary>
        /// Get the number of calls to jacobian.
        /// </summary>
        int JacobianEvaluations { get; set; }

        /// <summary>
        /// Get the degree of freedom.
        /// </summary>
        int DegreeOfFreedom { get; }

        bool IsGradientSupported { get; }
        bool IsHessianSupported { get; }
    }

    public interface IObjectiveModel : IObjectiveModelEvaluation
    {
        void SetParameters(VectorMathNet<double> initialGuess, List<bool> isFixed = null);

        void EvaluateAt(VectorMathNet<double> parameters);

        IObjectiveModel Fork();

        IObjectiveFunction ToObjectiveFunction();
    }
}
