using System;
using System.Collections.Generic;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

/// <summary>
/// База трансцендентных чисел и операции с ними
/// </summary>
public static class TranscendentalNumbers
{
    private static readonly Dictionary<double, string> KnownConstants = new Dictionary<double, string>
    {
        { Math.PI, "π" },
        { Math.E, "e" },
        { Math.PI * 2, "2π" },
        { Math.PI / 2, "π/2" },
        { Math.PI / 3, "π/3" },
        { Math.PI / 4, "π/4" },
        { Math.PI / 6, "π/6" },
        { Math.Sqrt(Math.PI), "√π" },
        { 1.0 / Math.PI, "1/π" },
        { Math.Log(2), "ln(2)" },
        { Math.Log(10), "ln(10)" },
        { 1.0 / Math.E, "1/e" },
        { Math.E * Math.E, "e²" },
        { (1 + Math.Sqrt(5)) / 2, "φ" }, // Золотое сечение (алгебраическое, но часто используется)
    };

    /// <summary>
    /// Проверяет, является ли число известной трансцендентной константой
    /// </summary>
    public static ConversionResult CheckKnownConstant(double number, double tolerance)
    {
        foreach (var kvp in KnownConstants)
        {
            if (Math.Abs(number - kvp.Key) < tolerance)
            {
                double error = Math.Abs(number - kvp.Key);
                return new ConversionResult
                {
                    Type = "Transcendent",
                    Fraction = kvp.Value,
                    Description = $"Трансцендентное число: {kvp.Value} (погрешность: {error:E3})",
                    Numerator = 0,
                    Denominator = 0
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Проверяет, является ли число произведением рационального на трансцендентное
    /// </summary>
    public static ConversionResult CheckRationalMultiple(double number, double tolerance)
    {
        var baseConstants = new Dictionary<double, string>
        {
            { Math.PI, "π" },
            { Math.E, "e" },
            { Math.Log(2), "ln(2)" },
            { Math.Sqrt(Math.PI), "√π" }
        };

        foreach (var kvp in baseConstants)
        {
            double ratio = number / kvp.Key;

            // является ли рациональным числом с небольшим знаменателем
            for (int den = 1; den <= 20; den++)
            {
                for (int num = -100; num <= 100; num++)
                {
                    if (num == 0) continue;

                    double testRatio = (double)num / den;
                    if (Math.Abs(ratio - testRatio) < tolerance)
                    {
                        double error = Math.Abs(number - testRatio * kvp.Key);

                        string fractionStr;
                        if (den == 1)
                        {
                            if (num == 1)
                                fractionStr = kvp.Value;
                            else if (num == -1)
                                fractionStr = $"-{kvp.Value}";
                            else
                                fractionStr = $"{num}{kvp.Value}";
                        }
                        else
                        {
                            if (num == 1)
                                fractionStr = $"{kvp.Value}/{den}";
                            else if (num == -1)
                                fractionStr = $"-{kvp.Value}/{den}";
                            else
                                fractionStr = $"{num}{kvp.Value}/{den}";
                        }

                        return new ConversionResult
                        {
                            Type = "Transcendent",
                            Fraction = fractionStr,
                            Description = $"Рациональное кратное {kvp.Value} (погрешность: {error:E3})",
                            Numerator = 0,
                            Denominator = 0
                        };
                    }
                }
            }
        }

        return null;
    }
}

