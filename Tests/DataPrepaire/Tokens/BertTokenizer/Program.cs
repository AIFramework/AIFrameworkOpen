using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using AI.ONNX.NLP.Bert;

var bertPath = "all-MiniLM-L6-v2.onnx";
BertTokenizer tokenizer = new BertTokenizer("vocab.txt");
BertInfer bertInfer = new BertInfer(bertPath);

var encoded1 = tokenizer.Encode2Struct("kittens love milk");
Vector vects1 = bertInfer.Forward(encoded1.InputIds, encoded1.AttentionMask, encoded1.TypeIds)[1];

var encoded2 = tokenizer.Encode2Struct("cats love milk");
Vector vects2 = bertInfer.Forward(encoded2.InputIds, encoded2.AttentionMask, encoded2.TypeIds)[1];

var encoded3 = tokenizer.Encode2Struct("Colab is a hosted Jupyter Notebook service that requires no configuration and provides free access to compute resources");
Vector vects3 = bertInfer.Forward(encoded3.InputIds, encoded3.AttentionMask, encoded3.TypeIds)[1];


Console.WriteLine(vects1.Cos(vects2));
Console.WriteLine(vects2.Cos(vects3));
Console.WriteLine(vects1.Cos(vects3));