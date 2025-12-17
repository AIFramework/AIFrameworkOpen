using AI.ClassicMath.MatrixUtils.FindFraction;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;



[Serializable]
public class Processor
{
    public readonly AdvancedCalculator AdvancedCalculator = new AdvancedCalculator();

    public List<string> Run(string script, CancellationToken cancellationToken = default)
    {
        var context = new ExecutionContext();
        var output = new List<string>();
        try
        {
            var lines = script.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var lineQueue = new Queue<string>(lines);
            var statements = ParseStatements(lineQueue, isSilentContext: false, cancellationToken);
            foreach (var statement in statements)
            {
                cancellationToken.ThrowIfCancellationRequested();
                statement.Execute(this, context, output, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            output.Add("ВЫПОЛНЕНИЕ ПРЕРВАНО: Операция была отменена");
        }
        catch (Exception ex)
        {
            output.Add($"!!! КРИТИЧЕСКАЯ ОШИБКА: {ex.GetType().Name} -> {ex.Message}");
        }
        return output;
    }

    #region Парсер скрипта

    private List<Statement> ParseStatements(Queue<string> lines, bool isSilentContext, CancellationToken cancellationToken = default)
    {
        var statements = new List<Statement>();
        while (lines.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var rawLine = lines.Dequeue();

            // Находим комментарий.
            int commentIndex = rawLine.IndexOf("//");
            // Обрезаем строку, если комментарий найден, иначе берем ее целиком.
            string line = commentIndex >= 0 ? rawLine.Substring(0, commentIndex) : rawLine;
            // Убираем пробелы.
            line = line.Trim();

            // Если после обрезки ничего не осталось, пропускаем строку.
            if (string.IsNullOrWhiteSpace(line)) continue;

            var ifMatch = Regex.Match(line, @"^if\s*\((.*)\)\s*\{?$");
            var whileMatch = Regex.Match(line, @"^while\s*\((.*)\)\s*\{?$");
            var forMatch = Regex.Match(line, @"^for\s*\((.*);(.*);(.*)\)\s*\{?$");

            if (ifMatch.Success)
            {
                var condition = ifMatch.Groups[1].Value;
                var trueBody = ParseBlock(lines, cancellationToken);
                List<Statement> falseBody = null;
                if (lines.Count > 0 && lines.Peek().Trim().StartsWith("else"))
                {
                    lines.Dequeue();
                    falseBody = ParseBlock(lines, cancellationToken);
                }
                statements.Add(new IfStatement(condition, trueBody, falseBody));
            }
            else if (whileMatch.Success)
            {
                var condition = whileMatch.Groups[1].Value;
                var body = ParseBlock(lines, cancellationToken);
                statements.Add(new WhileStatement(condition, body));
            }
            else if (forMatch.Success)
            {
                var initializer = forMatch.Groups[1].Value;
                var condition = forMatch.Groups[2].Value;
                var increment = forMatch.Groups[3].Value;
                var body = ParseBlock(lines, cancellationToken);
                statements.Add(new ForStatement(initializer, condition, increment, body));
            }
            else if (line == "{" || line.StartsWith("else")) { throw new ArgumentException($"Неожиданный токен: '{line}'"); }
            else { statements.Add(new ExpressionStatement(line, isSilentContext)); }
        }
        return statements;
    }

    private List<Statement> ParseBlock(Queue<string> lines, CancellationToken cancellationToken = default)
    {
        var blockLines = new Queue<string>();
        int braceLevel = 1;
        if (lines.Count > 0 && lines.Peek().Trim() == "{") lines.Dequeue();
        while (lines.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var line = lines.Dequeue();

            // Игнорируем скобки внутри комментариев
            int commentIndex = line.IndexOf("//");
            string codePart = commentIndex >= 0 ? line.Substring(0, commentIndex) : line;

            if (codePart.Contains("{")) braceLevel++;
            if (codePart.Contains("}")) braceLevel--;

            if (braceLevel == 0)
            {
                blockLines.Enqueue(line.Substring(0, line.LastIndexOf('}')));
                break;
            }
            blockLines.Enqueue(line);
        }
        if (braceLevel != 0) throw new ArgumentException("Нарушен баланс скобок '{}' в блоке кода.");
        return ParseStatements(blockLines, isSilentContext: true, cancellationToken);
    }
    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Проверка истинности
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool IsTruthy(object result)
    {
        return result switch
        {
            double d when !double.IsNaN(d) => d != 0,
            Complex c when c != null => c != Complex.Zero,
            Vector v => v.Count > 0,
            ComplexVector cv => cv.Count > 0,
            // Все остальное (ошибки-строки, null, NaN) считается ложью
            _ => false
        };
    }

    /// <summary>
    /// Форматирование ответа с поддержкой символьных выражений
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string FormatResult(object result)
    {
        return result switch
        {
            double d => FormatDouble(d),
            Complex c => FormatComplex(c),
            ComplexVector v => $"[{string.Join(", ", v.Select(c => FormatResult(c)))}]",
            Vector dv => $"[{string.Join(", ", dv.Select(c => FormatDouble(c)))}]",
            _ => result?.ToString() ?? "null"
        };
    }
    
    /// <summary>
    /// Форматирование вещественного числа с попыткой преобразования в дробь или символьную форму
    /// </summary>
    private static string FormatDouble(double d)
    {
        // Проверяем на специальные значения
        if (double.IsNaN(d)) return "NaN";
        if (double.IsPositiveInfinity(d)) return "∞";
        if (double.IsNegativeInfinity(d)) return "-∞";
        
        // Если число целое
        if (Math.Abs(d - Math.Round(d)) < 1e-10)
        {
            return ((long)Math.Round(d)).ToString(CultureInfo.InvariantCulture);
        }
        
        // Сначала проверяем простые дроби (приоритет!)
        string fraction = TryConvertToSymbolyc(d);
        if (!string.IsNullOrEmpty(fraction))
            return $"{d.ToString("G9", CultureInfo.InvariantCulture)}  [{fraction}]";
        
        //// Затем проверяем известные математические константы (корни и т.д.)
        //string symbolic = KnownConstants.TryGetSymbolicForm(d);
        //if (symbolic != null)
        //{
        //    return $"{d.ToString("G9", CultureInfo.InvariantCulture)}  [{symbolic}]";
        //}
        
        return d.ToString("G9", CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Форматирование комплексного числа
    /// </summary>
    private static string FormatComplex(Complex c)
    {
        if (Math.Abs(c.Imaginary) < 1e-12)
        {
            // Только действительная часть
            return FormatDouble(c.Real);
        }
        
        return $"{c.Real.ToString("G9", CultureInfo.InvariantCulture)} + {c.Imaginary.ToString("G9", CultureInfo.InvariantCulture)}i";
    }

    /// <summary>
    /// Попытка преобразовать число в дробь (оптимизированная версия)
    /// </summary>
    private static string TryConvertToFraction(double value, int maxDenominator = 100)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return null;

        // Сохраняем знак
        int sign = value < 0 ? -1 : 1;
        value = Math.Abs(value);

        // Быстрая проверка на простые дроби (оптимизация!)
        double bestError = 1e-9;
        int bestNum = 0, bestDen = 1;

        for (int denominator = 2; denominator <= maxDenominator; denominator++)
        {
            int numerator = (int)Math.Round(value * denominator);
            double error = Math.Abs((double)numerator / denominator - value);

            if (error < bestError)
            {
                bestError = error;
                bestNum = numerator;
                bestDen = denominator;

                // Если нашли точное совпадение, сразу возвращаем
                if (error < 1e-12)
                    break;
            }
        }

        if (bestError < 1e-9)
        {
            bestNum *= sign;

            // Упрощаем дробь
            int gcd = GCD(Math.Abs(bestNum), bestDen);
            bestNum /= gcd;
            bestDen /= gcd;

            return $"{bestNum}/{bestDen}";
        }

        return null;
    }


    /// <summary>
    /// Попытка преобразовать число в дробь (оптимизированная версия)
    /// </summary>
    private static string TryConvertToSymbolyc(double value)
    {
        var analyzeResult = NumberConverter.Analyze(value);
        return analyzeResult.Fraction;
    }

    /// <summary>
    /// Наибольший общий делитель (внутренний метод для дробей)
    /// </summary>
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
    
    /// <summary>
    /// Наибольший общий делитель (НОД) для long чисел - публичный метод
    /// </summary>
    public static long GCDLong(long a, long b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    
    /// <summary>
    /// Наименьшее общее кратное (НОК) для long чисел
    /// </summary>
    public static long LCM(long a, long b)
    {
        if (a == 0 || b == 0)
            return 0;
            
        a = Math.Abs(a);
        b = Math.Abs(b);
        
        return (a / GCDLong(a, b)) * b;
    }
    
    /// <summary>
    /// Преобразует вещественное число в дробь (числитель/знаменатель)
    /// </summary>
    private static (long numerator, long denominator) ToFraction(double value, int maxDenominator = 10000)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Невозможно преобразовать NaN или бесконечность в дробь");
        
        // Сохраняем знак
        int sign = value < 0 ? -1 : 1;
        value = Math.Abs(value);
        
        // Если число целое
        if (Math.Abs(value - Math.Round(value)) < 1e-10)
        {
            return ((long)(sign * Math.Round(value)), 1);
        }
        
        // Алгоритм непрерывных дробей (метод Евклида)
        long h1 = 1, h2 = 0;
        long k1 = 0, k2 = 1;
        double b = value;
        
        do
        {
            long a = (long)b;
            long aux = h1;
            h1 = a * h1 + h2;
            h2 = aux;
            aux = k1;
            k1 = a * k1 + k2;
            k2 = aux;
            b = 1 / (b - a);
        } while (Math.Abs(value - (double)h1 / k1) > 1e-10 && k1 <= maxDenominator && !double.IsInfinity(b));
        
        return (sign * h1, k1);
    }
    
    /// <summary>
    /// Наибольший общий делитель (НОД) для дробных чисел
    /// </summary>
    public static double GCDDouble(double a, double b)
    {
        if (a == 0) return Math.Abs(b);
        if (b == 0) return Math.Abs(a);
        
        // Преобразуем числа в дроби
        var (num1, den1) = ToFraction(a);
        var (num2, den2) = ToFraction(b);
        
        // НОД(a/b, c/d) = НОД(a, c) / НОК(b, d)
        long gcdNumerators = GCDLong(num1, num2);
        long lcmDenominators = LCM(den1, den2);
        
        return (double)gcdNumerators / lcmDenominators;
    }
    
    /// <summary>
    /// Наименьшее общее кратное (НОК) для дробных чисел
    /// </summary>
    public static double LCMDouble(double a, double b)
    {
        if (a == 0 || b == 0)
            return 0;
        
        // Преобразуем числа в дроби
        var (num1, den1) = ToFraction(a);
        var (num2, den2) = ToFraction(b);
        
        // НОК(a/b, c/d) = НОК(a, c) / НОД(b, d)
        long lcmNumerators = LCM(num1, num2);
        long gcdDenominators = GCDLong(den1, den2);
        
        return (double)lcmNumerators / gcdDenominators;
    }
    #endregion
}