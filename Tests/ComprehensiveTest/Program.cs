using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace ScientificNotationTest;

class ComprehensiveTests
{
    static int passedTests = 0;
    static int totalTests = 0;
    
    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         ПОЛНЫЙ НАБОР ТЕСТОВ МАТЕМАТИЧЕСКОГО КАЛЬКУЛЯТОРА     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝\n");
        
        // Тесты старой функциональности
        Console.WriteLine("═══ ГРУППА 1: БАЗОВАЯ ФУНКЦИОНАЛЬНОСТЬ (СТАРАЯ) ═══\n");
        Test("Арифметические операции", @"
result = 2 + 3 * 4 - 10 / 2
power = 2^10
", "result", 9);
        
        Test("Тригонометрические функции", @"
angle = 3.14159265358979 / 4
result = sin(angle)
", "result", 0.707, 0.001);
        
        Test("Квадратное уравнение", @"
a = 1
b = -3
c = 2
roots = QuadraticEquationSolver(a, b, c)
sum_roots = sum(roots)
", "sum_roots", 3);
        
        Test("Векторные операции", @"
v1 = [1, 2, 3]
v2 = [4, 5, 6]
dot_product = dot(v1, v2)
", "dot_product", 32);
        
        Test("Статистические функции", @"
minimum = min(5, 2, 8, 1)
maximum = max(5, 2, 8, 1)
average = mean(5, 2, 8, 1)
", "average", 4);
        
        Test("НОД и НОК", @"
nod = gcd(12, 18)
nok = lcm(12, 18)
", "nok", 36);
        
        Test("C-style if с фигурными скобками", @"
x = 10
result = 0
if (x > 5) {
    result = 1
}
output = result
", "output", 1);
        
        Test("C-style for цикл", @"
total = 0
for (i = 0; i < 5; i = i + 1) {
    total = total + i
}
result = total
", "result", 10);
        
        Test("C-style while цикл", @"
n = 1
total = 0
while (n <= 5) {
    total = total + n
    n = n + 1
}
result = total
", "result", 15);
        
        // Тесты новой функциональности
        Console.WriteLine("\n═══ ГРУППА 2: НАУЧНАЯ НОТАЦИЯ ═══\n");
        
        Test("Научная нотация - разные форматы", @"
a = 1e-10
b = 2.5e+3
c = 3E10
d = 1.5e-5
result = b
", "result", 2500);
        
        Test("Научная нотация в вычислениях", @"
tolerance = 1e-10
scaled = tolerance * 1e10
", "scaled", 1);
        
        // Тесты Python-style синтаксиса
        Console.WriteLine("\n═══ ГРУППА 3: PYTHON-STYLE СИНТАКСИС ═══\n");
        
        Test("Python-style for", @"
total = 0
for i = 1 to 5:
    total = total + i
result = total
", "result", 15);
        
        Test("Python-style for с шагом", @"
total = 0
for i = 0 to 10 step 2:
    total = total + i
result = total
", "result", 30);
        
        Test("Python-style if", @"
x = 10
result = 0
if x > 5:
    result = 100
else:
    result = 0
output = result
", "output", 100);
        
        Test("Python-style while", @"
n = 1
total = 0
while n <= 5:
    total = total + n
    n = n + 1
result = total
", "result", 15);
        
        // Тесты break и continue
        Console.WriteLine("\n═══ ГРУППА 4: BREAK И CONTINUE ═══\n");
        
        Test("Break в for цикле", @"
total = 0
for i = 1 to 100:
    total = total + i
    if total > 50:
        break
result = total
", "result", 55);
        
        Test("Continue в for цикле", @"
total = 0
for i = 1 to 10:
    if i == 5:
        continue
    total = total + i
result = total
", "result", 50);
        
        Test("Break в while цикле", @"
n = 1
total = 0
while n <= 100:
    total = total + n
    if total > 50:
        break
    n = n + 1
result = total
", "result", 55);
        
        Test("Continue в while (C-style)", @"
n = 0
total = 0
while (n < 10) {
    n = n + 1
    if (n == 5) {
        continue
    }
    total = total + n
}
result = total
", "result", 50);
        
        // Тесты вложенных конструкций
        Console.WriteLine("\n═══ ГРУППА 5: ВЛОЖЕННЫЕ КОНСТРУКЦИИ ═══\n");
        
        Test("Вложенные циклы", @"
total = 0
for i = 1 to 3:
    for j = 1 to 3:
        total = total + 1
result = total
", "result", 9);
        
        Test("Вложенный if в цикле", @"
positive_count = 0
for i = -5 to 5:
    if i > 0:
        positive_count = positive_count + 1
result = positive_count
", "result", 5);
        
        Test("Break во вложенном цикле", @"
found = 0
for i = 1 to 5:
    for j = 1 to 5:
        if i * j > 10:
            found = i * j
            break
    if found > 0:
        break
result = found
", "result", 12);
        
        // Тесты работы с переменной i
        Console.WriteLine("\n═══ ГРУППА 6: ПЕРЕМЕННАЯ 'i' (МНИМАЯ ЕДИНИЦА VS ПЕРЕМЕННАЯ) ═══\n");
        
        Test("i как мнимая единица (без присваивания)", @"
complex_result = i * i
real_part = complex_result
", "real_part", -1);
        
        Test("i как переменная цикла", @"
temp = 0
for i = 1 to 5:
    temp = i
result = temp
", "result", 5);
        
        Test("i после цикла остается переменной", @"
for i = 1 to 3:
    temp = i
result = i * 2
", "result", 8);
        
        Test("Комплексные числа", @"
z1 = 3 + 4*i
magnitude = abs(z1)
", "magnitude", 5);
        
        // Тесты смешанного синтаксиса
        Console.WriteLine("\n═══ ГРУППА 7: СМЕШАННЫЙ СИНТАКСИС ═══\n");
        
        Test("C-style и Python-style вместе", @"
total = 0
for i = 1 to 5:
    if (i % 2 == 0):
        total = total + i
result = total
", "result", 6);
        
        Test("Python if с C-style циклом", @"
total = 0
for (i = 0; i < 10; i = i + 1) {
    if i > 5:
        total = total + i
}
result = total
", "result", 30);
        
        // Тесты специальных функций
        Console.WriteLine("\n═══ ГРУППА 8: СПЕЦИАЛЬНЫЕ ФУНКЦИИ ═══\n");
        
        Test("floor, ceil, round", @"
a = floor(3.7)
b = ceil(3.2)
c = round(3.5)
result = a + b + c
", "result", 11);
        
        Test("abs для отрицательных чисел", @"
a = abs(-5.5)
b = abs(3.3)
result = a + b
", "result", 8.8, 0.001);
        
        Test("sqrt и степени", @"
a = sqrt(16)
b = a^2
result = b
", "result", 16);
        
        // Тест исходного выражения пользователя
        Console.WriteLine("\n═══ ГРУППА 9: ИСХОДНОЕ ВЫРАЖЕНИЕ ПОЛЬЗОВАТЕЛЯ ═══\n");
        
        Test("Метод Ньютона (полный)", @"
tolerance = 1e-10
max_iterations = 100
x1 = 233.95938
for i = 1 to max_iterations:
    f_x1 = x1*sin(x1) - 233
    f_prime_x1 = sin(x1) + x1*cos(x1)
    x1_new = x1 - f_x1/f_prime_x1
    error_x1 = abs(x1_new - x1)
    if error_x1 < tolerance:
        break
    x1 = x1_new
result = x1
", "result", 233.958, 0.01);
        
        // Граничные случаи
        Console.WriteLine("\n═══ ГРУППА 10: ГРАНИЧНЫЕ СЛУЧАИ ═══\n");
        
        Test("Пустой цикл (0 итераций)", @"
total = 0
for i = 10 to 5:
    total = total + 1
", "total", 0);
        
        Test("Break на первой итерации", @"
total = 0
for i = 1 to 100:
    total = total + i
    break
result = total
", "result", 1);
        
        Test("Continue на всех итерациях", @"
total = 0
count = 0
for i = 1 to 5:
    count = count + 1
    continue
    total = total + i
result = count
", "result", 5);
        
        Test("Деление на очень малое число", @"
small = 1e-10
result = 1 / small
", "result", 1e10);
        
        Test("Очень большие числа", @"
big = 1e100
result = big / 1e99
", "result", 10);
        
        // Итоговая статистика
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  РЕЗУЛЬТАТЫ: {passedTests}/{totalTests} ТЕСТОВ ПРОЙДЕНО");
        
        if (passedTests == totalTests)
        {
            Console.WriteLine("║  ✅ ВСЕ ТЕСТЫ УСПЕШНЫ! ФУНКЦИОНАЛЬНОСТЬ РАБОТАЕТ КОРРЕКТНО!");
        }
        else
        {
            Console.WriteLine($"║  ⚠️  ПРОВАЛЕНО: {totalTests - passedTests} ТЕСТОВ");
        }
        
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
    }
    
    static void Test(string name, string script, string resultVar, double expected, double tolerance = 0.0001)
    {
        totalTests++;
        var processor = new Processor();
        
        try
        {
            var output = processor.Run(script);
            
            // Проверяем на ошибки
            if (output.Any(line => line.Contains("КРИТИЧЕСКАЯ ОШИБКА")))
            {
                Console.WriteLine($"❌ [{totalTests}] {name}");
                Console.WriteLine($"   ОШИБКА: {string.Join(", ", output.Where(l => l.Contains("ОШИБКА")))}");
                return;
            }
            
            // Получаем результат
            var resultLine = output.FirstOrDefault(line => line.StartsWith($">> {resultVar} =") || line.StartsWith($"=> "));
            if (resultLine == null)
            {
                Console.WriteLine($"❌ [{totalTests}] {name}");
                Console.WriteLine($"   Переменная '{resultVar}' не найдена в выводе");
                return;
            }
            
            // Извлекаем значение после "=>"
            var lines = output.ToList();
            var varIndex = lines.FindIndex(l => l.Contains($">> {resultVar} =") || (l.StartsWith(">> ") && l.Contains(resultVar)));
            if (varIndex >= 0 && varIndex + 1 < lines.Count)
            {
                var valueLine = lines[varIndex + 1];
                if (valueLine.StartsWith("=> "))
                {
                    var valueStr = valueLine.Substring(3).Trim().Split(new[] { ' ', '[' })[0];
                    if (double.TryParse(valueStr, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out double actual))
                    {
                        if (Math.Abs(actual - expected) <= tolerance)
                        {
                            Console.WriteLine($"✅ [{totalTests}] {name}");
                            passedTests++;
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"❌ [{totalTests}] {name}");
                            Console.WriteLine($"   Ожидалось: {expected}, Получено: {actual}");
                            return;
                        }
                    }
                }
            }
            
            Console.WriteLine($"❌ [{totalTests}] {name}");
            Console.WriteLine($"   Не удалось извлечь результат");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [{totalTests}] {name}");
            Console.WriteLine($"   Исключение: {ex.Message}");
        }
    }
}

