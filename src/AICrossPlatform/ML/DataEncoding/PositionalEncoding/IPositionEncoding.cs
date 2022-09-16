using AI.DataStructs.Algebraic;

namespace AI.ML.DataEncoding.PositionalEncoding
{
    /// <summary>
    /// Кодер позиций
    /// </summary>
    public interface IPositionEncoding
    {
        /// <summary>
        /// Размерность выходного вектора
        /// </summary>
        int Dim { get; set; }
        /// <summary>
        /// Код(вектор) позиции 
        /// </summary>
        Vector GetCode(int position);
        /// <summary>
        /// Код(вектор) позиции 
        /// </summary>
        Vector GetCode(double position);

    }
}
