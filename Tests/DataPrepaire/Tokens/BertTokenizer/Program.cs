using AI;
using AI.DataPrepaire.DataLoader.NNWBlockLoader;
using AI.DataPrepaire.NLPUtils.QA;
using AI.DataStructs.Algebraic;
using AI.ML.Classifiers;
using AI.ML.Distances;
using AI.ONNX.NLP.Bert;
using System.Diagnostics;

string path = "SbertDistilAIFr";
// Загрузка модели Bert
BertEmbedder embedder = BertEmbedder.FromPretrained(path);
// Добавление последнего(линейного) слоя
LinearLayerLoader linearLayer = LinearLayerLoader.LoadFromBinary(@$"{path}\1_Linear\model.aifr");
embedder.V2VBlocks.Add(linearLayer);


//ChatBotRetrTest(embedder);
//WTest(embedder);
SimpleTest(embedder);

static void SimpleTest(BertEmbedder embedder)
{

    for (int i = 0; i < 1000; i++)
    {
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
    }
    Console.ReadKey();
}


static void WTest(BertEmbedder embedder)
{


    // Векторизация предложений
    Vector vects1 = embedder.ForwardBlockPooling(new[] { "Kittens love milk", "and eat mouses" }, new[] { 3.0, 1.0});
    Vector vects2 = embedder.ForwardBlockPooling(new[] { "Kittens love milk", "and eat mouses" }, new[] { 1.0, 3.0});

    Vector vects1_t = embedder.ForwardSBert("Kittens love milk");
    Vector vects2_t = embedder.ForwardSBert("Kittens eat mouses");



    // Рассчет близости между текстами
    Console.WriteLine(vects1.Cos(vects1_t));
    Console.WriteLine(vects1.Cos(vects2_t));
    Console.WriteLine(vects2.Cos(vects1_t));
    Console.WriteLine(vects2.Cos(vects2_t));
    Console.ReadKey();
}


static void ChatBotRetrTest(BertEmbedder embedder)
{
    string[] answers =
    {
    "Привет!",
    "Меня зовут Крот",
    "Мне 3 года",
    "Машина это средство передвижения",
    "Полупроводниковый триод — электронный компонент из полупроводникового материала, способный небольшим входным сигналом управлять значительным током в выходной цепи, что позволяет использовать его для усиления, генерирования, коммутации и преобразования электрических сигналов.",
    "В классическом понимании поезд представляет собой движущуюся по пути цепь из соединённых между собой 'повозок' — единиц подвижного состава.",
    "С точки зрения научной систематики, домашняя кошка — млекопитающее семейства кошачьих отряда хищных. Одни исследователи рассматривают домашнюю кошку как подвид дикой кошки[5], другие — как отдельный биологический вид[6].",
    };

   string[] test =
   {
        "Сколько тебе лет?",
        "What is your name?",
        "Расскажи про автотранспорт",
        "Че про кошек скажешь?",
        "Что можешь про поезда рассказать?",
        "Здравствуй!",
        "Что такое транзистор?"
    };

    KnnBot kBot = new KnnBot(embedder.ForwardSBert);
    kBot.Train(answers, answers);




    
    foreach (var item in test)
    {
        var ans = kBot.GetAnswer(item);
        Console.WriteLine($"Вопрос: {item}\nОтвет: {ans.AnswerStr}\nУверенность в ответе: {ans.Conf}%\n\n");
    }


    Stopwatch stopwatch = new Stopwatch();

    int N = 30;

    stopwatch.Start();
    for (int i = 0; i < N; i++)
    {
        foreach (var item in test)
        {
            KnnBot.Answer ans = kBot.GetAnswer(item);
        }
    }
    stopwatch.Stop();
    double aps = 1000.0 * N * test.Length / stopwatch.ElapsedMilliseconds;

    Console.WriteLine($"Ответов в секунду: {Math.Round(aps)}");

}