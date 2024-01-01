using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using AI.ONNX.NLP.Bert;

var bertPath = "all-MiniLM-L6-v2.onnx";
BertTokenizer tokenizer = new BertTokenizer("vocab.txt");
BertInfer model = new BertInfer(bertPath);

BertEmbedder embedder = new BertEmbedder(tokenizer, model);

Vector vects1 = embedder.ForwardAsSbert("kittens love milk");
Vector vects2 = embedder.ForwardAsSbert("cats love milk");
Vector vects3 = embedder.ForwardAsSbert("Colab is a hosted Jupyter Notebook service that requires no configuration and provides free access to compute resources");


Console.WriteLine(vects1.Cos(vects2));
Console.WriteLine(vects2.Cos(vects3));
Console.WriteLine(vects1.Cos(vects3));