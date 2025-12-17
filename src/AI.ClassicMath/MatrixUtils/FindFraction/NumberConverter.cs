using AI.ClassicMath.Calculator.Libs.Algebra;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AI.ClassicMath.MatrixUtils.FindFraction;

public static class NumberConverter
{
    public static ConversionResult Analyze(double number)
    {
        double tolerance = 1e-10;

        // 1. Check Integers
        if (Math.Abs(number % 1) < double.Epsilon)
        {
            return new ConversionResult
            {
                Type = "Integer",
                Fraction = ((BigInteger)number).ToString(),
                Description = "Целое число (погрешность: 0)",
                Numerator = (BigInteger)number,
                Denominator = 1
            };
        }

        // Трансцедентные числа
        var transcendental = TranscendentalNumbers.CheckKnownConstant(number, tolerance);
        if (transcendental != null) return transcendental;

        // Произведение рационального на трансцендентное
        var rationalMultiple = TranscendentalNumbers.CheckRationalMultiple(number, tolerance);
        if (rationalMultiple != null) return rationalMultiple;

        // Преобразование к рациональному (если возможно)
        var rational = RationalAnalyzer.Analyze(number, tolerance);
        if (rational != null && rational.Type != "Irrational") return rational;

        // Проверка на высокие корни
        var nthRoot = CheckNthRoot(number);
        if (nthRoot != null) return nthRoot;

        // Представление как √(p/q)
        //var rootResult = CheckRoot(number);
        //bool hasLongDenominator = false;

        //if (rootResult != null)
        //{
        //    // Проверка длину знаменателя
        //    if (rootResult.Denominator != 0)
        //    {
        //        string denomStr = BigInteger.Abs(rootResult.Denominator).ToString();
        //        if (denomStr.Length < 5) // Меньше 5 символов
        //            return rootResult;
        //        // Знаменатель >= 5 символов поиск (далее)
        //        hasLongDenominator = true;
        //    }
        //    else
        //    {
        //        return rootResult; // Не дробь под корнем (целое число)
        //    }
        //}

        //// Если знаменатель длинный, поиск полинома
        //if (hasLongDenominator)
        //{
        //    // биквадратное
        //    var biquadratic = TryAlgebraicDegree(number, 4, tolerance);
        //    if (biquadratic != null) return biquadratic;

        //    // квадратное
        //    var quadratic = TryAlgebraicQuadratic(number, tolerance);
        //    if (quadratic != null) return quadratic;

        //    if (rootResult != null) return rootResult;
        //}

        var algebraic = TryAlgebraicQuadratic(number, tolerance);
        if (algebraic != null) return algebraic;

        //if (rootResult != null) return rootResult;

        var cfResult = TryContinuedFraction(number);
        if (cfResult != null) return cfResult;

        return new ConversionResult
        {
            Type = "Irrational",
            Fraction = null,
            Description = "Иррациональное число или сложный период"
        };
    }

    private static ConversionResult TryAlgebraicQuadratic(double x, double tolerance)
    {
        return TryAlgebraicDegree(x, 2, tolerance);
    }

    private static ConversionResult TryAlgebraicDegree(double x, int degree, double tolerance)
    {
        var coeffs = FindPolynomialCoefficients(x, degree, tolerance);
        if (coeffs != null)
        {
            var result = SolveAndFormat(coeffs, x, tolerance);
            if (result != null) return result;
        }

        return null;
    }

    private static int[] FindPolynomialCoefficients(double x, int degree, double tolerance) => IntegerRelationFinder.FindPolynomial(x, degree, tolerance);


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

    private static ConversionResult SolveAndFormat(int[] coeffs, double targetX, double tolerance)
    {
        int degree = coeffs.Length - 1;

        if (degree == 2)
            return SolveQuadratic(coeffs, targetX, tolerance);
        else if (degree == 3)
            return SolveCubic(coeffs, targetX, tolerance);

        else if (degree == 4)
            return SolveQuartic(coeffs, targetX, tolerance);

        else if (degree >= 5)
            return SolveHighDegree(coeffs, targetX, tolerance, degree);


        return null;
    }

