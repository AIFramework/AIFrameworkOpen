using AI.DataPrepaire.Tokenizers.TextTokenizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTokenizerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WordTokenizer wordTokenizer = new WordTokenizer("cat.txt");
            string text = "коты, которые живут в Африке едят молоко";
            var ids = wordTokenizer.Encode(text);

            for (int i = 0; i < ids.Length; i++)  Console.Write(ids[i]+" ");

            Console.WriteLine("\n"+wordTokenizer.DecodeObj(ids)+"\n\n");
        }
    }
}
