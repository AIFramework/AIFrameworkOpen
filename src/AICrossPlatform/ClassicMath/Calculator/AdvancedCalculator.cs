using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics; // Основной тип для комплексных чисел
using AI.NLP;
using System.Threading;
using AI.DataStructs.WithComplexElements;
using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.ML.Distances;

namespace AI.ClassicMath.Calculator;

public static class AdvancedCalculator
{
    #region Внутренние определения и структуры

    private static readonly Dictionary<string, (int Precedence, string Associativity)> Operators;
    private static readonly Dictionary<string, FunctionDefinition> Functions;

    private static readonly Complex ImaginaryUnit = Complex.ImaginaryOne;

    static AdvancedCalculator()
    {
        Operators = new(StringComparer.OrdinalIgnoreCase)
        {
            { "+", (1, "Left") }, { "-", (1, "Left") },
            { "*", (2, "Left") }, { "/", (2, "Left") }, { "%", (2, "Left") },
            { "^", (3, "Right") }
        };

        Functions = new Dictionary<string, FunctionDefinition>(StringComparer.OrdinalIgnoreCase)
        {

            { "sin", new(1, args => Complex.Sin(CastToComplex(args[0], "sin"))) },
            { "cos", new(1, args => Complex.Cos(CastToComplex(args[0], "cos"))) },
            { "tan", new(1, args => Complex.Tan(CastToComplex(args[0], "tan"))) },
            { "sqrt", new(1, args => Complex.Sqrt(CastToComplex(args[0], "sqrt"))) },
            { "ln", new(1, args => Complex.Log(CastToComplex(args[0], "ln"))) },
            { "log10", new(1, args => Complex.Log10(CastToComplex(args[0], "log10"))) },
            { "exp", new(1, args => Complex.Exp(CastToComplex(args[0], "exp"))) },
            { "abs", new(1, args => (double)Complex.Abs(CastToComplex(args[0], "abs"))) },
            { "rad", new(1, args => FunctionsForEachElements.GradToRad(CastToDouble(args[0], "rad"))) },
            { "deg", new(1, args => FunctionsForEachElements.RadToGrad(CastToDouble(args[0], "deg"))) },
            { "gamma", new(1, args => FunctionsForEachElements.Gamma(CastToDouble(args[0], "gamma"))) },
            { "fact", new(1, args => FunctionsForEachElements.Factorial((int)CastToDouble(args[0], "fact"))) },

            { "mag", new(1, args => BaseDist.L2(CastToComplexVector(args[0], "mag"))) },
            //{ "normalize", new(1, args => CastToComplexVector(args[0], "normalize").Normalize(2)) },
            { "sum", new(1, args => Sum(CastToComplexVector(args[0], "sum"))) },

            { "pow", new(2, args => Complex.Pow(CastToComplex(args[0], "pow"), CastToComplex(args[1], "pow"))) },
            { "log", new(2, args => Complex.Log(CastToComplex(args[0], "log value")) / Complex.Log(CastToComplex(args[1], "log base"))) },
            { "C", new(2, args =>  MathUtils.Combinatorics.CombinatoricsBaseFunction.Combinations((int)CastToDouble(args[0], "C"), (int)CastToDouble(args[1], "C"))) },
            { "P", new(2, args => {
                                        // k-перестановки из n по формуле n! / (n-k)!
                                        var n = (int)CastToDouble(args[0], "P");
                                        var k = (int)CastToDouble(args[1], "P");
                                        return FunctionsForEachElements.Factorial(n) / FunctionsForEachElements.Factorial(n - k);
                                    })},

            { "dot", new(2, args => AnalyticGeometryFunctions.Dot(CastToComplexVector(args[0], "dot"), (CastToComplexVector(args[1], "dot")))) },
            { "cross", new(2, args =>
                {
                    var v1 = CastToRealVector(args[0], "cross"); var v2 = CastToRealVector(args[1], "cross");
                    if (v1.Count != 3 || v2.Count != 3) throw new ArgumentException("Функция 'cross' определена только для 3D-векторов.");
                    var r = new Vector(3);
                    r[0] = v1[1] * v2[2] - v1[2] * v2[1]; r[1] = v1[2] * v2[0] - v1[0] * v2[2]; r[2] = v1[0] * v2[1] - v1[1] * v2[0];
                    return r;
                })
            },

            { "mean", new(-1, args => {
                var complexArgs = args.Select(a => CastToComplex(a, "mean")).ToArray();
                if (!complexArgs.Any()) return Complex.Zero;
                return new Complex(complexArgs.Average(c => c.Real), complexArgs.Average(c => c.Imaginary));
              })
            },
            { "min", new(-1, args => new Complex(args.Select(a => CastToDouble(a, "min")).Min(), 0)) },
            { "max", new(-1, args => new Complex(args.Select(a => CastToDouble(a, "max")).Max(), 0)) },
        };
    }
    #endregion

