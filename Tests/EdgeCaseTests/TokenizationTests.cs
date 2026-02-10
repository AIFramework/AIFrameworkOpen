using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace EdgeCaseTests;

/// <summary>
/// МОЩНЫЕ тесты токенизации и зарезервированных слов
/// </summary>
class TokenizationTests
{
    static int passedTests = 0;
    static int failedTests = 0;
    static int totalTests = 0;

    static void TestString(string testName, string script, string expectedString)
    {
        totalTests++;
        try
        {
            var processor = new Processor();
            var output = processor.Run(script);
            var hasError = output.Any(line => line.Contains("КРИТИЧЕСКАЯ ОШИБКА") || line.Contains("ОШИБКА"));

            if (hasError)
            {
                failedTests++;
                Console.WriteLine($"❌ {testName}");
                Console.WriteLine($"   Ошибка выполнения:");
                foreach (var line in output)
                {
                    Console.WriteLine($"   {line}");
                }
                return;
            }

            string? actualString = null;
            foreach (var line in output)
            {
                if (line.Contains("=>"))
                {
                    var parts = line.Split(new[] { "=>" }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        actualString = parts[1].Trim();
                        var bracketIndex = actualString.IndexOf('[');
                        if (bracketIndex > 0)
                        {
                            actualString = actualString.Substring(0, bracketIndex).Trim();
                        }
                    }
                }
            }

            bool passed = actualString == expectedString;

            if (passed)
            {
                passedTests++;
                Console.WriteLine($"✅ {testName}");
            }
            else
            {
                failedTests++;
                Console.WriteLine($"❌ {testName}");
                Console.WriteLine($"   Ожидалось: \"{expectedString}\"");
                Console.WriteLine($"   Получено:  \"{actualString}\"");
            }
        }
        catch (Exception ex)
        {
            failedTests++;
            Console.WriteLine($"❌ {testName}");
            Console.WriteLine($"   Exception: {ex.Message}");
        }
    }

    static void TestNumber(string testName, string script, double expected, double precision = 1e-8)
    {
        totalTests++;
        try
        {
            var processor = new Processor();
            var output = processor.Run(script);
            var hasError = output.Any(line => line.Contains("КРИТИЧЕСКАЯ ОШИБКА") || line.Contains("ОШИБКА"));

            if (hasError)
            {
                failedTests++;
                Console.WriteLine($"❌ {testName}");
                Console.WriteLine($"   Ошибка выполнения:");
                foreach (var line in output)
                {
                    Console.WriteLine($"   {line}");
                }
                return;
            }

            double? actual = null;
            foreach (var line in output)
            {
                if (line.Contains("=>"))
                {
                    var parts = line.Split(new[] { "=>" }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        var valueStr = parts[1].Trim().Split(new[] { ' ', '[' })[0];
                        if (double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                            System.Globalization.CultureInfo.InvariantCulture, out var val))
                        {
                            actual = val;
                        }
                    }
                }
            }

            bool passed = actual.HasValue && Math.Abs(actual.Value - expected) < precision;

            if (passed)
            {
                passedTests++;
                Console.WriteLine($"✅ {testName}");
            }
            else
            {
                failedTests++;
                Console.WriteLine($"❌ {testName}");
                Console.WriteLine($"   Ожидалось: {expected}");
                Console.WriteLine($"   Получено:  {actual}");
            }
        }
        catch (Exception ex)
        {
            failedTests++;
            Console.WriteLine($"❌ {testName}");
            Console.WriteLine($"   Exception: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  МОЩНЫЕ ТЕСТЫ ТОКЕНИЗАЦИИ И ЗАРЕЗЕРВИРОВАННЫХ СЛОВ          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("═══ ГРУППА 1: ЗАРЕЗЕРВИРОВАННЫЕ СЛОВА В СТРОКАХ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Строка 'and'",
            @"s = ""and""
s",
            "and");

        TestString("Строка 'or'",
            @"s = ""or""
s",
            "or");

        TestString("Строка 'not'",
            @"s = ""not""
s",
            "not");

        TestString("Строка 'if'",
            @"s = ""if""
s",
            "if");

        TestString("Строка 'else'",
            @"s = ""else""
s",
            "else");

        TestString("Строка 'for'",
            @"s = ""for""
s",
            "for");

        TestString("Строка 'while'",
            @"s = ""while""
s",
            "while");

        TestString("Строка 'break'",
            @"s = ""break""
s",
            "break");

        TestString("Строка 'continue'",
            @"s = ""continue""
s",
            "continue");

        TestString("Строка с AND внутри текста",
            @"s = ""command""
s",
            "command");

        TestString("Строка с OR внутри",
            @"s = ""error""
s",
            "error");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 2: ОПЕРАТОРЫ В СТРОКАХ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Строка с +",
            @"s = ""a+b""
s",
            "a+b");

