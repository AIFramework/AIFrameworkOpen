using System;
using System.Numerics;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

/// <summary>
/// Класс для анализа рациональных чисел (конечные и периодические дроби)
/// </summary>
public static class RationalAnalyzer
{
    public static ConversionResult Analyze(double number, double tolerance)
    {
        BigInteger integerPart = (BigInteger)number;
        double fractionalPartVal = Math.Abs(number - (double)integerPart);

        string s = fractionalPartVal.ToString("G17");
        if (s.Contains("E")) return null;

        int decimalIndex = s.IndexOf(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
        if (decimalIndex == -1) decimalIndex = s.IndexOf('.');

        string digits = decimalIndex >= 0 && decimalIndex < s.Length - 1 ? s.Substring(decimalIndex + 1) : "";
        bool isNegative = number < 0;

        bool maxPrecisionUsed = digits.Length >= 14;

        if (!maxPrecisionUsed)
        {
            return CreateTerminating(digits, integerPart, isNegative);
        }

        var cycleResult = TryFindCycleImproved(digits, integerPart, isNegative);
        if (cycleResult != null) return cycleResult;

        if (HasRecurringSuffix(digits, '9', out string digitsRoundedUp))
        {
            var result = TryFindCycleImproved(digitsRoundedUp, integerPart, isNegative);
            if (result != null)
            {
                result.Description += " (с учётом округления 9...)";
                return result;
            }
        }
        if (HasRecurringSuffix(digits, '0', out string digitsRoundedDown))
        {
            var result = TryFindCycleImproved(digitsRoundedDown, integerPart, isNegative);
            if (result != null)
            {
                result.Description += " (с учётом округления 0...)";
                return result;
            }
        }

        return null;
    }

    private static ConversionResult CreateTerminating(string digits, BigInteger integerPart, bool isNegative)
    {
        if (string.IsNullOrEmpty(digits))
        {
            return new ConversionResult
            {
                Type = "Integer",
                Fraction = integerPart.ToString(),
                Description = "Целое число (погрешность: 0)",
                Numerator = integerPart,
                Denominator = 1
            };
        }

        BigInteger den = BigInteger.Pow(10, digits.Length);
        BigInteger num = BigInteger.Parse(digits);

        BigInteger totalNum = BigInteger.Abs(integerPart) * den + num;
        if (isNegative) totalNum = -totalNum;

        FractionHelper.Simplify(ref totalNum, ref den);

        string fractionStr = FractionHelper.FormatWithIntegerPart(totalNum, den);

        return new ConversionResult
        {
            Type = "Terminating",
            Fraction = fractionStr,
            Description = "Конечная дробь (погрешность: 0)",
            Numerator = totalNum,
            Denominator = den
        };
    }

    private static ConversionResult TryFindCycleImproved(string digits, BigInteger integerPart, bool isNegative)
    {
        int maxPeriodLen = digits.Length / 2;

        for (int periodLen = 1; periodLen <= maxPeriodLen; periodLen++)
        {
            int minCycles = GetMinCycles(periodLen);

            for (int prePeriodLen = 0; prePeriodLen <= digits.Length - periodLen * minCycles; prePeriodLen++)
            {
                int requiredCycles = minCycles;
                if (prePeriodLen > 0 && periodLen == 1)
                    requiredCycles = Math.Max(9, minCycles);

                if (prePeriodLen + periodLen * requiredCycles > digits.Length)
                    continue;

                string prePeriod = digits.Substring(0, prePeriodLen);
                string candidatePeriod = digits.Substring(prePeriodLen, periodLen);

                if (CheckCycleWithTolerance(digits, prePeriodLen, candidatePeriod, requiredCycles))
                    return CreateRepeatingResult(prePeriod, candidatePeriod, integerPart, isNegative);
            }
        }

        return null;
    }

    private static int GetMinCycles(int periodLen)
    {
        if (periodLen == 1) return 10;
        if (periodLen == 2) return 5;
        if (periodLen == 3) return 4;
        if (periodLen == 4) return 3;
        return 2;
    }

    private static bool CheckCycleWithTolerance(string digits, int prePeriodLen, string period, int minCycles)
    {
        int periodLen = period.Length;
        int cyclesFound = 0;
        int pos = prePeriodLen;

        while (pos + periodLen <= digits.Length)
        {
            string segment = digits.Substring(pos, periodLen);

            if (segment == period)
            {
                cyclesFound++;
                pos += periodLen;
            }
            else
            {
                if (pos + periodLen >= digits.Length - 1)
                {
                    if (IsRoundingError(segment, period))
                    {
                        cyclesFound++;
                        break;
                    }
                }
                break;
            }
        }

        if (pos < digits.Length)
        {
            string remaining = digits.Substring(pos);
            if (remaining.Length < periodLen)
            {
                if (!period.StartsWith(remaining) && !IsRoundingError(remaining, period.Substring(0, remaining.Length)))
                {
                    return false;
                }
            }
        }

        return cyclesFound >= minCycles;
    }

    private static bool IsRoundingError(string actual, string expected)
    {
        if (actual.Length != expected.Length) return false;

        for (int i = 0; i < actual.Length - 1; i++)
        {
            if (actual[i] != expected[i]) return false;
        }

        int lastActual = actual[actual.Length - 1] - '0';
        int lastExpected = expected[expected.Length - 1] - '0';

        return Math.Abs(lastActual - lastExpected) <= 1;
    }

    private static ConversionResult CreateRepeatingResult(string prePeriod, string period, BigInteger integerPart, bool isNegative)
    {
        BigInteger prePeriodVal = prePeriod == "" ? 0 : BigInteger.Parse(prePeriod);
        BigInteger combinedVal = BigInteger.Parse(prePeriod + period);

        BigInteger numeratorPart = combinedVal - prePeriodVal;

        System.Text.StringBuilder denStr = new System.Text.StringBuilder();
        for (int k = 0; k < period.Length; k++) denStr.Append('9');
        for (int k = 0; k < prePeriod.Length; k++) denStr.Append('0');

        BigInteger denominatorPart = BigInteger.Parse(denStr.ToString());

        BigInteger finalNum = BigInteger.Abs(integerPart) * denominatorPart + numeratorPart;
        if (isNegative) finalNum = -finalNum;

        FractionHelper.Simplify(ref finalNum, ref denominatorPart);

        string fractionStr = FractionHelper.FormatWithIntegerPart(finalNum, denominatorPart);

        double error = 1e-15;

        return new ConversionResult
        {
            Type = "Repeating",
            Fraction = fractionStr,
            Description = prePeriod == ""
                ? $"Периодическая дробь 0.({period}) (погрешность: {error:E3})"
                : $"Периодическая дробь 0.{prePeriod}({period}) (погрешность: {error:E3})",
            Numerator = finalNum,
            Denominator = denominatorPart
        };
    }

    private static bool HasRecurringSuffix(string digits, char c, out string roundedDigits)
    {
        roundedDigits = null;
        if (digits.Length < 5) return false;

        int count = 0;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            if (digits[i] == c) count++;
            else break;
        }

        if (count >= 3)
        {
            string prefix = digits.Substring(0, digits.Length - count);
            if (c == '0')
            {
                roundedDigits = prefix;
            }
            else
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    roundedDigits = "1";
                }
                else
                {
                    BigInteger val = BigInteger.Parse(prefix);
                    val++;
                    roundedDigits = val.ToString();
                }
            }
            return true;
        }
        return false;
    }
}

