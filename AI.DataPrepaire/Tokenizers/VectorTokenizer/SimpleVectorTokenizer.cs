using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Tokenizers.VectorTokenizer
{
    [Serializable]
    public class SimpleVectorTokenizer : ITokenizer<Vector>
    {
        public int UnknowToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PadToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StartToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int EndToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MaxSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Vector[] Decode(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Vector[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }

        public Vector[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }

        public Vector DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public int[] Encode(IEnumerable<Vector> data)
        {
            throw new NotImplementedException();
        }

        public int[] Encode(Vector data)
        {
            throw new NotImplementedException();
        }

        public int[,] EncodeBatch(IEnumerable<IEnumerable<Vector>> data)
        {
            throw new NotImplementedException();
        }

        public int[,] EncodeBatch(IEnumerable<Vector> data)
        {
            throw new NotImplementedException();
        }
    }
}
