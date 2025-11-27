using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.ClassicMath.Calculator;

/// <summary>
/// Распознает известные математические константы и выражения
/// </summary>
public static class KnownConstants
{
    private static readonly List<(double Value, string Symbol, string Name)> Constants = new()
    {
        // Основные математические константы
        (Math.PI, "π", "Pi"),
        (Math.E, "e", "Euler's number"),
        
        // Основные квадратные корни
        (1.41421356237309, "√2", "Square root of 2"),
        (1.73205080756887, "√3", "Square root of 3"),
        (2.23606797749978, "√5", "Square root of 5"),
        
        // Важные дроби корней (тригонометрия)
        (0.707106781186547, "√2/2", "Half of square root of 2"),
        (0.866025403784438, "√3/2", "Half of square root of 3"),
        
        // Рационализации (часто встречающиеся)
        (1.15470053837925, "2√3/3", "2/√3 rationalized"),
        (1.88982236504614, "5√7/7", "5/√7 rationalized")
    };

    /// <summary>
    /// Попытка найти символьное представление для числа
    /// </summary>
    public static string TryGetSymbolicForm(double value, double tolerance = 1e-8)
    {
        // Проверяем, является ли число целым
        if (Math.Abs(value - Math.Round(value)) < tolerance)
        {
            return null; // Целые числа не нуждаются в символьной форме
        }

        // Ищем в списке известных констант (быстрый путь)
        foreach (var (constValue, symbol, _) in Constants)
        {
            if (Math.Abs(value - constValue) < tolerance)
            {
                return symbol;
            }
        }

        // Предвычисляем корни для эффективности (до 20 для простых корней)
        double[] sqrts = new double[21];
        for (int i = 2; i <= 20; i++)
        {
            sqrts[i] = Math.Sqrt(i);
        }

        // СНАЧАЛА проверяем a√b (где a = 2..5, b = 2..20) - приоритет упрощениям!
        for (int a = 2; a <= 5; a++)
        {
            for (int b = 2; b <= 20; b++)
            {
                if (Math.Abs(value - a * sqrts[b]) < tolerance)
                {
                    return $"{a}√{b}";
                }
            }
        }

        // Затем проверяем простые корни √i
        for (int i = 2; i <= 20; i++)
        {
            if (Math.Abs(value - sqrts[i]) < tolerance)
            {
                return $"√{i}";
            }
        }

        // Проверяем a√b/c (рационализация и обычные дроби)
        for (int b = 2; b <= 10; b++)
        {
            double sqrtB = sqrts[b];
            
            for (int a = 1; a <= 10; a++)
            {
                for (int c = 2; c <= 10; c++)
                {
                    double result = a * sqrtB / c;
                    if (Math.Abs(value - result) < tolerance)
                    {
                        // Упрощаем запись
                        if (a == 1)
                            return $"√{b}/{c}";
                        else
                            return $"{a}√{b}/{c}";
                    }
                }
            }
        }

        return null;
    }

}

