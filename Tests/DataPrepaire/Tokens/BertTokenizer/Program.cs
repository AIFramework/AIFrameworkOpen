using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using AI.ONNX.NLP.Bert;

var bertPath = "all-MiniLM-L6-v2.onnx";
BertTokenizer tokenizer = new BertTokenizer("vocab.txt");
BertInfer model = new BertInfer(bertPath);

BertEmbedder embedder = new BertEmbedder(tokenizer, model);

Vector vects1 = embedder.ForwardAsSbert("kittens love milk");
Vector vects2 = embedder.ForwardAsSbert("Visual Studio Code is a code editor redefined and optimized for building and debugging modern web and cloud applications");
Vector vects3 = embedder.ForwardAsSbert("Is free and available on your favorite platform - Linux, macOS, and Windows. Download Visual Studio Code to experience a redefined code");


var data = embedder.ForwardBert("kittens love milk");

Console.WriteLine(vects1.Cos(vects2));
Console.WriteLine(vects2.Cos(vects3));
Console.WriteLine(vects1.Cos(vects3));