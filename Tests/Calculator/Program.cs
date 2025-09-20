using AI.ClassicMath.Calculator;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System.Globalization;
using System.Numerics;
using ExecutionContext = AI.ClassicMath.Calculator.ExecutionContext;
using Vector = AI.DataStructs.Algebraic.Vector;


//ToDo: реализовать поддержку скриптов и уравнений
public class Program
{
    public static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        //Console.WriteLine("Калькулятор с поддержкой комплексных чисел. Используйте 'i' для мнимой единицы.");
        //Console.WriteLine("Примеры: 3 + 4*i, sqrt(-1), exp(i*pi)");

        var context = new ExecutionContext();
        while (true)
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "exit") break;
            var result = AdvancedCalculator.Evaluate(input, context);
            if (result != null) Console.WriteLine($"=> {FormatResult(result)}");
        }
    }

    /// <summary>
    /// Корректно форматирует результат вычисления для вывода в консоль.
    /// </summary>
    public static string FormatResult(object result) => result switch
    {
        // Базовые типы
        double d => d.ToString("G6"),
        Complex c => (Math.Abs(c.Imaginary) < 1e-12) ? c.Real.ToString("G6") : $"{c.Real:G4} + {c.Imaginary:G4}i",

        ComplexVector v => $"[{string.Join(", ", v.Select(c => FormatResult(c)))}]",

        Vector dv => $"[{string.Join(", ", dv.Select(c => c.ToString("G4", CultureInfo.InvariantCulture)))}]",

        // Обработка остальных случаев
        _ => result?.ToString() ?? "null"
    };
}
