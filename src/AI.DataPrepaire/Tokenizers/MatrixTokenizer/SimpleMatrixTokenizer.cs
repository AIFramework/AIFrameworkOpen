using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Tokenizers.MatrixTokenizer
{
    [Serializable]
    public class SimpleMatrixTokenizer : ITokenizer<Matrix>
    {
        public int UnknowToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PadToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StartToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int EndToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Matrix[] Decode(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Matrix[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }

        public Matrix[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }

        public Matrix DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public int[] Encode(IEnumerable<Matrix> data)
        {
            throw new NotImplementedException();
        }

        public int[] Encode(Matrix data)
        {
            throw new NotImplementedException();
        }

        public int[,] EncodeBatch(IEnumerable<IEnumerable<Matrix>> data)
        {
            throw new NotImplementedException();
        }

        public int[,] EncodeBatch(IEnumerable<Matrix> data)
        {
            throw new NotImplementedException();
        }
    }
}
