using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AI.ClassicMath.Calculator.ProcessorLogic;



[Serializable]
public class Processor
{
    public readonly AdvancedCalculator AdvancedCalculator = new AdvancedCalculator();

    public List<string> Run(string script)
    {
        var context = new ExecutionContext();
        var output = new List<string>();
        try
        {
            var lines = script.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var lineQueue = new Queue<string>(lines);
            var statements = ParseStatements(lineQueue, isSilentContext: false);
            foreach (var statement in statements) statement.Execute(this, context, output);
        }
        catch (Exception ex)
        {
            output.Add($"!!! КРИТИЧЕСКАЯ ОШИБКА: {ex.GetType().Name} -> {ex.Message}");
        }
        return output;
    }

    #region Парсер скрипта

    private List<Statement> ParseStatements(Queue<string> lines, bool isSilentContext)
    {
        var statements = new List<Statement>();
        while (lines.Count > 0)
        {
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
                var trueBody = ParseBlock(lines);
                List<Statement> falseBody = null;
                if (lines.Count > 0 && lines.Peek().Trim().StartsWith("else"))
                {
                    lines.Dequeue();
                    falseBody = ParseBlock(lines);
                }
                statements.Add(new IfStatement(condition, trueBody, falseBody));
            }
            else if (whileMatch.Success)
            {
                var condition = whileMatch.Groups[1].Value;
                var body = ParseBlock(lines);
                statements.Add(new WhileStatement(condition, body));
            }
            else if (forMatch.Success)
            {
                var initializer = forMatch.Groups[1].Value;
                var condition = forMatch.Groups[2].Value;
                var increment = forMatch.Groups[3].Value;
                var body = ParseBlock(lines);
                statements.Add(new ForStatement(initializer, condition, increment, body));
            }
            else if (line == "{" || line.StartsWith("else")) { throw new ArgumentException($"Неожиданный токен: '{line}'"); }
            else { statements.Add(new ExpressionStatement(line, isSilentContext)); }
        }
        return statements;
    }

    private List<Statement> ParseBlock(Queue<string> lines)
    {
        var blockLines = new Queue<string>();
        int braceLevel = 1;
        if (lines.Count > 0 && lines.Peek().Trim() == "{") lines.Dequeue();
        while (lines.Count > 0)
        {
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
        return ParseStatements(blockLines, isSilentContext: true);
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
    /// Форматирование ответа
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string FormatResult(object result)
    {
        return result switch
        {
            double d => d.ToString("G9", CultureInfo.InvariantCulture),
            Complex c => Math.Abs(c.Imaginary) < 1e-12 ? c.Real.ToString("G6", CultureInfo.InvariantCulture) : $"{c.Real.ToString("G9", CultureInfo.InvariantCulture)} + {c.Imaginary.ToString("G9", CultureInfo.InvariantCulture)}i",
            ComplexVector v => $"[{string.Join(", ", v.Select(c => FormatResult(c)))}]",
            Vector dv => $"[{string.Join(", ", dv.Select(c => c.ToString("G6", CultureInfo.InvariantCulture)))}]",
            _ => result?.ToString() ?? "null"
        };
    }
    #endregion
}