using AI.DataPrepaire.DataLoader.NNWBlockLoader;
using AI.DataStructs.Algebraic;
using AI.ONNX.NLP.Bert;


string path = "SbertDistilAIFr";
// Загрузка модели Bert
BertEmbedder embedder = BertEmbedder.FromPretrained(path);
// Добавление последнего(линейного) слоя
LinearLayerLoader linearLayer = LinearLayerLoader.LoadFromBinary(@$"{path}\1_Linear\model.aifr");
embedder.V2VBlocks.Add(linearLayer);

// Векторизация предложений
Vector vects1 = embedder.ForwardSBert("Kittens love milk");
Vector vects2 = embedder.ForwardSBert("Visual Studio Code (VS Code) — текстовый редактор, разработанный Microsoft для Windows, Linux и macOS.");
Vector vects3 = embedder.ForwardSBert("Visual Studio Code, also commonly referred to as VS Code,[12] is a source-code editor developed by Microsoft for Windows, Linux and macOS.");
Vector vects1_rus = embedder.ForwardSBert("Котята любят молоко");

// Рассчет близости между текстами
Console.WriteLine(vects1.Cos(vects1_rus));
Console.WriteLine(vects1.Cos(vects2));
Console.WriteLine(vects2.Cos(vects3));
Console.WriteLine(vects1.Cos(vects3));
Console.ReadKey();