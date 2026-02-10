using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace EdgeCaseTests;

/// <summary>
/// СЛОЖНЫЕ тесты комментариев - граничные случаи и edge cases
/// </summary>
class ComplexCommentTests
{
    static int passedTests = 0;
    static int failedTests = 0;
    static int totalTests = 0;

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

    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  СЛОЖНЫЕ ТЕСТЫ КОММЕНТАРИЕВ (ГРАНИЧНЫЕ СЛУЧАИ)              ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("═══ ГРУППА 1: ВЛОЖЕННЫЕ КОНСТРУКЦИИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Вложенные циклы с комментариями",
            @"total = 0
for i = 1 to 3:
    for j = 1 to 2:
        total = total + i * j
total",
            18.0);

        TestNumber("Вложенные if с комментариями",
            @"x = 10
result = 0
if x > 5:
    if x > 8:
        if x > 9:
            result = 100
result",
            100.0);

        TestNumber("Цикл с if и break с комментариями",
            @"sum_val = 0
for i = 1 to 100:
    if i > 10:
        break
    sum_val = sum_val + i
sum_val",
            55.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 2: КОММЕНТАРИИ С UNICODE И ЭМОДЗИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Комментарий с кириллицей и #",
            @"s = ""#Привет мир!""
s",
            "#Привет мир!");

        TestNumber("Комментарий с кириллицей после кода",
            @"x = 42
x",
            42.0);

        TestString("Строка с разными языками и #",
            @"s = ""#Hello #Привет #你好""
s",
            "#Hello #Привет #你好");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 3: ОЧЕНЬ ДЛИННЫЕ КОММЕНТАРИИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Очень длинный комментарий не влияет на код",
            @"x = 10
y = 20
x + y",
            30.0);

        TestNumber("Множественные строки с длинными комментариями",
            @"a = 1
b = 2
c = 3
d = 4
e = 5
a + b + c + d + e",
            15.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 4: МЕТОД НЬЮТОНА С КОММЕНТАРИЯМИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Метод Ньютона с комментариями на каждой строке",
            @"x = 2.0
tol = 1e-10
max_iter = 100
for i = 1 to max_iter:
    f_val = x*x - 4
    f_prime = 2*x
    x_new = x - f_val/f_prime
    if abs(x_new - x) < tol:
        break
    x = x_new
x",
            2.0, 1e-8);

        TestNumber("Сложные вычисления с комментариями",
            @"M = 2
m = 1
k = 27
omega = sqrt(k / (M + m))
omega",
            3.0, 1e-8);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 5: КОММЕНТАРИИ В ТЕРНАРНЫХ ОПЕРАТОРАХ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Тернарный оператор с комментарием после",
            @"x = 10
result = x > 5 ? 100 : 50
result",
            100.0);

        TestNumber("Вложенный тернарный (через if) с комментариями",
            @"x = 15
if x > 10:
    result = 100
elif x > 5:
    result = 50
else:
    result = 0
result",
            100.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 6: КОММЕНТАРИИ С ПРОБЕЛАМИ И ТАБУЛЯЦИЕЙ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Множество пробелов перед комментарием",
            @"x = 10
x",
            10.0);

        TestNumber("Табуляция перед комментарием",
            @"x = 5
x",
            5.0);

        TestNumber("Смешанные пробелы и табуляция",
            @"a = 1
b = 2
c = 3
a + b + c",
            6.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 7: КОММЕНТАРИИ В ФУНКЦИЯХ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Комментарии внутри вызова функции",
            @"x = max(
    5,
    10,
    3
)
x",
            10.0);

        TestNumber("Комментарии в сложных выражениях",
            @"a = 5
b = 3
result = (a + b) * (a - b)
result",
            16.0);

        TestNumber("Множественные функции с комментариями",
            @"x = sqrt(16)
y = abs(-10)
z = max(x, y)
z",
            10.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 8: КРИТИЧЕСКИЕ ГРАНИЧНЫЕ СЛУЧАИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Комментарий в начале каждой строки",
            @"a = 1
b = 2
c = 3
a + b + c",
            6.0);

        TestNumber("Чередование кода и комментариев",
            @"x = 1
x = x + 1
x = x + 1
x = x + 1
x",
            4.0);

        TestString("Комментарий после concat",
            @"s1 = ""Hello""
s2 = ""World""
concat(s1, "" "", s2)",
            "Hello World");

        TestNumber("Комментарий в условии while",
            @"i = 0
total = 0
while i < 5:
    total = total + i
    i = i + 1
total",
            10.0);

        TestString("Множественные строки с # и комментарии",
            @"s1 = ""#test1""
s2 = ""test2#""
s3 = ""#test3#""
concat(s1, s2, s3)",
            "#test1test2##test3#");

        TestNumber("Сложная физическая формула с комментариями",
            @"M = 2
m = 1
V0 = 2
k = 27
mu = 0.3
g = 10
x1 = (mu * g * (M + m)) / k
dt = (2 * V0) / (mu * g)
a = (k * x1 * x1) / (2 * (M + m))
x1 + dt + a",
            13.0 / 6.0, 1e-8);  // 1/3 + 4/3 + 1/2 = 13/6 ≈ 2.16667

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
            Console.WriteLine("🎉 ВСЕ СЛОЖНЫЕ ТЕСТЫ ПРОЙДЕНЫ!");
            Environment.Exit(0);
        }
    }
}

