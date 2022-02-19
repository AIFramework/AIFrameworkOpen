using AI.DataStructs.Shapes;

namespace AI.DataStructs.Algebraic
{
    /// <summary>
    /// Algebraic structure interface
    /// </summary>
    public interface IAlgebraicStructure
    {
        /// <summary>
        /// Structure data as 1D array
        /// </summary>
        double[] Data { get; }
        /// <summary>
        /// Structure shape
        /// </summary>
        Shape Shape { get; }
    }
}