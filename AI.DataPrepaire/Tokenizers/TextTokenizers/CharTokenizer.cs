using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
