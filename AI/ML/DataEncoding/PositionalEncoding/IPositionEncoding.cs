using AI.DataStructs.Algebraic;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Position encoder
    /// </summary>
    public interface IPositionEncoding
    {
        /// <summary>
        /// Output vector dimension
        /// </summary>
        int Dim { get; set; }
        /// <summary>
        /// Getting the vector position code
        /// </summary>
        Vector GetCode(int position);
        /// <summary>
        /// Getting the vector position code
        /// </summary>
        Vector GetCode(double position);
        
    }
}
