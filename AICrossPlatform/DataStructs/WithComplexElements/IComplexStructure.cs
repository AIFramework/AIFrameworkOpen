using AI.DataStructs.Shapes;
using System.Numerics;

namespace AI.DataStructs.WithComplexElements
{
    /// <summary>
    /// Complex structure interface
    /// </summary>
    public interface IComplexStructure
    {
        /// <summary>
        /// Structure data as 1D array
        /// </summary>
        Complex[] Data { get; }
        /// <summary>
        /// Structure shape
        /// </summary>
        Shape Shape { get; }
    }
}