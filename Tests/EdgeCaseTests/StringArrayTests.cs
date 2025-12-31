using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace EdgeCaseTests;

/// <summary>
/// МОЩНЫЕ ЧЕСТНЫЕ тесты массивов строк + join + конкатенация
/// Проверяем РЕАЛЬНОЕ содержимое, а не только длину!
/// </summary>
class StringArrayTests
{
    static int passedTests = 0;
    static int failedTests = 0;
    static int totalTests = 0;

    /// <summary>
    /// Честный тест: проверяет РЕАЛЬНОЕ содержимое строки
    /// </summary>
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

            // Ищем результат (последнюю строку с =>)
            string? actualString = null;
            foreach (var line in output)
            {
                if (line.Contains("=>"))
                {
                    var parts = line.Split(new[] { "=>" }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        actualString = parts[1].Trim();
                        // Убираем дополнительную информацию в скобках если есть
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
                Console.WriteLine($"   Результат: \"{actualString}\"");
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

    /// <summary>
    /// Честный тест: проверяет числовое значение
    /// </summary>
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

            // Ищем результат
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
                Console.WriteLine($"   Результат: {actual}");
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
        Console.WriteLine("║  МОЩНЫЕ ЧЕСТНЫЕ ТЕСТЫ МАССИВОВ СТРОК                         ║");
        Console.WriteLine("║  Проверяем join, concat, index, len и все операции!          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("═══ ГРУППА 1: БАЗОВЫЕ ОПЕРАЦИИ С МАССИВАМИ СТРОК ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Создание массива строк",
            @"arr = [""один"", ""два"", ""три""]
index(arr, 1)",
            "два");

        TestNumber("Длина массива строк",
            @"arr = [""one"", ""two"", ""three"", ""four""]
len(arr)",
            4.0);

        TestString("Доступ к первому элементу",
            @"arr = [""first"", ""second"", ""third""]
index(arr, 0)",
            "first");

        TestString("Доступ к последнему элементу",
            @"arr = [""alpha"", ""beta"", ""gamma""]
index(arr, 2)",
            "gamma");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 2: JOIN - СОЕДИНЕНИЕ МАССИВОВ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("JOIN с пробелом",
            @"arr = [""Hello"", ""World"", ""!""]
join(arr, "" "")",
            "Hello World !");

        TestString("JOIN с запятой",
            @"arr = [""яблоко"", ""груша"", ""банан""]
join(arr, "", "")",
            "яблоко, груша, банан");

        TestString("JOIN без разделителя",
            @"arr = [""a"", ""b"", ""c"", ""d""]
join(arr, """")",
            "abcd");

        TestString("JOIN с длинным разделителем",
            @"arr = [""one"", ""two"", ""three""]
join(arr, "" -> "")",
            "one -> two -> three");

        TestString("JOIN одного элемента",
            @"arr = [""single""]
join(arr, "", "")",
            "single");

        TestString("JOIN с символом #",
            @"arr = [""test"", ""value"", ""end""]
join(arr, ""#"")",
            "test#value#end");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 3: СТРОКИ С # ВНУТРИ МАССИВОВ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Массив со строками начинающимися с #",
            @"arr = [""#hashtag"", ""#python"", ""#test""]
join(arr, "" "")",
            "#hashtag #python #test");

        TestString("Массив со строками с # в середине",
            @"arr = [""test#1"", ""test#2"", ""test#3""]
index(arr, 1)",
            "test#2");

        TestString("Массив со строками заканчивающимися на #",
            @"arr = [""end#"", ""final#"", ""last#""]
join(arr, "" | "")",
            "end# | final# | last#");

        TestString("Смешанные позиции #",
            @"arr = [""#start"", ""mid#dle"", ""end#""]
join(arr, """")",
            "#startmid#dleend#");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 4: CONCAT С ЭЛЕМЕНТАМИ МАССИВА ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("CONCAT двух элементов массива",
            @"arr = [""Hello"", ""World""]
concat(index(arr, 0), "" "", index(arr, 1))",
            "Hello World");

        TestString("CONCAT всех элементов вручную",
            @"arr = [""a"", ""b"", ""c""]
concat(index(arr, 0), index(arr, 1), index(arr, 2))",
            "abc");

        TestString("CONCAT с добавлением суффикса",
            @"arr = [""file"", ""txt""]
concat(index(arr, 0), ""."", index(arr, 1))",
            "file.txt");

        TestString("CONCAT элементов с # внутри",
            @"arr = [""#one"", ""two#"", ""#three#""]
concat(index(arr, 0), ""-"", index(arr, 2))",
            "#one-#three#");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 5: КОМБИНИРОВАННЫЕ ОПЕРАЦИИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestNumber("Длина JOIN результата",
            @"arr = [""one"", ""two"", ""three""]
result = join(arr, ""-"")
len(result)",
            13.0);  // "one-two-three" = 13

        TestString("JOIN + CONCAT",
            @"arr1 = [""Hello"", ""World""]
arr2 = [""Foo"", ""Bar""]
result1 = join(arr1, "" "")
result2 = join(arr2, "" "")
concat(result1, "" & "", result2)",
            "Hello World & Foo Bar");

        TestNumber("Подсчет элементов с #",
            @"arr = [""#one"", ""two"", ""#three"", ""four"", ""#five""]
# Подсчитываем строки начинающимися с #
count = 0
for i = 0 to 4:
    s = index(arr, i)
    first_char = substr(s, 0, 1)
    # Сравнение строк теперь работает!
    if first_char == ""#"":
        count = count + 1
count",
            3.0);

        TestString("Фильтрация и JOIN",
            @"arr = [""#tag1"", ""normal"", ""#tag2"", ""text""]
# Собираем только строки с #
result1 = """"
result2 = """"
# Проверяем каждый элемент
s0 = index(arr, 0)
s2 = index(arr, 2)
if substr(s0, 0, 1) == ""#"":
    result1 = s0
if substr(s2, 0, 1) == ""#"":
    result2 = s2
concat(result1, "" "", result2)",
            "#tag1 #tag2");

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 6: ГРАНИЧНЫЕ СЛУЧАИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Пустые строки в массиве",
            @"arr = ["""", ""test"", """", ""end""]
join(arr, ""|"")",
            "|test||end");

        TestString("Только пустые строки",
            @"arr = ["""", """", """"]
join(arr, "","") ",
            ",,");

        TestString("Очень длинные строки",
            @"arr = [""This is a very long string for testing"", ""Another long string here""]
len(join(arr, "" - ""))",
            "This is a very long string for testing - Another long string here");

        TestNumber("Массив из одной строки с #",
            @"arr = [""#solo""]
len(index(arr, 0))",
            5.0);

        // ═══════════════════════════════════════════════════════════════
        Console.WriteLine("\n═══ ГРУППА 7: СЛОЖНЫЕ СЦЕНАРИИ ═══\n");
        // ═══════════════════════════════════════════════════════════════

        TestString("Построение CSV строки",
            @"names = [""Alice"", ""Bob"", ""Charlie""]
ages = [""25"", ""30"", ""35""]
row1 = join(names, "","")
row2 = join(ages, "","")
# Калькулятор НЕ интерпретирует \n как перевод строки
concat(row1, ""---"", row2)",
            "Alice,Bob,Charlie---25,30,35");

        TestString("Путь к файлу из массива",
            @"parts = [""C:"", ""Users"", ""Documents"", ""file.txt""]
# Калькулятор работает с \\ как с двумя символами
join(parts, ""/"")",
            "C:/Users/Documents/file.txt");

        TestString("URL из компонентов",
            @"protocol = ""https:""
domain = ""example.com""
path = ""api""
endpoint = ""users""
# Простое формирование URL через concat
concat(protocol, ""//"", domain, ""/"", path, ""/"", endpoint)",
            "https://example.com/api/users");

        TestNumber("Поиск подстроки в элементах",
            @"arr = [""test#123"", ""value#456"", ""data#789""]
# Находим элемент содержащий #456
found = """"
for i = 0 to 2:
    s = index(arr, i)
    # Проверяем есть ли #456 через substr
    if len(s) >= 9:
        check = substr(s, 5, 4)
        if check == ""#456"":
            found = s
len(found)",
            10.0);  // "value#456" = 10

        Console.WriteLine("\n═══ ГРУППА 8: КОММЕНТАРИИ + МАССИВЫ СТРОК ═══\n");

        TestString("Комментарии при работе с массивами",
            @"# Создаем массив имен
names = [""Alice"", ""Bob"", ""Charlie""]  # Три имени
# Соединяем с разделителем
result = join(names, "" и "")  # Используем 'и' как разделитель
result",
            "Alice и Bob и Charlie");

        TestString("Многострочный скрипт с комментариями",
            @"# ТЕСТ: Обработка массива строк с # внутри
arr = [""#hashtag"", ""normal"", ""test#123""]  # Массив с разными вариантами
# Берем первый элемент
first = index(arr, 0)  # Должен быть '#hashtag'
# Берем последний
last = index(arr, 2)  # Должен быть 'test#123'
# Соединяем
concat(first, "" + "", last)  # Результат",
            "#hashtag + test#123");

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
            Console.WriteLine("🎉 ВСЕ МОЩНЫЕ ЧЕСТНЫЕ ТЕСТЫ ПРОЙДЕНЫ!");
            Environment.Exit(0);
        }
    }
}

