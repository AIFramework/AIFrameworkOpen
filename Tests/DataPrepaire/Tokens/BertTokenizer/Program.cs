using AI.DataPrepaire.DataLoader.NNWBlockLoader;
using AI.DataPrepaire.Tokenizers.TextTokenizers.HFTokenizers;
using AI.DataStructs.Algebraic;
using AI.ONNX.NLP.Bert;

var bertPath = "model.onnx";
BertTokenizer tokenizer = new BertTokenizer("vocab.txt", false);


BertInfer model = new BertInfer(bertPath);
BertEmbedder embedder = new BertEmbedder(tokenizer, model);
LinearLayerLoader linearLayer = LinearLayerLoader.LoadFromBinary("model.aifr");
embedder.V2VBlocks.Add(linearLayer);
embedder.Config.HiddenSize = 312;

Vector vects1 = embedder.ForwardSBert("Kittens love milk");
Vector vects2 = embedder.ForwardSBert("Visual Studio Code (VS Code) — текстовый редактор, разработанный Microsoft для Windows, Linux и macOS.");
Vector vects3 = embedder.ForwardSBert("Visual Studio Code, also commonly referred to as VS Code,[12] is a source-code editor developed by Microsoft for Windows, Linux and macOS.");
Vector vects1_rus = embedder.ForwardSBert("Котята любят молоко");

Console.WriteLine(vects1.Cos(vects1_rus));
Console.WriteLine(vects1.Cos(vects2));
Console.WriteLine(vects2.Cos(vects3));
Console.WriteLine(vects1.Cos(vects3));