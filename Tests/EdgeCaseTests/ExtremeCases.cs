using AI.ClassicMath.Calculator.ProcessorLogic;
using System;

namespace EdgeCaseTests;

class ExtremeCases
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  ЭКСТРЕМАЛЬНЫЕ ТЕСТЫ - ПОИСК ТОНКОСТЕЙ И ГРАНИЧНЫХ СЛУЧАЕВ  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");
        
        var processor = new Processor();
        int total = 0;
        int passed = 0;
        
        void Test(string name, string script, string expected)
        {
            total++;
            Console.WriteLine($"[{total}] {name}");
            Console.WriteLine($"    📝 Скрипт: {script.Replace("\n", "\\n")}");
            
            try
            {
                var result = processor.Run(script);
                var lastLine = "";
                foreach (var line in result)
                {
                    if (line.StartsWith("=> ") || line.StartsWith("!!! "))
                        lastLine = line.Replace("=> ", "").Replace("!!! КРИТИЧЕСКАЯ ОШИБКА: ", "ERROR: ");
                }
                
                if (expected == "ERROR")
                {
                    if (lastLine.StartsWith("ERROR:"))
                    {
                        Console.WriteLine($"    ✅ ОЖИДАЕМАЯ ОШИБКА");
                        passed++;
                    }
                    else
                    {
                        Console.WriteLine($"    ❌ ПРОВАЛЕН: ожидалась ошибка, получено {lastLine}");
                    }
                }
                else if (lastLine == expected)
                {
                    Console.WriteLine($"    ✅ ПРОШЕЛ: {expected}");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"    ❌ ПРОВАЛЕН: ожидалось {expected}, получено {lastLine}");
                }
            }
            catch (Exception ex)
            {
                if (expected == "ERROR")
                {
                    Console.WriteLine($"    ✅ ОЖИДАЕМАЯ ОШИБКА");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"    ❌ ОШИБКА: {ex.Message}");
                }
            }
        }
        
        Console.WriteLine("═══ ГРУППА 1: ОТРИЦАТЕЛЬНЫЕ ЦИКЛЫ - ЭКСТРЕМАЛЬНЫЕ СЛУЧАИ ═══\n");
        
        Test("Отрицательный шаг -2", 
            "total = 0\nfor i = 10 to 0 step -2:\n    total = total + i\ntotal", "30");
        
        Test("Отрицательный шаг -3", 
            "total = 0\nfor i = 15 to 0 step -3:\n    total = total + i\ntotal", "45");
        
        Test("Отрицательный шаг через ноль", 
            "total = 0\nfor i = 5 to -5 step -1:\n    total = total + i\ntotal", "0");
        
        Test("Отрицательный шаг большой", 
            "total = 0\nfor i = 100 to 0 step -10:\n    total = total + i\ntotal", "550");
        
        Test("Отрицательный шаг дробный", 
            "total = 0\nfor i = 10 to 0 step -0.5:\n    total = total + 1\ntotal", "21");
        
        Test("Отрицательный шаг не достигает конца", 
            "total = 0\nfor i = 10 to 1 step -3:\n    total = total + i\ntotal", "22");
        
        Test("Положительный старт, отрицательный конец, отрицательный шаг", 
            "total = 0\nfor i = 10 to -10 step -5:\n    total = total + 1\ntotal", "5");
        
        Console.WriteLine("\n═══ ГРУППА 2: ТЕРНАРНЫЕ ОПЕРАТОРЫ - ЭКСТРЕМАЛЬНЫЕ ═══\n");
        
        Test("Тернарный с функцией слева", 
            "x = 3\nmax(5, 3) > x ? 100 : 200", "100");
        
        Test("Тернарный с функцией справа", 
            "x = 5\nx > 3 ? max(10, 20) : min(1, 2)", "20");
        
        Test("Тернарный с обеими функциями", 
            "x = 5\nx > 3 ? abs(-10) : sqrt(16)", "10");
        
        Test("Множественные тернарные последовательно", 
            "a = 1 > 0 ? 1 : 0\nb = 2 > 1 ? 2 : 0\nc = 3 > 2 ? 3 : 0\na + b + c", "6");
        
        Test("Тернарный в степени", 
            "x = 5\n(x > 3 ? 2 : 3) ^ (x > 10 ? 3 : 2)", "4");
        
        Test("Тернарный в индексе массива", 
            "arr = [10, 20, 30]\nindex(arr, 1 > 0 ? 1 : 2)", "20");
        
        Test("Тернарный с нулями", 
            "x = 0\nx == 0 ? 0 : (1 / x)", "0");
        
        Test("Цепочка тернарных в выражении", 
            "(1>0?1:0) + (2>1?2:0) + (3>2?3:0)", "6");
        
        Console.WriteLine("\n═══ ГРУППА 3: БИТОВЫЕ ОПЕРАЦИИ - ГРАНИЧНЫЕ ═══\n");
        
        Test("Битовый сдвиг влево на 0", 
            "5 << 0", "5");
        
        Test("Битовый сдвиг вправо на 0", 
            "5 >> 0", "5");
        
        Test("Битовый сдвиг большого числа", 
            "1 << 10", "1024");
        
        Test("Битовый XOR с самим собой", 
            "xor(42, 42)", "0");
        
        Test("Битовый AND с нулем", 
            "42 & 0", "0");
        
        Test("Битовый OR с нулем", 
            "42 | 0", "42");
        
        Test("Битовый NOT от нуля", 
            "bitnot(0)", "-1");
        
        Test("Цепочка битовых операций", 
            "(5 & 3) | (2 << 1)", "5");
        
        Test("Битовые в тернарном", 
            "x = 7\n(x & 1) > 0 ? (x << 1) : (x >> 1)", "14");
        
        Console.WriteLine("\n═══ ГРУППА 4: МАССИВЫ - ЭКСТРЕМАЛЬНЫЕ ═══\n");
        
        Test("Массив из одних нулей", 
            "sum([0, 0, 0, 0, 0])", "0");
        
        Test("Массив с большими числами", 
            "sum([1e10, 2e10, 3e10])", "60000000000");
        
        Test("Массив с чередованием знаков", 
            "sum([1, -1, 1, -1, 1, -1])", "0");
        
        Test("Массив с дробными отрицательными", 
            "arr = [-0.5, -1.5, -2.5]\nsum(arr)", "-4.5  [-4 + 1/2]");
        
        Test("Индекс последнего элемента", 
            "arr = [10, 20, 30, 40, 50]\nindex(arr, 4)", "50");
        
        Test("Массив с выражениями и функциями", 
            "arr = [abs(-5), sqrt(16), 2^3]\nsum(arr)", "17");
        
        Test("Длина массива из одного элемента", 
            "len([42])", "1");
        
        Test("Пустой массив", 
            "len([])", "0");
        
        Console.WriteLine("\n═══ ГРУППА 5: СТРОКИ - ЭКСТРЕМАЛЬНЫЕ ═══\n");
        
        Test("Конкатенация многих строк", 
            "concat(\"a\", \"b\", \"c\", \"d\", \"e\")", "abcde");
        
        Test("substr от начала до конца", 
            "substr(\"Hello\", 0, 5)", "Hello");
        
        Test("substr один символ", 
            "substr(\"Hello\", 1, 1)", "e");
        
        Test("len от длинной строки", 
            "len(\"This is a very long string for testing\")", "38");
        
        Test("Строка с пробелами", 
            "len(\"   spaces   \")", "12");
        
        Test("Конкатенация чисел как строк", 
            "len(concat(\"12\", \"34\", \"56\"))", "6");
        
        Console.WriteLine("\n═══ ГРУППА 6: ВЛОЖЕННЫЕ КОНСТРУКЦИИ ═══\n");
        
        Test("Тройная вложенность циклов", 
            "total = 0\nfor i = 0 to 2:\n    for j = 0 to 2:\n        for k = 0 to 2:\n            total = total + 1\ntotal", "27");
        
        Test("Цикл с множественными break", 
            "total = 0\nfor i = 0 to 10:\n    if i == 3:\n        break\n    if i == 5:\n        break\n    total = total + i\ntotal", "3");
        
        Test("Цикл с continue в середине", 
            "total = 0\nfor i = 0 to 5:\n    if i == 2:\n        continue\n    if i == 4:\n        continue\n    total = total + i\ntotal", "9");
        
        Test("Вложенный if в цикле", 
            "total = 0\nfor i = 0 to 5:\n    if i > 2:\n        if i < 5:\n            total = total + i\ntotal", "7");
        
        Console.WriteLine("\n═══ ГРУППА 7: МАТЕМАТИЧЕСКИЕ ФУНКЦИИ - ГРАНИЧНЫЕ ═══\n");
        
        Test("log от 1", 
            "log(1)", "0");
        
        Test("exp от 0", 
            "exp(0)", "1");
        
        Test("sin от 0", 
            "sin(0)", "0");
        
        Test("cos от 1", 
            "cos(1)", "0.540302306");
        
        Test("sqrt от 1", 
            "sqrt(1)", "1");
        
        Test("Факториал от 1", 
            "fact(1)", "1");
        
        Test("Факториал от 5", 
            "fact(5)", "120");
        
        Test("gcd двух простых", 
            "gcd(17, 19)", "1");
        
        Test("lcm одинаковых", 
            "lcm(5, 5)", "5");
        
        Test("min отрицательных", 
            "min(-5, -3, -10)", "-10");
        
        Test("max отрицательных", 
            "max(-5, -3, -10)", "-3");
        
        Test("mean от массива", 
            "mean([1, 2, 3, 4, 5])", "3");
        
        Console.WriteLine("\n═══ ГРУППА 8: КОМБИНИРОВАННЫЕ ОПЕРАЦИИ ═══\n");
        
        Test("Битовые + арифметические + тернарные", 
            "x = 5\n(x & 3) + (x << 1) + (x > 3 ? 10 : 0)", "21");
        
        Test("Функции + массивы + циклы", 
            "arr = [1, 2, 3, 4, 5]\ntotal = 0\nfor i = 0 to 4:\n    total = total + index(arr, i)\ntotal", "15");
        
        Test("Степени + корни + логарифмы", 
            "x = 2^3\ny = sqrt(x)\nlog(x)", "2.07944154  [3ln(2)]");
        
        Test("Строки + массивы + длина", 
            "s1 = \"Hello\"\ns2 = \"World\"\nlen(s1) + len(s2)", "10");
        
        Test("Комплексные + реальные", 
            "z = 3 + 4*i\nabs(z)", "5");
        
        Console.WriteLine("\n═══ ГРУППА 9: ПРИОРИТЕТЫ ОПЕРАТОРОВ ═══\n");
        
        Test("Степень и умножение", 
            "2 * 3 ^ 2", "18");
        
        Test("Деление и модуль", 
            "10 / 2 % 3", "2");
        
        Test("Сдвиги и сложение", 
            "1 << 2 + 1", "8");
        
        Test("Сравнение и логические", 
            "1 > 0 && 2 > 1", "1");
        
        Test("Тернарный и сложение", 
            "1 + (2 > 1 ? 3 : 4) + 5", "9");
        
        Test("Унарный минус и степень", 
            "-2 ^ 2", "4");
        
        Test("Битовые AND/OR", 
            "5 | 3 & 6", "7");
        
        Console.WriteLine("\n═══ ГРУППА 10: EDGE CASES С ПЕРЕМЕННЫМИ ═══\n");
        
        Test("Переменная i как счетчик", 
            "for i = 0 to 3:\n    x = i\nx", "3");
        
        Test("Перезапись переменной в цикле", 
            "x = 0\nfor i = 0 to 5:\n    x = i\nx", "5");
        
        Test("Множественное присваивание", 
            "a = 1\nb = 2\nc = 3\na + b + c", "6");
        
        Test("Присваивание результата функции", 
            "x = abs(-42)\ny = sqrt(x)\ny", "6.4807407  [√42]");
        
        Test("Присваивание массива", 
            "arr = [1, 2, 3]\nsum(arr)", "6");
        
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  ИТОГИ: {passed}/{total} тестов пройдено ({(double)passed/total*100:F1}%)");
        Console.WriteLine($"║  ✅ Успешных: {passed}");
        Console.WriteLine($"║  ❌ Провалено: {total - passed}");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        
        if (passed < total)
        {
            Console.WriteLine($"\n⚠️  {total - passed} тестов провалено - требуется исправление!");
        }
        else
        {
            Console.WriteLine("\n🎉 ВСЕ ТЕСТЫ ПРОШЛИ!");
        }
    }
}

