using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Tokenizers.MatrixTokenizer
{
    /// <summary>
    /// Не реализовано
    /// </summary>
    [Serializable]
    public class SimpleMatrixTokenizer : ITokenizer<Matrix>
    {
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int UnknowToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int PadToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int StartToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int EndToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int MaxSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public Matrix[] Decode(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public Matrix[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public Matrix[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public Matrix DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int[] Encode(IEnumerable<Matrix> data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int[] Encode(Matrix data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int[,] EncodeBatch(IEnumerable<IEnumerable<Matrix>> data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Не реализовано
        /// </summary>
        public int[,] EncodeBatch(IEnumerable<Matrix> data)
        {
            throw new NotImplementedException();
        }
    }
}
