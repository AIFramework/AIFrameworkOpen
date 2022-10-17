using System;
using System.Collections.Generic;

namespace AI.DataPrepaire.Tokenizers.TextTokenizers
{
    /// <summary>
    /// Токенизация на уровне букв
    /// </summary>
    [Serializable]
    public class CharTokenizer : TokenizerBase<string>
    {
        /// <summary>
        /// Токенизация на уровне букв
        /// </summary>
        public CharTokenizer(string[] decoder, Dictionary<string, int> encoder) : base(decoder, encoder)
        {
        }
    }
}