    private static ConversionResult SolveQuadratic(int[] coeffs, double targetX, double tolerance)
    {
        int c = coeffs[0], b = coeffs[1], a = coeffs[2];

        int D = b * b - 4 * a * c;

        if (D < 0) return null;

        int sqrtD = (int)Math.Round(Math.Sqrt(D));
        bool isPerfectSquare = sqrtD * sqrtD == D;

        string sqrtDStr;
        if (isPerfectSquare)
            sqrtDStr = sqrtD.ToString();

        else
        {
            RadicalHelper.SimplifySqrt(D, out int outPart, out int inPart);
            if (outPart == 1)
                sqrtDStr = $"√{inPart}";
            else
                sqrtDStr = $"{outPart}√{inPart}";
        }

        double x1 = (-b + Math.Sqrt(D)) / (2.0 * a);
        double x2 = (-b - Math.Sqrt(D)) / (2.0 * a);

        // Проверка на знаки
        string rootFormula;
        if (Math.Abs(x1 - targetX) < tolerance && Math.Sign(targetX) == Math.Sign(x1))
            rootFormula = FormatQuadraticRoot(-b, sqrtDStr, 2 * a, true);
        else if (Math.Abs(x2 - targetX) < tolerance && Math.Sign(targetX) == Math.Sign(x2))
            rootFormula = FormatQuadraticRoot(-b, sqrtDStr, 2 * a, false);
        else
            return null;

        double calculatedValue = Math.Abs(x1 - targetX) < tolerance ? x1 : x2;
        double error = Math.Abs(targetX - calculatedValue);

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = rootFormula,
            Description = $"Корень уравнения {FormatPolynomial(coeffs)}=0 (погрешность: {error:E3})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static string FormatQuadraticRoot(int numeratorConst, string sqrtPart, int denominator, bool plusSign)
    {
        string sign = plusSign ? "+" : "-";

        int sqrtCoeff = 1;
        string sqrtInner = sqrtPart;

        if (sqrtPart.Contains("√"))
        {
            int sqrtIdx = sqrtPart.IndexOf('√');
            if (sqrtIdx > 0)
            {
                string coeffStr = sqrtPart.Substring(0, sqrtIdx);
                if (int.TryParse(coeffStr, out int parsedCoeff))
                {
                    sqrtCoeff = parsedCoeff;
                    sqrtInner = "√" + sqrtPart.Substring(sqrtIdx + 1);
                }
            }
        }

        int gcd = GCD(GCD(Math.Abs(numeratorConst), sqrtCoeff), Math.Abs(denominator));

        if (gcd > 1)
        {
            numeratorConst /= gcd;
            sqrtCoeff /= gcd;
            denominator /= gcd;
        }

        if (sqrtCoeff == 1)
            sqrtPart = sqrtInner;
        else if (sqrtCoeff == 0)
            sqrtPart = "";
        else
            sqrtPart = sqrtCoeff + sqrtInner;

        if (denominator == 1)
        {
            if (numeratorConst == 0)
            {
                if (string.IsNullOrEmpty(sqrtPart))
                    return "0";
                return plusSign ? sqrtPart : $"-{sqrtPart}";
            }
            else if (string.IsNullOrEmpty(sqrtPart))
                return numeratorConst.ToString();
            else
                return $"{numeratorConst}{sign}{sqrtPart}";
        }
        else
        {
            if (numeratorConst == 0)
            {
                if (string.IsNullOrEmpty(sqrtPart))
                    return "0";
                return plusSign ? $"{sqrtPart}/{denominator}" : $"-{sqrtPart}/{denominator}";
            }
            else if (string.IsNullOrEmpty(sqrtPart))
                return $"{numeratorConst}/{denominator}";
            else
                return $"({numeratorConst}{sign}{sqrtPart})/{denominator}";
        }
    }

    private static ConversionResult SolveCubic(int[] coeffs, double targetX, double tolerance)
    {
        double d = coeffs[0], c = coeffs[1], b = coeffs[2], a = coeffs[3];

        double p = (3 * a * c - b * b) / (3 * a * a);
        double q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);

        double discriminant = -(4 * p * p * p + 27 * q * q);

        if (discriminant > 0)
        {
            double m = 2 * Math.Sqrt(-p / 3);
            double theta = Math.Acos(3 * q / (p * m)) / 3;

            double t1 = m * Math.Cos(theta);
            double t2 = m * Math.Cos(theta - 2 * Math.PI / 3);
            double t3 = m * Math.Cos(theta - 4 * Math.PI / 3);

            double shift = -b / (3 * a);
            double x1 = t1 + shift;
            double x2 = t2 + shift;
            double x3 = t3 + shift;

            if (Math.Abs(x1 - targetX) < tolerance)
                return FormatCubicRoot(coeffs, x1, "x₁");
            if (Math.Abs(x2 - targetX) < tolerance)
                return FormatCubicRoot(coeffs, x2, "x₂");
            if (Math.Abs(x3 - targetX) < tolerance)
                return FormatCubicRoot(coeffs, x3, "x₃");
        }
        else
        {
            double delta = q * q / 4 + p * p * p / 27;
            double u = Math.Pow(-q / 2 + Math.Sqrt(delta), 1.0 / 3.0);
            double v = Math.Pow(-q / 2 - Math.Sqrt(delta), 1.0 / 3.0);
            double t = u + v;
            double x = t - b / (3 * a);

            if (Math.Abs(x - targetX) < tolerance)
                return FormatCubicRoot(coeffs, x, "x");
        }

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = $"Корень полинома {FormatPolynomial(coeffs)}",
            Description = $"Алгебраическое число 3-й степени",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static ConversionResult FormatCubicRoot(int[] coeffs, double rootValue, string rootLabel)
    {
        double d = coeffs[0], c = coeffs[1], b = coeffs[2], a = coeffs[3];

        if (Math.Abs(b) < 1e-10 && Math.Abs(c) < 1e-10 && a == 1)
        {
            int cubeRoot = (int)Math.Round(Math.Pow(-d, 1.0 / 3.0));
            if (Math.Abs(Math.Pow(cubeRoot, 3) + d) < 1e-10)
            {
                return new ConversionResult
                {
                    Type = "Algebraic",
                    Fraction = cubeRoot.ToString(),
                    Description = $"Корень кубический из {-d}",
                    Numerator = 0,
                    Denominator = 0
                };
            }

            SimplifyCubeRoot((int)-d, out int outPart, out int inPart);
            string cubeRootStr = outPart == 1 ? $"∛{inPart}" : $"{outPart}∛{inPart}";

            return new ConversionResult
            {
                Type = "Algebraic",
                Fraction = cubeRootStr,
                Description = $"Корень кубический",
                Numerator = 0,
                Denominator = 0
            };
        }

        string polyStr = FormatPolynomial(coeffs);

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = $"{rootLabel} уравнения {polyStr}=0",
            Description = $"Алгебраическое число 3-й степени (корень ≈ {rootValue:F10})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static void SimplifyCubeRoot(int n, out int outPart, out int inPart)
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

    private static ConversionResult SolveQuartic(int[] coeffs, double targetX, double tolerance)
    {
        int e = coeffs[0], d = coeffs[1], c = coeffs[2], b = coeffs[3], a = coeffs[4];

        if (b == 0 && d == 0)
            return SolveBiquadratic(a, c, e, targetX, tolerance);

        return SolveQuarticFerrari(coeffs, targetX, tolerance);
    }

    private static ConversionResult SolveBiquadratic(int a, int c, int e, double targetX, double tolerance)
    {
        int D = c * c - 4 * a * e;
        if (D < 0)
        {
            return new ConversionResult
            {
                Type = "Algebraic",
                Fraction = $"Корень уравнения {a}x⁴{(c >= 0 ? "+" : "")}{c}x²{(e >= 0 ? "+" : "")}{e}=0",
                Description = "Комплексные корни",
                Numerator = 0,
                Denominator = 0
            };
        }

        double y1 = (-c + Math.Sqrt(D)) / (2.0 * a);
        double y2 = (-c - Math.Sqrt(D)) / (2.0 * a);

        double[] possibleRoots = new double[4];
        int rootCount = 0;

        if (y1 >= 0)
        {
            possibleRoots[rootCount++] = Math.Sqrt(y1);
            possibleRoots[rootCount++] = -Math.Sqrt(y1);
        }
        if (y2 >= 0 && Math.Abs(y2 - y1) > 1e-10)
        {
            possibleRoots[rootCount++] = Math.Sqrt(y2);
            possibleRoots[rootCount++] = -Math.Sqrt(y2);
        }

        for (int i = 0; i < rootCount; i++)
        {
            if (Math.Abs(possibleRoots[i] - targetX) < tolerance && Math.Sign(targetX) == Math.Sign(possibleRoots[i]))
            {
                return FormatBiquadraticRoot(a, c, e, D, possibleRoots[i], targetX, tolerance);
            }
        }

        return null;
    }

    private static ConversionResult FormatBiquadraticRoot(int a, int c, int e, int D, double root, double targetX, double tolerance)
    {
        double y = root * root;

        double y1 = (-c + Math.Sqrt(D)) / (2.0 * a);
        double y2 = (-c - Math.Sqrt(D)) / (2.0 * a);

        bool useY1 = Math.Abs(y - y1) < Math.Abs(y - y2);
        bool positive = root > 0;
        RadicalHelper.SimplifySqrt(D, out int sqrtDOut, out int sqrtDIn);

        if (a == 1)
        {
            int constPart = -c;
            int sqrtCoeff = useY1 ? sqrtDOut : -sqrtDOut;
            int den = 2 * a;

            var decomp = TryDecomposeNestedSqrtAdvanced(constPart, sqrtCoeff, sqrtDIn, den, targetX);
            if (decomp != null)
            {
                double error = Math.Abs(targetX - root);
                return new ConversionResult
                {
                    Type = "Algebraic",
                    Fraction = decomp,
                    Description = $"Биквадратное уравнение {a}x⁴{(c >= 0 ? "+" : "")}{c}x²{(e >= 0 ? "+" : "")}{e}=0 (погрешность: {error:E3})",
                    Numerator = 0,
                    Denominator = 0
                };
            }
        }

        string sign1 = positive ? "" : "-";
        string sign2 = useY1 ? "+" : "-";

        string sqrtDStr = sqrtDIn == 1 ? sqrtDOut.ToString() :
                         sqrtDOut == 1 ? $"√{sqrtDIn}" : $"{sqrtDOut}√{sqrtDIn}";

        string innerExpr;
        int yDen = 2 * a;
        if (a == 1 && yDen == 2)
        {
            innerExpr = $"√(({-c}{sign2}{sqrtDStr})/2)";
        }
        else
        {
            innerExpr = $"√(({-c}{sign2}{sqrtDStr})/{2 * a})";
        }

        string formula = sign1 + innerExpr;
        double error2 = Math.Abs(targetX - root);

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = formula,
            Description = $"Биквадратное уравнение {a}x⁴{(c >= 0 ? "+" : "")}{c}x²{(e >= 0 ? "+" : "")}{e}=0 (погрешность: {error2:E3})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static string TryDecomposeNestedSqrtAdvanced(int A, int B, int C, int D, double targetX)
    {
        if (B > 0)
        {
            for (int m = 1; m <= 20; m++)
            {
                for (int n = 1; n <= 20; n++)
                {
                    if (m >= n) continue;

                    if (A != D * (m + n)) continue;

                    if (B * B * C != 4 * m * n * D * D) continue;

                    double x1 = Math.Sqrt(m) + Math.Sqrt(n);
                    if (Math.Sign(targetX) == Math.Sign(x1))
                        return $"√{m}+√{n}";
                    else return $"-√{m}-√{n}";
                }
            }
        }

        if (B < 0)
        {
            for (int m = 1; m <= 20; m++)
            {
                for (int n = 1; n <= 20; n++)
                {
                    if (m <= n) continue;
                    if (A != D * (m + n)) continue;
                    if (B * B * C != 4 * m * n * D * D) continue;

                    double x1 = Math.Sqrt(m) - Math.Sqrt(n);
                    if (Math.Sign(targetX) == Math.Sign(x1))
                        return $"√{m}-√{n}";
                    else return $"√{n}-√{m}";

                }
            }

            for (int m = 1; m <= 20; m++)
            {
                for (int n = 1; n <= 20; n++)
                {
                    if (n >= m) continue;
                    if (A != D * (m + n)) continue;
                    if (B * B * C != 4 * m * n * D * D) continue;

                    double x1 = Math.Sqrt(n) - Math.Sqrt(m);
                    if (Math.Sign(targetX) == Math.Sign(x1))
                        return $"√{n}-√{m}";
                    else return $"√{m}-√{n}";

                }
            }
        }

        return null;
    }


    private static ConversionResult SolveQuarticFerrari(int[] coeffs, double targetX, double tolerance)
    {
        double e = coeffs[0], d = coeffs[1], c = coeffs[2], b = coeffs[3], a = coeffs[4];

        double p = (8 * a * c - 3 * b * b) / (8 * a * a);
        double q = (b * b * b - 4 * a * b * c + 8 * a * a * d) / (8 * a * a * a);
        double r = (-3 * b * b * b * b + 256 * a * a * a * e - 64 * a * a * b * d + 16 * a * b * b * c) / (256 * a * a * a * a);

        double[] resolventCoeffs = new double[4];
        resolventCoeffs[0] = -q * q;
        resolventCoeffs[1] = p * p - 4 * r;
        resolventCoeffs[2] = 2 * p;
        resolventCoeffs[3] = 1;

        double y = SolveResolventCubic(resolventCoeffs);

        if (double.IsNaN(y))
        {
            double error = Math.Abs(EvaluatePolynomial(coeffs, targetX));
            return new ConversionResult
            {
                Type = "Algebraic",
                Fraction = $"Корень полинома {FormatPolynomial(coeffs)}",
                Description = $"Алгебраическое число 4-й степени (погрешность: {error:E3})",
                Numerator = 0,
                Denominator = 0
            };
        }

        double sqrtTerm = Math.Sqrt(2 * y - p);

        double a1 = 1;
        double b1 = sqrtTerm;
        double c1 = y - q / (2 * sqrtTerm);

        double a2 = 1;
        double b2 = -sqrtTerm;
        double c2 = y + q / (2 * sqrtTerm);

        double[] roots = new double[4];
        int rootCount = 0;

        double disc1 = b1 * b1 - 4 * a1 * c1;
        if (disc1 >= 0)
        {
            roots[rootCount++] = (-b1 + Math.Sqrt(disc1)) / (2 * a1) - b / (4 * a);
            roots[rootCount++] = (-b1 - Math.Sqrt(disc1)) / (2 * a1) - b / (4 * a);
        }

        double disc2 = b2 * b2 - 4 * a2 * c2;
        if (disc2 >= 0)
        {
            roots[rootCount++] = (-b2 + Math.Sqrt(disc2)) / (2 * a2) - b / (4 * a);
            roots[rootCount++] = (-b2 - Math.Sqrt(disc2)) / (2 * a2) - b / (4 * a);
        }

        for (int i = 0; i < rootCount; i++)
        {
            if (Math.Abs(roots[i] - targetX) < tolerance)
            {
                double error = Math.Abs(targetX - roots[i]);
                return FormatQuarticRoot(coeffs, roots[i], $"x_{i + 1}", error);
            }
        }

        double minError = double.MaxValue;
        for (int i = 0; i < rootCount; i++)
        {
            double err = Math.Abs(roots[i] - targetX);
            if (err < minError) minError = err;
        }

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = $"Корень полинома {FormatPolynomial(coeffs)}",
            Description = $"Алгебраическое число 4-й степени (погрешность: {minError:E3})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static double SolveResolventCubic(double[] coeffs)
    {
        double x = 1.0;
        for (int i = 0; i < 100; i++)
        {
            double f = coeffs[3] * x * x * x + coeffs[2] * x * x + coeffs[1] * x + coeffs[0];
            double df = 3 * coeffs[3] * x * x + 2 * coeffs[2] * x + coeffs[1];

            if (Math.Abs(df) < 1e-15) break;

            double xNew = x - f / df;
            if (Math.Abs(xNew - x) < 1e-12) return xNew;
            x = xNew;
        }
        return x;
    }

    private static ConversionResult FormatQuarticRoot(int[] coeffs, double rootValue, string rootLabel, double error)
    {
        string polyStr = FormatPolynomial(coeffs);

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = $"{rootLabel} уравнения {polyStr}=0",
            Description = $"Алгебраическое число 4-й степени (корень ≈ {rootValue:F10}, погрешность: {error:E3})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static ConversionResult SolveHighDegree(int[] coeffs, double targetX, double tolerance, int degree)
    {
        double error = Math.Abs(EvaluatePolynomial(coeffs, targetX));
        if (error > tolerance)
            return null;

        if (degree == 6)
        {
            var decomposed = TryDecomposeDegree6(coeffs, targetX, tolerance);
            if (decomposed != null) return decomposed;
        }

        string polyStr = FormatPolynomial(coeffs);

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = $"Корень уравнения {polyStr}=0",
            Description = $"Алгебраическое число {degree}-й степени (≈ {targetX:F10})",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static ConversionResult TryDecomposeDegree6(int[] coeffs, double targetX, double tolerance)
    {
        for (int cubeNum = -20; cubeNum <= 20; cubeNum++)
        {
            if (cubeNum == 0) continue;

            for (int cubeDen = 1; cubeDen <= 20; cubeDen++)
            {
                double cubeRootPart = Math.Pow((double)cubeNum / cubeDen, 1.0 / 3.0);
                double remainder = targetX - cubeRootPart;

                double remainderSquared = remainder * remainder;

                for (int sqNum = 1; sqNum <= 50; sqNum++)
                {
                    for (int sqDen = 1; sqDen <= 20; sqDen++)
                    {
                        double sqrtVal = Math.Sqrt((double)sqNum / sqDen);

                        if (Math.Abs(remainder - sqrtVal) < tolerance)
                            return FormatMixedRadical(cubeNum, cubeDen, sqNum, sqDen, true);

                        if (Math.Abs(remainder + sqrtVal) < tolerance)
                            return FormatMixedRadical(cubeNum, cubeDen, sqNum, sqDen, false);
                    }
                }
            }
        }

        return null;
    }

    private static ConversionResult FormatMixedRadical(int cubeNum, int cubeDen, int sqNum, int sqDen, bool plusSign)
    {

        string cubeRootStr;
        if (cubeDen == 1)
            cubeRootStr = RadicalHelper.SimplifyNthRoot(cubeNum, 3);
        else
        {
            int gcd = GCD(Math.Abs(cubeNum), cubeDen);
            int simpleCubeNum = cubeNum / gcd;
            int simpleCubeDen = cubeDen / gcd;
            cubeRootStr = simpleCubeDen == 1
                ? RadicalHelper.SimplifyNthRoot(simpleCubeNum, 3)
                : $"∛({simpleCubeNum}/{simpleCubeDen})";
        }

        string sqrtStr;
        if (sqDen == 1)
        {
            RadicalHelper.SimplifySqrt(sqNum, out int outPart, out int inPart);
            if (inPart == 1)
                sqrtStr = outPart.ToString();
            else if (outPart == 1)
                sqrtStr = $"√{inPart}";
            else
                sqrtStr = $"{outPart}√{inPart}";
        }
        else
        {
            int gcd = GCD(sqNum, sqDen);
            int simpleSqNum = sqNum / gcd;
            int simpleSqDen = sqDen / gcd;

            RadicalHelper.SimplifySqrt(simpleSqNum, out int numOut, out int numIn);
            RadicalHelper.SimplifySqrt(simpleSqDen, out int denOut, out int denIn);

            if (numIn == 1 && denIn == 1)
            {
                sqrtStr = denOut == 1 ? numOut.ToString() : $"{numOut}/{denOut}";
            }
            else if (denIn == 1)
            {
                string numStr = numOut == 1 ? $"√{numIn}" : $"{numOut}√{numIn}";
                sqrtStr = denOut == 1 ? numStr : $"{numStr}/{denOut}";
            }
            else
            {
                sqrtStr = $"√({simpleSqNum}/{simpleSqDen})";
            }
        }

        string sign = plusSign ? "+" : "-";
        string result = $"{cubeRootStr}{sign}{sqrtStr}";

        return new ConversionResult
        {
            Type = "Algebraic",
            Fraction = result,
            Description = $"Алгебраическое число 6-й степени (смешанные радикалы)",
            Numerator = 0,
            Denominator = 0
        };
    }

    private static string FormatPolynomial(int[] coeffs)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = coeffs.Length - 1; i >= 0; i--)
        {
            int c = coeffs[i];
            if (c == 0) continue;

            if (sb.Length > 0 && c > 0) sb.Append("+");

            if (i == 0)
            {
                sb.Append(c);
            }
            else if (i == 1)
            {
                if (c == 1) sb.Append("x");
                else if (c == -1) sb.Append("-x");
                else sb.Append($"{c}x");
            }
            else
            {
                if (c == 1) sb.Append($"x^{i}");
                else if (c == -1) sb.Append($"-x^{i}");
                else sb.Append($"{c}x^{i}");
            }
        }
        return sb.ToString();
    }

    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private static ConversionResult DetectRational(double number)
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
                {
                    requiredCycles = Math.Max(9, minCycles);
                }

                if (prePeriodLen + periodLen * requiredCycles > digits.Length)
                    continue;

                string prePeriod = digits.Substring(0, prePeriodLen);
                string candidatePeriod = digits.Substring(prePeriodLen, periodLen);

                if (CheckCycleWithTolerance(digits, prePeriodLen, candidatePeriod, requiredCycles))
                {
                    return CreateRepeatingResult(prePeriod, candidatePeriod, integerPart, isNegative);
                }
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
                if (period.StartsWith(remaining) || IsRoundingError(remaining, period.Substring(0, remaining.Length)))
                {
                    // OK
                }
                else
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

        StringBuilder denStr = new StringBuilder();
        for (int k = 0; k < period.Length; k++) denStr.Append('9');
        for (int k = 0; k < prePeriod.Length; k++) denStr.Append('0');

        BigInteger denominatorPart = BigInteger.Parse(denStr.ToString());

        BigInteger finalNum = BigInteger.Abs(integerPart) * denominatorPart + numeratorPart;
        if (isNegative) finalNum = -finalNum;

        Simplify(ref finalNum, ref denominatorPart);

        return new ConversionResult
        {
            Type = "Repeating",
            Fraction = $"{finalNum}/{denominatorPart}",
            Description = prePeriod == ""
                ? $"Периодическая дробь 0.({period})"
                : $"Периодическая дробь 0.{prePeriod}({period})",
            Numerator = finalNum,
            Denominator = denominatorPart
        };
    }

    private static ConversionResult CheckNthRoot(double number)
    {
        double tolerance = 1e-10;


        if (RadicalHelper.IsNthRoot(number, 2, tolerance, out int qRadicand))
        {
            string simplified = RadicalHelper.SimplifyNthRoot(qRadicand, 3);
            return new ConversionResult
            {
                Type = "Root",
                Fraction = simplified,
                Description = $"Кубический корень из {qRadicand}",
                Numerator = 0,
                Denominator = 0
            };
        }

        if (RadicalHelper.IsNthRoot(number, 3, tolerance, out int cubeRadicand))
        {
            string simplified = RadicalHelper.SimplifyNthRoot(cubeRadicand, 3);
            return new ConversionResult
            {
                Type = "Root",
                Fraction = simplified,
                Description = $"Кубический корень из {cubeRadicand}",
                Numerator = 0,
                Denominator = 0
            };
        }

        if (RadicalHelper.IsNthRoot(number, 4, tolerance, out int fourthRadicand))
        {
            int sqrtRadicand = (int)Math.Round(Math.Sqrt(fourthRadicand));
            if (sqrtRadicand * sqrtRadicand == fourthRadicand)
            {
                // Это квадратный корень
            }
            else
            {
                string simplified = RadicalHelper.SimplifyNthRoot(fourthRadicand, 4);
                return new ConversionResult
                {
                    Type = "Root",
                    Fraction = simplified,
                    Description = $"Корень 4-й степени из {fourthRadicand}",
                    Numerator = 0,
                    Denominator = 0
                };
            }
        }

        if (RadicalHelper.IsNthRoot(number, 5, tolerance, out int fifthRadicand))
        {
            string simplified = RadicalHelper.SimplifyNthRoot(fifthRadicand, 5);
            return new ConversionResult
            {
                Type = "Root",
                Fraction = simplified,
                Description = $"Корень 5-й степени из {fifthRadicand}",
                Numerator = 0,
                Denominator = 0
            };
        }

        return null;
    }

    private static ConversionResult CheckRoot(double number)
    {
        double square = number * number;
        double tolerance = 1e-10;

        double roundedSquare = Math.Round(square);
        if (Math.Abs(square - roundedSquare) < tolerance)
        {
            BigInteger val = (BigInteger)roundedSquare;
            return CreateRootResult(val, 1, number < 0);
        }

        var ratResult = DetectRational(square);

        // Убираю огромные дроби
        if(ratResult != null)
        if ((double)ratResult.Denominator > 1e+5)
            return null;

        if (ratResult != null && ratResult.Type != "Irrational")
        {
            return CreateRootResult(ratResult.Numerator, ratResult.Denominator, number < 0);
        }

        var cfResult = TryContinuedFraction(square);

        // Убираю огромные дроби
        if (cfResult != null)
            if ((double)cfResult.Denominator > 1e+5)
                return null;

        if (cfResult != null && cfResult.Type != "Irrational")
        {
            return CreateRootResult(cfResult.Numerator, cfResult.Denominator, number < 0);
        }

        return null;
    }

    private static ConversionResult CreateRootResult(BigInteger num, BigInteger den, bool isNegative)
    {
        Simplify(ref num, ref den);

        ExtractSquare(num, out BigInteger numOut, out BigInteger numIn);
        ExtractSquare(den, out BigInteger denOut, out BigInteger denIn);

        string sign = isNegative ? "-" : "";
        string prefix = "";

        if (numOut != 1 || denOut != 1)
        {
            if (denOut == 1) prefix = $"{numOut}";
            else prefix = $"{numOut}/{denOut}";
        }

        string rootPart;
        if (denIn == 1)
        {
            if (numIn == 1) rootPart = "";
            else rootPart = $"√{numIn}";
        }
        else
        {
            rootPart = $"√({numIn}/{denIn})";
        }

        string finalStr;
        if (string.IsNullOrEmpty(prefix)) finalStr = string.IsNullOrEmpty(rootPart) ? "1" : rootPart;
        else finalStr = string.IsNullOrEmpty(rootPart) ? prefix : prefix == "1" ? rootPart : $"{prefix}*{rootPart}";

        if (isNegative) finalStr = "-" + finalStr;

        return new ConversionResult
        {
            Type = "Root",
            Fraction = finalStr,
            Description = $"Корень из числа {num}/{den}",
            Numerator = numIn,
            Denominator = denIn
        };
    }

    private static void ExtractSquare(BigInteger val, out BigInteger outPart, out BigInteger inPart)
    {
        outPart = 1;
        inPart = val;

        for (long i = 2; i < 100; i++)
        {
            BigInteger sq = i * i;
            while (inPart % sq == 0)
            {
                inPart /= sq;
                outPart *= i;
            }
        }
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

    private static ConversionResult CreateTerminating(string digits, BigInteger integerPart, bool isNegative)
    {
        if (string.IsNullOrEmpty(digits))
        {
            return new ConversionResult
            {
                Type = "Integer",
                Fraction = $"{integerPart}/1",
                Description = "Целое число",
                Numerator = integerPart,
                Denominator = 1
            };
        }

        BigInteger den = BigInteger.Pow(10, digits.Length);
        BigInteger num = BigInteger.Parse(digits);

        BigInteger totalNum = BigInteger.Abs(integerPart) * den + num;
        if (isNegative) totalNum = -totalNum;

        Simplify(ref totalNum, ref den);

        return new ConversionResult
        {
            Type = "Terminating",
            Fraction = $"{totalNum}/{den}",
            Description = "Конечная дробь",
            Numerator = totalNum,
            Denominator = den
        };
    }

    private static ConversionResult TryContinuedFraction(double number)
    {
        long maxDen = 1000000000;
        double epsilon = 1e-10;

        double x = Math.Abs(number);
        long p0 = 0, p1 = 1, q0 = 1, q1 = 0;

        long a = (long)x;
        long p2 = a * p1 + p0;
        long q2 = a * q1 + q0;

        long p_2 = 0, p_1 = 1;
        long q_2 = 1, q_1 = 0;
        long p = 0, q = 1;
        double r = x;

        for (int i = 0; i < 100; i++)
        {
            long ai = (long)r;
            p = ai * p_1 + p_2;
            q = ai * q_1 + q_2;

            if (q > maxDen || q < 0)
            {
                p = p_1;
                q = q_1;
                break;
            }

            double approx = (double)p / q;
            if (Math.Abs(x - approx) < epsilon)
            {
                if (number < 0) p = -p;

                

                return new ConversionResult
                {
                    Type = "Repeating",
                    Fraction = q > 1e+4? null: $"{p}/{q}",
                    Description = "Приближение (непрерывная дробь)",
                    Numerator = p,
                    Denominator = q
                };
            }

            p_2 = p_1; q_2 = q_1;
            p_1 = p; q_1 = q;

            if (Math.Abs(r - ai) < 1e-15) break;
            r = 1.0 / (r - ai);
        }

        return null;
    }

    private static void Simplify(ref BigInteger num, ref BigInteger den)
    {
        BigInteger gcd = GCD_BigInt(BigInteger.Abs(num), den);
        num /= gcd;
        den /= gcd;
    }

    private static BigInteger GCD_BigInt(BigInteger a, BigInteger b)
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
