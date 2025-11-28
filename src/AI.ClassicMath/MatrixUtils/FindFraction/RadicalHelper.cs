using System;
using System.Numerics;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

/// <summary>
/// Вспомогательный класс для работы с корнями n-й степени
/// </summary>
public static class RadicalHelper
{
    /// <summary>
    /// Упрощает корень n-й степени
    /// </summary>
    public static string SimplifyNthRoot(int value, int degree)
    {
        if (degree == 1) return value.ToString();
        if (degree == 2) return SimplifySquareRoot(value);
        if (degree == 3) return SimplifyCubeRoot(value);
        if (degree == 4) return SimplifyFourthRoot(value);

        // Для других степеней - базовое упрощение
        SimplifyNthRootGeneral(value, degree, out int outPart, out int inPart);

        if (inPart == 1) return outPart.ToString();

        string rootSymbol = GetRootSymbol(degree);
        if (outPart == 1) return $"{rootSymbol}{inPart}";
        return $"{outPart}{rootSymbol}{inPart}";
    }

    private static string SimplifySquareRoot(int n)
    {
        SimplifySqrt(n, out int outPart, out int inPart);
        if (inPart == 1) return outPart.ToString();
        if (outPart == 1) return $"√{inPart}";
        return $"{outPart}√{inPart}";
    }

    private static string SimplifyCubeRoot(int n)
    {
        SimplifyCubeRootInternal(n, out int outPart, out int inPart);
        if (inPart == 1) return outPart.ToString();
        if (outPart == 1) return $"∛{inPart}";
        return $"{outPart}∛{inPart}";
    }

    private static string SimplifyFourthRoot(int n)
    {
        SimplifyFourthRootInternal(n, out int outPart, out int inPart);
        if (inPart == 1) return outPart.ToString();
        if (outPart == 1) return $"⁴√{inPart}";
        return $"{outPart}⁴√{inPart}";
    }

    public static void SimplifySqrt(int n, out int outPart, out int inPart)
    {
        outPart = 1;
        inPart = Math.Abs(n);

        for (int i = 2; i * i <= Math.Abs(n); i++)
        {
            while (inPart % (i * i) == 0)
            {
                inPart /= i * i;
                outPart *= i;
            }
        }
    }

    private static void SimplifyCubeRootInternal(int n, out int outPart, out int inPart)
    {
        outPart = 1;
        inPart = Math.Abs(n);

        for (int i = 2; i * i * i <= Math.Abs(n); i++)
        {
            int cube = i * i * i;
            while (inPart % cube == 0)
            {
                inPart /= cube;
                outPart *= i;
            }
        }

        if (n < 0)
        {
            outPart = -outPart;
        }
    }

    private static void SimplifyFourthRootInternal(int n, out int outPart, out int inPart)
    {
        outPart = 1;
        inPart = Math.Abs(n);

        for (int i = 2; i * i * i * i <= Math.Abs(n); i++)
        {
            int fourth = i * i * i * i;
            while (inPart % fourth == 0)
            {
                inPart /= fourth;
                outPart *= i;
            }
        }
    }

    private static void SimplifyNthRootGeneral(int n, int degree, out int outPart, out int inPart)
    {
        outPart = 1;
        inPart = Math.Abs(n);

        for (int i = 2; Math.Pow(i, degree) <= Math.Abs(n); i++)
        {
            int power = (int)Math.Pow(i, degree);
            while (inPart % power == 0)
            {
                inPart /= power;
                outPart *= i;
            }
        }
    }

    private static string GetRootSymbol(int degree)
    {
        switch (degree)
        {
            case 2: return "√";
            case 3: return "∛";
            case 4: return "⁴√";
            case 5: return "⁵√";
            case 6: return "⁶√";
            default: return $"^(1/{degree})√";
        }
    }

    /// <summary>
    /// Проверяет, является ли число корнем n-й степени из целого
    /// </summary>
    public static bool IsNthRoot(double value, int degree, double tolerance, out int radicand)
    {
        radicand = 0;
        double powered = Math.Pow(value, degree);
        int rounded = (int)Math.Round(powered);

        if (Math.Abs(powered - rounded) < tolerance)
        {
            // Проверка обратно
            double check = Math.Pow(rounded, 1.0 / degree);
            if (Math.Abs(check - value) < tolerance)
            {
                radicand = rounded;
                return true;
            }
        }

        return false;
    }
}

