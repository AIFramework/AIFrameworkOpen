
using AI.DataPrepaire.NLPUtils.RegexpNLP;
using AI.DataPrepaire.NLPUtils.RegexpNLP.SimpleNER.SpecialNers;



var ner = new CombineNerProcessor();
var text = "Мой номер +8 999 666 555 4. А.В. Александров идет к И.К. Гаврилову. Сайт vkre.com/su. Почта zzszzs@mszk.com. Адрес ул. Гон, д. 56, кв. 882. Созвонимся в 22.16 или 22:39";


var result = ner.RunProcessor(text);
var result2 = ner.NerDecoder(result);

Console.WriteLine($"Замена неров, для обработки: \n{result}");
Console.WriteLine($"\n\nВосстановленный текст: \n{result2}");


SentencesTokenizer sentences = new SentencesTokenizer();
var data = sentences.Tokenize(text);

Console.WriteLine($"\n\n\nТокенизация без неров:\n");
foreach (var item in data)
    Console.WriteLine(item);


data = sentences.TokenizeWithNer(text);
Console.WriteLine($"\n\n\nТокенизация c нерами:\n");
foreach (var item in data)
    Console.WriteLine(item);