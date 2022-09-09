using AI.DataStructs.Algebraic;
using AI.Dog.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Tokenizers.VectorTokenizer
{
    /// <summary>
    /// Токенизация векторов
    /// </summary>
    [Serializable]
    public class SimpleVectorTokenizer : ITokenizer<Vector>
    {
        public int UnknowToken { get; set; } = -1;
        public int PadToken { get; set; } = -2;
        public int StartToken { get; set; } = -3;
        public int EndToken { get; set; } = -4;
        public int MaxSize { get; set; } = 1024;

        public Func<Vector, Vector> VectorTransformer;

        /// <summary>
        /// Функция формирующая бинарный код
        /// </summary>
        public Func<Vector, Vector> ActivationFunction { get; set; } = x => x.Transform(r => r < 0 ? 0 : 1);
        /// <summary>
        /// Нереализовано
        /// </summary>
        public Vector[] Decode(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Нереализовано
        /// </summary>
        public Vector[][] DecodeBatch(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Нереализовано
        /// </summary>
        public Vector[] DecodeBatchObj(IEnumerable<IEnumerable<int>> ids)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Нереализовано
        /// </summary>
        public Vector DecodeObj(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Токенизация последовательности векторов
        /// </summary>
        public virtual int[] Encode(IEnumerable<Vector> data)
        {
            Vector[] dataArr = data.ToArray();
            int[] tokens = new int[dataArr.Length];
            int len = dataArr.Length <= MaxSize ? dataArr.Length : MaxSize;

            for (int i = 0; i < len; i++)
                tokens[i] = EncodeObj(dataArr[i]);
            return tokens;
        }

        /// <summary>
        /// Нереализовано
        /// </summary>
        public virtual int[] Encode(Vector data)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Токенизация батча
        /// </summary>
        public virtual int[,] EncodeBatch(IEnumerable<IEnumerable<Vector>> data)
        {
            var dArr = data.ToArray();
            int batch_size = dArr.Length;
            int len = dArr[0].Count();

            for (int i = 1; i < dArr.Length; i++)
                if(len < dArr[i].Count())  len = dArr[i].Count();

            len = len <= MaxSize ? len : MaxSize;

            int[,] batch_tokens = new int[batch_size, len];

            for (int batch_count = 0; batch_count < batch_size; batch_count++)
            {
                int[] tokens = Encode(dArr[batch_count]);

                for (int token_ids = 0; token_ids < tokens.Length; token_ids++)
                {
                    batch_tokens[batch_count, token_ids] = tokens[token_ids];
                }
            }

            return batch_tokens;
        }

        /// <summary>
        /// Нереализовано
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int[,] EncodeBatch(IEnumerable<Vector> data){throw new NotImplementedException();}

        /// <summary>
        /// Токенизация вектора
        /// </summary>
        /// <param name="data">Вектор</param>
        public virtual int EncodeObj(Vector data) 
        {
            var binary =  ActivationFunction(VectorTransformer(data));
            bool[] bin = new bool[binary.Count];

            for (int i = 0; i < binary.Count; i++)
                bin[i] = binary[i] > 0.5;

            return bin.GrayDecoder();
        }
    }
}