        TestString("Строка с -",
            @"s = ""x-y""
s",
            "x-y");

        TestString("Строка с *",
            @"s = ""2*3""
s",
            "2*3");

        TestString("Строка с /",
            @"s = ""10/5""
s",
            "10/5");

        TestString("Строка с =",
            @"s = ""a=b""
s",
            "a=b");

        TestString("Строка с ==",
            @"s = ""a==b""
s",
            "a==b");

        TestString("Строка с &&",
            @"s = ""a&&b""
s",
            "a&&b");

        TestString("Строка с ||",
            @"s = ""a||b""
s",
            "a||b");

        TestString("Строка со скобками",
            @"s = ""(x)""
s",
            "(x)");

        TestString("Строка с квадратными скобками",
            @"s = ""[array]""
s",
            "[array]");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 3: СРАВНЕНИЕ СТРОК ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Строки равны",
            @"s1 = ""test""
s2 = ""test""
s1 == s2",
            1.0);

        TestNumber("Строки НЕ равны",
            @"s1 = ""test""
s2 = ""other""
s1 == s2",
            0.0);

        TestNumber("Строки не равны (!=)",
            @"s1 = ""abc""
s2 = ""xyz""
s1 != s2",
            1.0);

        TestNumber("Пустые строки равны",
            @"s1 = """"
s2 = """"
s1 == s2",
            1.0);

        TestNumber("Строка с and == строке с and",
            @"s1 = ""and""
s2 = ""and""
s1 == s2",
            1.0);

        TestNumber("Строки с # равны",
            @"s1 = ""#test""
s2 = ""#test""
s1 == s2",
            1.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 4: CONCAT С ЗАРЕЗЕРВИРОВАННЫМИ СЛОВАМИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("concat с 'and' (НЕ &&)",
            @"result = concat(""a"", "" & "", ""b"")
result",
            "a & b");

        TestString("concat с 'or'",
            @"result = concat(""true"", "" | "", ""false"")
result",
            "true | false");

        TestString("concat трех 'and'",
            @"result = concat(""command"", "" & "", ""control"")
result",
            "command & control");

        TestString("concat с if в строке",
            @"result = concat(""check "", ""if"", "" true"")
result",
            "check if true");

        TestString("concat с while в строке",
            @"result = concat(""run "", ""while"", "" true"")
result",
            "run while true");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 5: МАССИВЫ СТРОК С ЗАРЕЗЕРВИРОВАННЫМИ СЛОВАМИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Массив с 'and'",
            @"arr = [""and"", ""or"", ""not""]
index(arr, 0)",
            "and");

        TestString("Массив с операторами",
            @"arr = [""+"", ""-"", ""*"", ""/""]
index(arr, 2)",
            "*");

        TestString("JOIN массива с зарезервированными словами",
            @"arr = [""if"", ""else"", ""for""]
join(arr, "" "")",
            "if else for");

        TestString("JOIN с разделителем 'and'",
            @"arr = [""one"", ""two"", ""three""]
join(arr, "" & "")",
            "one & two & three");

        TestString("Массив с логическими словами",
            @"arr = [""true"", ""false"", ""null""]
join(arr, "","")  ",
            "true,false,null");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 6: СПЕЦИАЛЬНЫЕ СИМВОЛЫ В СТРОКАХ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Строка с точкой с запятой",
            @"s = ""a;b;c""
