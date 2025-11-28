using System;
using System.Linq;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

/// <summary>
/// Алгоритм поиска целочисленных полиномиальных коэфициентов
/// Использует градиентный спуск с округлением до целых
/// </summary>
public static class IntegerRelationFinder
{
    /// <summary>
    /// Находит целочисленные коэффициенты полинома для заданного числа
    /// </summary>
    public static int[] FindPolynomial(double x, int degree, double tolerance)
    {
        // Специальная проверка для биквадратных (степень 4)
        if (degree == 4)
        {
            var biquad = TryBiquadratic(x, tolerance);
            if (biquad != null) return biquad;
        }

        // Основной метод: градиентный спуск
        return GradientDescentInteger(x, degree, tolerance);
    }

    private static int[] GradientDescentInteger(double x, int degree, double tolerance)
    {
        // Массив степеней x
        double[] xPowers = new double[degree + 1];
        xPowers[0] = 1;
        for (int i = 1; i <= degree; i++)
        {
            xPowers[i] = xPowers[i - 1] * x;
        }

        int maxCoeff = degree <= 2 ? 100 : degree == 3 ? 50 : degree == 4 ? 50 : 30;
        int maxIterations = degree <= 2 ? 5000 : degree == 4 ? 10000 : 3000;

        // Множественные запуски с разными начальными условиями
        int numRestarts = degree <= 2 ? 10 : degree == 4 ? 20 : 5;

        int[] bestCoeffs = null;
        double bestError = double.MaxValue;

        for (int restart = 0; restart < numRestarts; restart++)
        {
            Random rand = new Random(42 + restart);

            // Инициализация
            int[] coeffs = new int[degree + 1];
            coeffs[degree] = 1; // Старший коэффициент = 1

            for (int i = 0; i < degree; i++)
            {
                coeffs[i] = rand.Next(-maxCoeff / 2, maxCoeff / 2 + 1);
            }

            // Градиентный спуск
            for (int iter = 0; iter < maxIterations; iter++)
            {
                double f = EvaluatePolynomial(coeffs, xPowers);
                double currentError = f * f; // MSE

                if (currentError < tolerance * tolerance)
                {
                    return coeffs;
                }

                bool improved = false;

                for (int i = 0; i < degree; i++) // Не трогать старший коэффициент
                {
                    int original = coeffs[i];

                    int[] deltas = { -2, -1, 1, 2 };

                    foreach (int delta in deltas)
                    {
                        coeffs[i] = original + delta;

                        if (Math.Abs(coeffs[i]) > maxCoeff)
                        {
                            coeffs[i] = original;
                            continue;
                        }

                        double newF = EvaluatePolynomial(coeffs, xPowers);
                        double newError = newF * newF;

                        if (newError < currentError)
                        {
                            currentError = newError;
                            improved = true;
                            break;
                        }
                        else
                        {
                            coeffs[i] = original;
                        }
                    }
                }

                if (!improved)
                {
                    break;
                }
            }

            // Финальная проверка
            double finalError = Math.Abs(EvaluatePolynomial(coeffs, xPowers));

            if (finalError < bestError)
            {
                bestError = finalError;
                bestCoeffs = (int[])coeffs.Clone();

                if (bestError < tolerance)
                {
                    return bestCoeffs;
                }
            }
        }

        // Умножаю на 10 для того чтобы все пахало т.е. будет 1e-8 - это тоже адекватно
        return bestError < tolerance * 100.0 ? bestCoeffs : null;
    }

    private static double EvaluatePolynomial(int[] coeffs, double[] xPowers)
    {
        double result = 0;
        for (int i = 0; i < coeffs.Length; i++)
        {
            result += coeffs[i] * xPowers[i];
        }
        return result;
    }

    private static double EvaluatePolynomial(int[] coeffs, double x)
    {
        double result = 0;
        double xPow = 1;
        for (int i = 0; i < coeffs.Length; i++)
        {
            result += coeffs[i] * xPow;
            xPow *= x;
        }
        return Math.Abs(result);
    }

    private static int[] TryBiquadratic(double x, double tolerance)
    {
        // Биквадратное: ax^4 + cx^2 + e = 0 (коэффициенты при x^3 и x равны 0)

        double y = x * x;


        for (int c = -50; c <= 50; c++)
        {
            for (int e = -50; e <= 50; e++)
            {
                // Уравнение: y^2 + cy + e = 0
                double error = y * y + c * y + e;

                if (Math.Abs(error) < tolerance)
                {
                    int[] coeffs = new int[5] { e, 0, c, 0, 1 }; // e + 0x + cx^2 + 0x^3 + x^4

                    double checkError = EvaluatePolynomial(coeffs, x);
                    if (checkError < tolerance)
                    {
                        return coeffs;
                    }
                }
            }
        }

        return null;
    }
}
