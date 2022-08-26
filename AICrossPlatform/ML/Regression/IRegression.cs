using AI.DataStructs.Algebraic;

namespace AI.ML.Regression
{
    /// <summary>
    /// Regression interface
    /// </summary>
    public interface IRegression
    {
        /// <summary>
        /// Regression training
        /// </summary>
        /// <param name="data">Set of vector inputs</param>
        /// <param name="targets">Output vector</param>
        void Train(Vector[] data, Vector targets);
        /// <summary>
        /// Model prediction
        /// </summary>
        /// <param name="data">Input data vector</param>
        double Predict(Vector data);
    }
}
