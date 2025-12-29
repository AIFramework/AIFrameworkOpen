using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace ScientificNotationTest;

class TestOriginalScript
{
    static void Main(string[] args)
    {
        var processor = new Processor();
        
        Console.WriteLine("=== ТЕСТ ИСХОДНОГО ВЫРАЖЕНИЯ ПОЛЬЗОВАТЕЛЯ ===\n");
        
        // Точно такой же код, как отправил пользователь
        var originalScript = @"
// Метод Ньютона для уравнения x*sin(x) = 233
// Уточнение численных корней с высокой точностью
// Параметры метода Ньютона
tolerance = 1e-10
max_iterations = 100
// ПЕРВЫЙ КОРЕНЬ (начальное приближение x1 ≈ 233.95938)
x1 = 233.95938
for i = 1 to max_iterations:
    f_x1 = x1*sin(x1) - 233
    f_prime_x1 = sin(x1) + x1*cos(x1)
    x1_new = x1 - f_x1/f_prime_x1
    error_x1 = abs(x1_new - x1)
    if error_x1 < tolerance:
        break
    x1 = x1_new
// ВТОРОЙ КОРЕНЬ (начальное приближение x2 ≈ 234.13791)
x2 = 234.13791
for i = 1 to max_iterations:
    f_x2 = x2*sin(x2) - 233
    f_prime_x2 = sin(x2) + x2*cos(x2)
    x2_new = x2 - f_x2/f_prime_x2
    error_x2 = abs(x2_new - x2)
    if error_x2 < tolerance:
        break
    x2 = x2_new
// Проверка найденных корней подстановкой в исходное уравнение x^2 = 233*x/sin(x)
left1 = x1^2
right1 = 233*x1/sin(x1)
error_check1 = abs(left1 - right1)
relative_error1 = error_check1/left1
left2 = x2^2
right2 = 233*x2/sin(x2)
error_check2 = abs(left2 - right2)
relative_error2 = error_check2/left2
// Вывод результатов
root_x1 = x1
root_x2 = x2
verification_error1 = error_check1
verification_error2 = error_check2
verification_relative_error1 = relative_error1
verification_relative_error2 = relative_error2
";
        
        Console.WriteLine("Запуск исходного скрипта...\n");
        var output = processor.Run(originalScript);
        Console.WriteLine(string.Join("\n", output));
        
        Console.WriteLine("\n" + new string('═', 60));
        Console.WriteLine("РЕЗУЛЬТАТ: " + (output.Any(line => line.Contains("КРИТИЧЕСКАЯ ОШИБКА")) 
            ? "❌ ОШИБКА" 
            : "✅ УСПЕШНО"));
        Console.WriteLine(new string('═', 60));
    }
}