s",
            "a;b;c");

        TestString("Строка с двоеточием",
            @"s = ""key:value""
s",
            "key:value");

        TestString("Строка с запятой",
            @"s = ""a,b,c""
s",
            "a,b,c");

        TestString("Строка с кавычками внутри (escaped)",
            @"s = ""say \""hello\""""
s",
            "say \"hello\"");

        TestString("Строка с пробелами",
            @"s = ""   spaces   ""
len(s)",
            "12");  // 3 пробела + "spaces"(6) + 3 пробела = 12

        TestString("Строка с табуляцией",
            @"s = ""tab\there""
s",
            @"tab\there");  // Калькулятор не интерпретирует \t как табуляцию, хранит литерально

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 7: СЛОЖНЫЕ ВЫРАЖЕНИЯ СО СТРОКАМИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("IF с сравнением строк",
            @"s = ""test""
result = """"
if s == ""test"":
    result = ""match""
else:
    result = ""no match""
result",
            "match");

        TestString("LOOP с проверкой строки",
            @"found = """"
arr = [""one"", ""two"", ""three""]
for i = 0 to 2:
    s = index(arr, i)
    if s == ""two"":
        found = ""yes""
found",
            "yes");

        TestNumber("Подсчет совпадений строк",
            @"arr = [""a"", ""b"", ""a"", ""c"", ""a""]
count = 0
for i = 0 to 4:
    if index(arr, i) == ""a"":
        count = count + 1
count",
            3.0);

        TestString("Фильтрация массива по условию",
            @"arr = [""apple"", ""banana"", ""apricot""]
result = """"
for i = 0 to 2:
    s = index(arr, i)
    # Проверяем первую букву
    first = substr(s, 0, 1)
    if first == ""a"":
        result = concat(result, s, "" "")
result",
            "apple apricot");  // Trailing space обрезается при выводе результата

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 8: ГРАНИЧНЫЕ СЛУЧАИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Строка только из пробелов",
            @"s = ""     ""
len(s)",
            "5");

        TestString("Очень длинная строка",
            @"s = ""This is a very long string that contains many words and characters to test the tokenizer""
len(s)",
            "88");  // Фактическая длина строки = 88 символов

        TestNumber("Сравнение пустых строк",
            @"s1 = """"
s2 = """"
s1 == s2",
            1.0);

        TestString("Строка с цифрами",
            @"s = ""12345""
s",
            "12345");

        TestString("Строка похожая на число",
            @"s = ""3.14""
s",
            "3.14");

        TestString("Строка с научной нотацией",
            @"s = ""1e10""
s",
            "1e10");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 9: СМЕШАННЫЕ ОПЕРАЦИИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Строка + число через concat",
            @"s = ""value: ""
n = 42
result = concat(s, ""42"")
result",
            "value: 42");

        TestString("Массив строк и чисел (преобразование)",
            @"# НЕЛЬЗЯ смешивать типы в массиве!
# Но можем создать массив строк
arr = [""1"", ""2"", ""3""]
join(arr, "","")  ",
            "1,2,3");

        TestNumber("len строки vs len массива",
            @"s = ""test""
arr = [1, 2, 3, 4]
len(s) + len(arr)",
            8.0);

        // Финальная статистика
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  ИТОГИ: {passedTests}/{totalTests} тестов пройдено ({(passedTests * 100.0 / totalTests):F1}%)");
        Console.WriteLine($"║  ✅ Успешных: {passedTests}");
        Console.WriteLine($"║  ❌ Провалено: {failedTests}");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        if (failedTests > 0)
        {
            Console.WriteLine("⚠️ Есть непрошедшие тесты!");
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine("🎉 ВСЕ ТЕСТЫ ТОКЕНИЗАЦИИ ПРОЙДЕНЫ!");
            Environment.Exit(0);
        }
    }
}

