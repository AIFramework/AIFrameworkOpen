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
            // УЛУЧШЕНИЕ 1: Поддержка точки с запятой (;) как разделителя выражений
            // Сначала разбиваем по точке с запятой, сохраняя переносы строк
            script = PreprocessScript(script);
            
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
    
    /// <summary>
    /// Предобработка скрипта: разбивает точки с запятой на отдельные строки
    /// </summary>
    private string PreprocessScript(string script)
    {
        // Защищаем строковые литералы от изменения
        var strings = new List<string>();
        var stringPattern = @"""(?:[^""\\]|\\.)*""";
        script = Regex.Replace(script, stringPattern, m =>
        {
            strings.Add(m.Value);
            return $"__STRING_{strings.Count - 1}__";
        });
        
        // УЛУЧШЕНИЕ 6: Разбиваем inline-блоки if(...){...}else{...} на многострочные
        // Просто добавляем переводы строк перед/после { и }
        script = script.Replace("{", "\n{\n");
        script = script.Replace("}", "\n}\n");
        
        // Теперь заменяем ; на \n (только вне строк и вне for(...;...;...))
        // Защищаем for(...;...;...) от разбивки
        var forPattern = @"for\s*\([^)]+\)";
        var forLoops = new List<string>();
        script = Regex.Replace(script, forPattern, m =>
        {
            forLoops.Add(m.Value);
            return $"__FOR_{forLoops.Count - 1}__";
        });
        
        // Заменяем ; на \n
        script = script.Replace(";", "\n");
        
        // Восстанавливаем for-циклы
        for (int i = 0; i < forLoops.Count; i++)
        {
            script = script.Replace($"__FOR_{i}__", forLoops[i]);
        }
        
        // Восстанавливаем строковые литералы
        for (int i = 0; i < strings.Count; i++)
        {
            script = script.Replace($"__STRING_{i}__", strings[i]);
        }
        
        return script;
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

            // Проверка на break и continue
            if (line == "break")
            {
                statements.Add(new BreakStatement());
                continue;
            }
            if (line == "continue")
            {
                statements.Add(new ContinueStatement());
                continue;
            }

            var ifMatch = Regex.Match(line, @"^if\s*\((.*)\)\s*\{?$");
            var ifColonMatch = Regex.Match(line, @"^if\s+(.+):\s*$");  // Python-style: if condition:
            var elifMatch = Regex.Match(line, @"^elif\s+(.+):\s*$");  // Python-style: elif condition:
            var whileMatch = Regex.Match(line, @"^while\s*\((.*)\)\s*\{?$");
            var whileColonMatch = Regex.Match(line, @"^while\s+(.+):\s*$");  // Python-style: while condition:
            // C-style for: for(init; condition; increment) - делаем части опциональными
            var forMatch = Regex.Match(line, @"^for\s*\(([^;]*);([^;]*);([^)]*)\)\s*\{?$");
            var forToMatch = Regex.Match(line, @"^for\s+(\w+)\s*=\s*(.+?)\s+to\s+(.+?)(?:\s+step\s+(.+?))?\s*:\s*$");

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
            else if (ifColonMatch.Success)
            {
                // Python-style if с отступами
                var condition = ifColonMatch.Groups[1].Value;
                var trueBody = ParseIndentedBlock(lines, cancellationToken);
                List<Statement> falseBody = null;
                
                // Рекурсивная обработка elif/else
                falseBody = ParseElifElse(lines, cancellationToken);
                
                statements.Add(new IfStatement(condition, trueBody, falseBody));
            }
            else if (whileMatch.Success)
            {
                var condition = whileMatch.Groups[1].Value;
                var body = ParseBlock(lines, cancellationToken);
                statements.Add(new WhileStatement(condition, body));
            }
            else if (whileColonMatch.Success)
            {
                // Python-style while с отступами
                var condition = whileColonMatch.Groups[1].Value;
                var body = ParseIndentedBlock(lines, cancellationToken);
                statements.Add(new WhileStatement(condition, body));
            }
            else if (forToMatch.Success)
            {
                // Синтаксис: for i = 1 to 100: или for i = 1 to 100 step 2:
                var varName = forToMatch.Groups[1].Value;
                var startExpr = forToMatch.Groups[2].Value;
                var endExpr = forToMatch.Groups[3].Value;
                var stepExpr = forToMatch.Groups[4].Success ? forToMatch.Groups[4].Value : "1";
                
                // Преобразуем в стандартный for
                var initializer = $"{varName} = {startExpr}";
                
                // ИСПРАВЛЕНИЕ: Поддержка отрицательных шагов
                // Условие: (step > 0 && i <= end) || (step < 0 && i >= end)
                // Упрощённо: используем общее условие, которое работает для обоих направлений
                // Если step положительный: i <= end
                // Если step отрицательный: i >= end
                // Универсальное условие: ((i - end) * step) <= 0
                var stepVar = $"__step_{varName}";
                var endVar = $"__end_{varName}";
                var condition = $"(({varName} - {endVar}) * {stepVar}) <= 0";
                var increment = $"{varName} = {varName} + {stepVar}";
                
                // Добавляем инициализацию вспомогательных переменных
                statements.Add(new ExpressionStatement($"{stepVar} = {stepExpr}"));
                statements.Add(new ExpressionStatement($"{endVar} = {endExpr}"));
                
                var body = ParseIndentedBlock(lines, cancellationToken);
                statements.Add(new ForStatement(initializer, condition, increment, body));
            }
            else if (forMatch.Success)
            {
                var initializer = forMatch.Groups[1].Value.Trim();
                var condition = forMatch.Groups[2].Value.Trim();
                var increment = forMatch.Groups[3].Value.Trim();
                
                // Проверяем, есть ли { на той же строке
                if (line.EndsWith("{"))
                {
                    var body = ParseBlock(lines, cancellationToken);
                    statements.Add(new ForStatement(initializer, condition, increment, body));
                }
                else
                {
                    // Однострочное тело или блок на следующей строке
                    var body = ParseBlock(lines, cancellationToken);
                    statements.Add(new ForStatement(initializer, condition, increment, body));
                }
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
                // Проверяем, есть ли else на той же строке после }
                var closingIndex = codePart.LastIndexOf('}');
                var afterClosing = codePart.Substring(closingIndex + 1).Trim();
                
                if (afterClosing.StartsWith("else"))
                {
                    // Возвращаем "else ..." обратно в очередь как отдельную строку
                    var remainingLine = line.Substring(line.LastIndexOf('}') + 1).Trim();
                    if (!string.IsNullOrWhiteSpace(remainingLine))
                    {
                        var newQueue = new List<string> { remainingLine };
                        newQueue.AddRange(lines);
                        lines = new Queue<string>(newQueue);
                    }
                }
                
                // Добавляем всё что было до }
                var beforeClosing = line.Substring(0, line.LastIndexOf('}'));
                if (!string.IsNullOrWhiteSpace(beforeClosing.Trim()))
                {
                    blockLines.Enqueue(beforeClosing);
                }
                break;
            }
            blockLines.Enqueue(line);
        }
        if (braceLevel != 0) throw new ArgumentException("Нарушен баланс скобок '{}' в блоке кода.");
        return ParseStatements(blockLines, isSilentContext: true, cancellationToken);
    }

    private List<Statement> ParseIndentedBlock(Queue<string> lines, CancellationToken cancellationToken = default)
    {
        // Парсинг блока с отступами (Python-style) для циклов 'for i = 1 to N:'
        var blockLines = new Queue<string>();
        
        // Минимальный отступ для определения принадлежности к блоку
        int? blockIndent = null;
        
        while (lines.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var line = lines.Peek();
            
            // Убираем комментарии для проверки отступов
            int commentIndex = line.IndexOf("//");
            string lineWithoutComment = commentIndex >= 0 ? line.Substring(0, commentIndex) : line;
            
            // Если строка пустая или только комментарий - пропускаем
            if (string.IsNullOrWhiteSpace(lineWithoutComment))
            {
                lines.Dequeue();
                continue;
            }
            
            // Определяем отступ текущей строки
            int currentIndent = line.Length - line.TrimStart().Length;
            
            // Устанавливаем базовый отступ блока по первой строке
            if (blockIndent == null)
            {
                blockIndent = currentIndent;
                if (currentIndent == 0)
                {
                    // Если отступа нет, читаем только до первой строки без отступа (кроме текущей)
                    blockLines.Enqueue(lines.Dequeue());
                    break;
                }
            }
            
            // Если отступ меньше базового - блок закончился
            if (currentIndent < blockIndent.Value)
            {
                break;
            }
            
            // Добавляем строку в блок
            blockLines.Enqueue(lines.Dequeue());
        }
        
        return ParseStatements(blockLines, isSilentContext: true, cancellationToken);
    }
    
    private List<Statement> ParseElifElse(Queue<string> lines, CancellationToken cancellationToken = default)
    {
        // Рекурсивная обработка elif/else цепочки
        if (lines.Count == 0) return null;
        
        var nextLine = lines.Peek().Trim();
        
        if (nextLine.StartsWith("elif"))
        {
            lines.Dequeue();
            var elifCondMatch = Regex.Match(nextLine, @"^elif\s+(.+):\s*$");
            if (elifCondMatch.Success)
            {
                var elifCondition = elifCondMatch.Groups[1].Value;
                var elifBody = ParseIndentedBlock(lines, cancellationToken);
                // elif это просто if с возможным продолжением elif/else
                var elifElse = ParseElifElse(lines, cancellationToken);
                var elifStatement = new IfStatement(elifCondition, elifBody, elifElse);
                return new List<Statement> { elifStatement };
            }
        }
        else if (nextLine.StartsWith("else:"))
        {
            lines.Dequeue();
            return ParseIndentedBlock(lines, cancellationToken);
        }
        
        return null;
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
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
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