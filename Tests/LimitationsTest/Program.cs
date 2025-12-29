using AI.ClassicMath.Calculator.ProcessorLogic;
using System;
using System.Linq;

namespace ScientificNotationTest;

class TestLimitations
{
    static void Main(string[] args)
    {
        var processor = new Processor();
        
        Console.WriteLine("═══ ТЕСТИРОВАНИЕ ОГРАНИЧЕНИЙ ═══\n");
        
        // Ограничение 1: else на той же строке с }
        Console.WriteLine("1. else на той же строке с }:");
        var script1 = @"
x = 5
if (x > 3) {
    result = 1
} else {
    result = 0
}
";
        var output1 = processor.Run(script1);
        Console.WriteLine(output1.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        if (output1.Any(l => l.Contains("ОШИБКА")))
            Console.WriteLine("   " + string.Join("\n   ", output1.Where(l => l.Contains("ОШИБКА"))));
        
        // Ограничение 2: else: для Python-style
        Console.WriteLine("\n2. else: для Python-style if:");
        var script2 = @"
x = 5
if x > 3:
    result = 1
else:
    result = 0
final = result
";
        var output2 = processor.Run(script2);
        Console.WriteLine(output2.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        if (output2.Any(l => l.Contains("ОШИБКА")))
            Console.WriteLine("   " + string.Join("\n   ", output2.Where(l => l.Contains("ОШИБКА"))));
        
        // Ограничение 3: модуль %
        Console.WriteLine("\n3. Оператор модуля (%):");
        var script3 = @"
result = 10 % 3
";
        var output3 = processor.Run(script3);
        Console.WriteLine(output3.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        // Ограничение 4: += -= *= /=
        Console.WriteLine("\n4. Составные операторы (+=, -=, *=, /=):");
        var script4 = @"
x = 5
x += 3
result = x
";
        var output4 = processor.Run(script4);
        Console.WriteLine(output4.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        // Ограничение 5: ++, --
        Console.WriteLine("\n5. Инкремент/декремент (++, --):");
        var script5 = @"
x = 5
x++
result = x
";
        var output5 = processor.Run(script5);
        Console.WriteLine(output5.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        // Ограничение 6: elif/elsif
        Console.WriteLine("\n6. elif для Python-style:");
        var script6 = @"
x = 5
if x > 10:
    result = 1
elif x > 3:
    result = 2
else:
    result = 3
final = result
";
        var output6 = processor.Run(script6);
        Console.WriteLine(output6.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        // Ограничение 7: логические операторы and/or
        Console.WriteLine("\n7. Логические операторы (and, or, not):");
        var script7 = @"
x = 5
if x > 3 and x < 10:
    result = 1
else:
    result = 0
final = result
";
        var output7 = processor.Run(script7);
        Console.WriteLine(output7.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        // Ограничение 8: строки
        Console.WriteLine("\n8. Строки и конкатенация:");
        var script8 = @"
text = ""Hello""
result = text
";
        var output8 = processor.Run(script8);
        Console.WriteLine(output8.Any(l => l.Contains("ОШИБКА")) ? "❌ НЕ РАБОТАЕТ" : "✅ РАБОТАЕТ");
        
        Console.WriteLine("\n═══════════════════════════════════════");
    }
}