    #region Вспомогательные методы приведения типов
    private static Complex CastToComplex(object obj, string funcName)
    {
        if (obj is Complex c) return c;
        if (obj is double d) return new Complex(d, 0);
        throw new ArgumentException($"Функция '{funcName}' ожидает скалярный аргумент, но получила {obj.GetType().Name}.");
    }

    // ToDo: добавить в вектор
    private static Complex Sum(ComplexVector complexes) 
    {
        Complex sum = new Complex(0, 0);
        foreach (var complex in complexes)
            sum += complex;
        return sum;
    }   

    private static double CastToDouble(object obj, string funcName)
    {
        var c = CastToComplex(obj, funcName);
        if (Math.Abs(c.Imaginary) > 1e-12) throw new ArgumentException($"Функция '{funcName}' ожидает действительный аргумент, но получила комплексное число {c}.");
        return c.Real;
    }

    private static ComplexVector CastToComplexVector(object obj, string funcName)
    {
        if (obj is ComplexVector cv) return cv;
        if (obj is Vector rv) return rv.Select(c => new Complex(c, 0)).ToArray();
        throw new ArgumentException($"Функция '{funcName}' ожидает векторный аргумент, но получила {obj.GetType().Name}.");
    }

    private static Vector CastToRealVector(object obj, string funcName)
    {
        if (obj is Vector rv) return rv;
        if (obj is ComplexVector cv)
        {
            if (cv.Any(c => Math.Abs(c.Imaginary) > 1e-12))
                throw new ArgumentException($"Функция '{funcName}' ожидает вектор с действительными компонентами.");
            return cv.Select(c => c.Real).ToArray();
        }
        throw new ArgumentException($"Функция '{funcName}' ожидает векторный аргумент, но получила {obj.GetType().Name}.");
    }
    #endregion

