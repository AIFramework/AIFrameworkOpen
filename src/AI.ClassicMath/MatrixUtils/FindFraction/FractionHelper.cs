using System;
using System.Numerics;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

/// <summary>
/// Класс для работы с дробями - выделение целой части, упрощение
/// </summary>
public static class FractionHelper
{
    /// <summary>
    /// Выделяет целую часть из дроби
    /// </summary>
    public static string FormatWithIntegerPart(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0) return "∞";
        if (numerator == 0) return "0";

        BigInteger wholePart = numerator / denominator;
        BigInteger remainder = BigInteger.Abs(numerator % denominator);

        // Не выводить гигантские дроби
        if ((double)denominator > 1e+5)
            return null;

        if (remainder == 0)
        {
            return wholePart.ToString();
        }

        if (wholePart == 0)
        {
            return $"{numerator}/{denominator}";
        }

        // Есть целая и дробная части
        string sign = numerator < 0 ? "-" : "";

        

        return $"{sign}{BigInteger.Abs(wholePart)} + {remainder}/{denominator}";
    }

    /// <summary>
    /// Упрощает дробь через НОД
    /// </summary>
    public static void Simplify(ref BigInteger numerator, ref BigInteger denominator)
    {
        if (denominator == 0) return;

        BigInteger gcd = GCD(BigInteger.Abs(numerator), BigInteger.Abs(denominator));
        numerator /= gcd;
        denominator /= gcd;

        // Знак всегда в числителе
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }
    }

    private static BigInteger GCD(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}

