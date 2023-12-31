using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;

BertTokenizer tokenizer = new BertTokenizer("vocab.txt");
var encoded = tokenizer.Encode2Struct("how are you?");

Console.WriteLine(encoded);