    #region Токенизация и Преобразование в ОПН
    private static List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        string currentToken = "";
        bool lastTokenWasOpOrParen = true;
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            if (char.IsWhiteSpace(c)) continue;
            if (c == '[' || c == ']')
            {
                if (!string.IsNullOrEmpty(currentToken)) tokens.Add(currentToken);
                currentToken = "";
                tokens.Add(c.ToString());
                lastTokenWasOpOrParen = true;
                continue;
            }
            if (char.IsLetterOrDigit(c) || c == '.') { currentToken += c; continue; }
            if (!string.IsNullOrEmpty(currentToken)) { tokens.Add(currentToken); currentToken = ""; lastTokenWasOpOrParen = false; }
            if (c == '-' && lastTokenWasOpOrParen) { tokens.Add("~"); }
            else { tokens.Add(c.ToString()); lastTokenWasOpOrParen = c == '(' || c == ',' || Operators.ContainsKey(c.ToString()); }
        }
        if (!string.IsNullOrEmpty(currentToken)) tokens.Add(currentToken); return tokens;
    }

    private static Queue<string> ConvertToRpn(List<string> tokens, ExecutionContext context)
    {
        var outputQueue = new Queue<string>(); var operatorStack = new Stack<string>(); var argCountStack = new Stack<int>();

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (IsValue(token, context)) { outputQueue.Enqueue(token); }
            else if (Functions.ContainsKey(token)) { operatorStack.Push(token); argCountStack.Push(1); }
            else if (token == ",")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(" && operatorStack.Peek() != "[") outputQueue.Enqueue(operatorStack.Pop());
                if (operatorStack.Count > 0 && (operatorStack.Peek() == "(" || operatorStack.Peek() == "["))
                {
                    if (argCountStack.Count > 0) argCountStack.Push(argCountStack.Pop() + 1);
                }
                else throw new ArgumentException("Лишняя запятая или запятая вне вызова функции/вектора.");
            }
            else if (token == "~") { operatorStack.Push(token); }
            else if (Operators.ContainsKey(token))
            {
                while (operatorStack.Count > 0 && Operators.TryGetValue(operatorStack.Peek(), out var op2) && (op2.Precedence > Operators[token].Precedence || (op2.Precedence == Operators[token].Precedence && op2.Associativity == "Left")))
                    outputQueue.Enqueue(operatorStack.Pop());
                operatorStack.Push(token);
            }
            else if (token == "(" || token == "[")
            {
                // *** ОСНОВНОЕ ИСПРАВЛЕНИЕ ЗДЕСЬ ***
                // Проверяем токен, предшествующий '(', на предмет того, является ли он переменной.
                if (i > 0 && token == "(")
                {
                    var prevToken = tokens[i - 1];
                    if (context.Memory.ContainsKey(prevToken) && !Functions.ContainsKey(prevToken))
                    {
                        throw new ArgumentException($"Переменная '{prevToken}' не является функцией.");
                    }
                }
                operatorStack.Push(token);
                if (token == "[") argCountStack.Push(1);
            }
            else if (token == ")" || token == "]")
            {
                string openBracket = token == ")" ? "(" : "[";
                while (operatorStack.Count > 0 && operatorStack.Peek() != openBracket) outputQueue.Enqueue(operatorStack.Pop());
                if (operatorStack.Count == 0) throw new ArgumentException($"Отсутствует парная открывающая скобка '{openBracket}'.");
                operatorStack.Pop();

                if (token == ")" && operatorStack.Count > 0 && Functions.ContainsKey(operatorStack.Peek()))
                {
                    outputQueue.Enqueue($"{operatorStack.Pop()}_{(argCountStack.Any() ? argCountStack.Pop() : 1)}");
                }
                else if (token == "]")
                {
                    var count = (argCountStack.Any() && outputQueue.Any()) ? argCountStack.Pop() : (outputQueue.Any() || IsPrecededByBracket(i, tokens)) ? 1 : 0;
                    if (tokens.Count == 2 && tokens[0] == "[" && tokens[1] == "]") count = 0;
                    outputQueue.Enqueue($"vector_{count}");
                }
            }
            else { throw new ArgumentException($"Неизвестный токен или синтаксическая ошибка: '{token}'"); }
        }

        while (operatorStack.Count > 0)
        {
            var op = operatorStack.Pop();
            if (op == "(" || op == "[") throw new ArgumentException($"Отсутствует парная закрывающая скобка '{(op == "(" ? ")" : "]")}'.");
            outputQueue.Enqueue(op);
        }
        return outputQueue;
    }

    private static bool IsPrecededByBracket(int currentIndex, List<string> tokens)
    {
        if (currentIndex < 1) return false;
        return tokens[currentIndex - 1] == "[" || tokens[currentIndex - 1] == "(";
    }

    private static bool IsValue(string token, ExecutionContext context) =>
        token.Equals("i", StringComparison.OrdinalIgnoreCase) ||
        double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ||
        context.Memory.ContainsKey(token);
    #endregion

    #region Вычисление ОПН
    private static object EvaluateRpn(Queue<string> rpnTokens, ExecutionContext context)
    {
        var evalStack = new Stack<object>();
        foreach (var token in rpnTokens)
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out var num)) evalStack.Push(new Complex(num, 0));
            else if (token.Equals("i", StringComparison.OrdinalIgnoreCase)) evalStack.Push(ImaginaryUnit);
            else if (context.Memory.TryGetValue(token, out var value)) evalStack.Push(value);
            else if (token == "~")
            {
                if (evalStack.Count < 1) throw new InvalidOperationException("Недостаточно операндов для унарного минуса.");
                object operand = evalStack.Pop();
                if (operand is Complex c) evalStack.Push(Complex.Negate(c));
                else if (operand is ComplexVector cv) evalStack.Push(cv * -1);
                else if (operand is Vector rv) evalStack.Push(rv * -1);
                else throw new InvalidOperationException($"Унарный минус не применим к типу {operand.GetType().Name}.");
            }
            else if (Operators.ContainsKey(token)) ApplyOperator(token, evalStack);
            else if (token.StartsWith("vector_") || token.Contains('_'))
            {
                var parts = token.Split('_'); var funcName = parts[0]; var argCountStr = parts[1];
                if (!int.TryParse(argCountStr, out var argCount)) throw new InvalidOperationException("Поврежденный токен функции/вектора.");

                if (argCount == 0 && funcName == "vector")
                {
                    evalStack.Push(new ComplexVector(0));
                    continue;
                }

                if (evalStack.Count < argCount) throw new InvalidOperationException($"Недостаточно операндов в стеке для '{funcName}' (нужно {argCount}, найдено {evalStack.Count}).");

                var args = new object[argCount]; for (int i = argCount - 1; i >= 0; i--) args[i] = evalStack.Pop();

                if (funcName == "vector")
                {
                    evalStack.Push(new ComplexVector(args.Select(a => CastToComplex(a, "vector component"))));
                }
                else
                {
                    if (!Functions.ContainsKey(funcName)) throw new NotSupportedException($"Функция '{funcName}' не найдена.");
                    var funcDef = Functions[funcName];
                    if (funcDef.ArgumentCount != -1 && funcDef.ArgumentCount != argCount) throw new ArgumentException($"Функция '{funcName}' ожидает {funcDef.ArgumentCount} аргументов, но получила {argCount}.");
                    evalStack.Push(funcDef.Delegate(args));
                }
            }
        }
        if (evalStack.Count != 1) return new InvalidOperationException($"Ошибка в синтаксисе выражения. Стек содержит {evalStack.Count} элементов вместо 1.");
        return evalStack.Pop();
    }
    #endregion

    #region Вспомогательные методы вычисления
    private static void ApplyOperator(string op, Stack<object> stack)
    {
        if (stack.Count < 2) throw new InvalidOperationException($"Недостаточно операндов для оператора '{op}'.");
        var op2 = stack.Pop();
        var op1 = stack.Pop();

        object result = (Normalize(op1), Normalize(op2), op) switch
        {
            (ComplexVector v1, ComplexVector v2, "+") => v1 + v2,
            (ComplexVector v1, ComplexVector v2, "-") => v1 - v2,
            (ComplexVector v, Complex c, "*") => v * c,
            (Complex c, ComplexVector v, "*") => v * c,
            (ComplexVector v, Complex c, "/") => v * c,

            (Complex c1, Complex c2, "+") => c1 + c2,
            (Complex c1, Complex c2, "-") => c1 - c2,
            (Complex c1, Complex c2, "*") => c1 * c2,
            (Complex c1, Complex c2, "/") => c1 / c2,
            (Complex c1, Complex c2, "^") => Complex.Pow(c1, c2),
            (Complex c1, Complex c2, "%") => new Complex(c1.Real % c2.Real, 0),

            _ => throw new InvalidOperationException($"Оператор '{op}' не применим к типам {op1.GetType().Name} и {op2.GetType().Name}.")
        };
        stack.Push(result);
    }

    private static object Normalize(object obj) => obj switch
    {
        double d => new Complex(d, 0),
        Vector rv => rv.Select(c => new Complex(c, 0)),
        _ => obj
    };
    #endregion

    #region Публичный метод-обертка
    public static object Evaluate(string expression, ExecutionContext context)
    {
        try
        {
            expression = expression.Trim();
            if (string.IsNullOrEmpty(expression)) return null;

            if (expression.Count(c => c == '=') == 1 && !IsComplexExpression(expression))
            {
                var assignmentParts = expression.Split('=');
                var varName = assignmentParts[0].Trim();
                if (!IsValidVarName(varName)) throw new ArgumentException($"Недопустимое имя переменной: '{varName}'.");

                var exprToEvaluate = assignmentParts[1];
                var result = EvaluateExpression(exprToEvaluate, context);
                context.Memory[varName] = result;
                return result;
            }
            return EvaluateExpression(expression, context);
        }
        catch (Exception ex)
        {
            var inner = ex.InnerException != null ? " -> " + ex.InnerException.Message : "";
            return $"Ошибка: {ex.Message}{inner}";
        }
    }

    private static object EvaluateExpression(string expression, ExecutionContext context)
    {
        var tokens = Tokenize(expression);
        var rpn = ConvertToRpn(tokens, context);
        return EvaluateRpn(rpn, context);
    }

    private static bool IsValidVarName(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        !name.Equals("i", StringComparison.OrdinalIgnoreCase) &&
        (char.IsLetter(name[0]) || name[0] == '_') &&
        name.All(c => char.IsLetterOrDigit(c) || c == '_') &&
        !Functions.ContainsKey(name);

    private static bool IsComplexExpression(string expression) => expression.Split('=')[0].Any(c => "()[]<>!".Contains(c));
    #endregion
}

