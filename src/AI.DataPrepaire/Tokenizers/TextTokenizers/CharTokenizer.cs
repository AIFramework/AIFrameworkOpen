using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    [Serializable]
    public class CharTokenizer : TokenizerBase<string>
    {
        public CharTokenizer(string[] decoder, Dictionary<string, int> encoder) : base(decoder, encoder)
        {
        }
    }
}
