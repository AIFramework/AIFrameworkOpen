using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    public class WordTokenizer : TokenizerBase<string>
    {
        public WordTokenizer(string[] decoder, Dictionary<string, int> encoder) : base(decoder, encoder)
        {
        }
    }
}
