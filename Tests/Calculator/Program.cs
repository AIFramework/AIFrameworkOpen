using AI.ClassicMath.Calculator;
using AI.ClassicMath.Calculator.ProcessorLogic;
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

        string script =

            //@"
            //a = 10
            //if (a > 2^5) {
            //    b = a * 2
            //}
            //else
            //{
            //    v1 = [1,2,3]
            //    v2 = [0.1,-0.2, 0.3]
            //    b = dot(v1, v2)/(mag(v1)*mag(v2)) // косинус
            //    b = b * 10
            //}
            //b // значение b

            //// Факториал
            //factorial = 1

            //for (j=1; j<=b; j=j+1) {
            //    factorial = factorial * j
            //}

            //factorial // финальное значение

            //fact(b) == factorial // проверка
            //gamma(b) > factorial // проверка
            //gamma(b)

            //"

            //"// Валидация корней кубического уравнения x^3 = x - 3\n// Корни из предыдущего решения:\nx1 = -1.67169988\nx2 = 0.835849941 + (-1.04686932)*i\nx3 = 0.835849941 + 1.04686932*i\n\n// Проверка первого корня x1\nleft1 = x1^3\nright1 = x1 - 3\ndiff1 = left1 - right1\n\n// Проверка второго корня x2\nleft2 = x2^3\nright2 = x2 - 3\ndiff2 = left2 - right2\n\n// Проверка третьего корня x3\nleft3 = x3^3\nright3 = x3 - 3\ndiff3 = left3 - right3"

            "// Валидация корней кубического уравнения x^3 = x - 3\n// Корни из предыдущего решения:\nx1 = -1.67169988\nx2 = 0.835849941 + (-1.04686932)*i\nx3 = 0.835849941 + 1.04686932*i\n\n// Проверка первого корня x1\nleft1 = x1^3\nright1 = x1 - 3\ndiff1 = left1 - right1\n\n// Проверка второго корня x2\nleft2 = x2^3\nright2 = x2 - 3\ndiff2 = left2 - right2\n\n// Проверка третьего корня x3\nleft3 = x3^3\nright3 = x3 - 3\ndiff3 = left3 - right3"
;

        //"diff1 = left1 - right1\n\n// Проверка второго корня x2\nleft2 = x2^3\nright2 = x2 - 3\ndiff2 = left2 - right2\n\n// Проверка третьего корня x3\nleft3 = x3^3\nright3 = x3 - 3\ndiff3 = left3 - right3"
        Processor processor = new Processor();
        var answer = processor.Run(script);
        Console.WriteLine(string.Join("\n", answer));







        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        AdvancedCalculator advancedCalculator = new AdvancedCalculator();

        var context = new ExecutionContext();
        while (true)
        {
            Console.Write(">> ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "exit") break;
            var result = advancedCalculator.Evaluate(input, context);
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
