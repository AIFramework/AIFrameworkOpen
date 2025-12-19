using AI.ClassicMath.Calculator.Libs;
using AI.ClassicMath.Calculator.Libs.Algebra;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;

namespace AI.ClassicMath.Calculator
{
    /// <summary>
    /// Калькулятор с поддержкой библиотек, векторов и комплексных чисел.
    /// </summary>
    [Serializable]
    public class AdvancedCalculator
    {
        #region Поля и конструктор

        /// <summary>
        /// Зарегистрированные операторы
        /// </summary>
        public Dictionary<string, (int Precedence, string Associativity)> Operators { get; set; }

        /// <summary>
        /// Операции с этими операторами
        /// </summary>
        public Dictionary<(Type, Type, string), Func<object, object, object>> OperationsFunctions { get; set; }

        /// <summary>
        /// Функции (математические)
        /// </summary>
        public Dictionary<string, FunctionDefinition> Functions { get; set; }


        /// <summary>
        /// Калькулятор с поддержкой библиотек, векторов и комплексных чисел.
        /// </summary>
        public AdvancedCalculator()
        {
            var baseMathLib = new BaseMathLib();
            var eq = new EquationLib();

            var baseOperators = new LibOperatorsBase();

            Operators = baseOperators.GetOperators();
            OperationsFunctions = baseOperators.GetOperationsFunctions();

            Functions = baseMathLib.GetFunctions();

            foreach (var func in eq.GetFunctions())
            {
                Functions.Add(func.Key, func.Value);
            }
        }

        #endregion


        public object Evaluate(string expression, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                expression = expression.Trim();
                if (string.IsNullOrEmpty(expression)) return null;

                if (expression.Count(c => c == '=') == 1 && IsSimpleAssignmentTarget(expression))
                {
                    var parts = expression.Split('=');
                    var varName = parts[0].Trim();
                    if (!IsValidVarName(varName)) throw new ArgumentException($"Недопустимое имя переменной: '{varName}'.");

                    var exprToEvaluate = parts[1];
                    var result = EvaluateExpression(exprToEvaluate, context, cancellationToken);
                    context.Memory[varName] = result;
                    return result;
                }
                return EvaluateExpression(expression, context, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
        }

        private object EvaluateExpression(string expression, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var tokens = Tokenize(expression, cancellationToken);
            var rpn = ConvertToRpn(tokens, context, cancellationToken);
            return EvaluateRpn(rpn, context, cancellationToken);
        }


        #region Токенизация -> ОПН -> Вычисление

        private List<string> Tokenize(string expression, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Сначала извлекаем строковые литералы (в двойных кавычках)
            var pattern = @"(""[^""]*"")|([0-9]+\.?[0-9]*|[0-9]*\.?[0-9]+)|([a-zA-Z_][a-zA-Z0-9_]*)|(>=|<=|==|!=)|(.)";
            var tokens = Regex.Matches(expression, pattern).Cast<Match>()
                .Where(m => !string.IsNullOrWhiteSpace(m.Value))
                .Select(m => m.Value).ToList();

            for (int i = 0; i < tokens.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (tokens[i] == "-")
                {
                    bool isUnary = (i == 0) || Operators.ContainsKey(tokens[i - 1]) || "([,".Contains(tokens[i - 1]);
                    if (isUnary) tokens[i] = "~";
                }
            }
            return tokens;
        }

        private Queue<string> ConvertToRpn(List<string> tokens, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();
            var argCountStack = new Stack<int>();
            for (int i = 0; i < tokens.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var token = tokens[i];
                if (IsValue(token, context))
                {
                    outputQueue.Enqueue(token);
                }
                else if (Functions.ContainsKey(token))
                {
                    operatorStack.Push(token);
                    argCountStack.Push(1);
                }
                else if (token == ",")
                {
                    while (operatorStack.Count > 0 && !"([".Contains(operatorStack.Peek()))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && "([".Contains(operatorStack.Peek()))
                    {
                        if (argCountStack.Count > 0) argCountStack.Push(argCountStack.Pop() + 1);
                    }
                    else throw new ArgumentException("Лишняя запятая или запятая вне вызова функции/вектора.");
                }
                else if (token == "~")
                {
                    operatorStack.Push(token);
                }
                else if (Operators.ContainsKey(token))
                {
                    while (operatorStack.Count > 0 && Operators.TryGetValue(operatorStack.Peek(), out
                        var op2) && (op2.Precedence > Operators[token].Precedence || (op2.Precedence == Operators[token].Precedence && op2.Associativity == "Left")))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                }
                else if ("([".Contains(token))
                {
                    if (i > 0 && token == "(" && context.Memory.ContainsKey(tokens[i - 1]) && !Functions.ContainsKey(tokens[i - 1])) throw new ArgumentException($"Переменная '{tokens[i - 1]}' не является функцией.");
                    operatorStack.Push(token);
                    if (token == "[")
                        argCountStack.Push(1);
                }
                else if (")]".Contains(token))
                {
                    string openBracket = token == ")" ? "(" : "[";
                    while (operatorStack.Count > 0 && operatorStack.Peek() != openBracket)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.Count == 0)
                        throw new ArgumentException($"Отсутствует парная открывающая скобка '{openBracket}'.");
                    operatorStack.Pop();

                    if (token == ")" && operatorStack.Count > 0 && Functions.ContainsKey(operatorStack.Peek()))
                    {
                        outputQueue.Enqueue($"{operatorStack.Pop()}_{(argCountStack.Any() ? argCountStack.Pop() : 1)}");
                    }
                    else if (token == "]")
                    {
                        var count = (argCountStack.Any() && outputQueue.Any()) ? argCountStack.Pop() : (outputQueue.Any() || (i > 0 && "([".Contains(tokens[i - 1]))) ? 1 : 0;
                        if (tokens.Count == 2 && tokens[0] == "[" && tokens[1] == "]") count = 0;
                        outputQueue.Enqueue($"vector_{count}");
                    }
                }
                else
                {
                    throw new ArgumentException($"Неизвестный токен или синтаксическая ошибка: '{token}'");
                }
            }
            while (operatorStack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var op = operatorStack.Pop();
                if ("([".Contains(op)) throw new ArgumentException($"Отсутствует парная закрывающая скобка.");
                outputQueue.Enqueue(op);
            }
            return outputQueue;
        }

        private object EvaluateRpn(Queue<string> rpnTokens, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            var evalStack = new Stack<object>();
            foreach (var token in rpnTokens)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out
                    var num))
                {
                    evalStack.Push(new Complex(num, 0));
                }
                else if (token == "i")
                {
                    evalStack.Push(Complex.ImaginaryOne);
                }
                else if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    // Строковый литерал - убираем кавычки и пушим как строку
                    evalStack.Push(token.Substring(1, token.Length - 2));
                }
                else if (context.Memory.TryGetValue(token, out
                    var value))
                {
                    evalStack.Push(value);
                }
                else if (token == "~")
                {
                    if (evalStack.Count < 1) throw new InvalidOperationException("Недостаточно операндов для унарного минуса.");
                    object operand = evalStack.Pop();
                    if (operand is Complex c) evalStack.Push(Complex.Negate(c));
                    else if (operand is ComplexVector cv) evalStack.Push(cv * -1);
                    else if (operand is Vector rv) evalStack.Push(rv * -1);
                    else throw new InvalidOperationException($"Унарный минус не применим к типу {operand.GetType().Name}.");
                }
                else if (Operators.ContainsKey(token))
                {
                    ApplyOperator(token, evalStack);
                }
                else if (token.StartsWith("vector_") || token.Contains('_'))
                {
                    var parts = token.Split(new[] { '_' }, 2);
                    var funcName = parts[0];
                    var argCountStr = parts[1];
                    if (!int.TryParse(argCountStr, out
                        var argCount)) throw new InvalidOperationException("Поврежденный токен функции/вектора.");
                    if (argCount == 0 && funcName == "vector")
                    {
                        evalStack.Push(new ComplexVector(0));
                        continue;
                    }
                    if (evalStack.Count < argCount) throw new InvalidOperationException($"Недостаточно операндов в стеке для '{funcName}' (нужно {argCount}, найдено {evalStack.Count}).");
                    var args = new object[argCount];
                    for (int i = argCount - 1; i >= 0; i--)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        args[i] = evalStack.Pop();
                    }
                    if (funcName == "vector")
                    {
                        evalStack.Push(new ComplexVector(args.Select(a => CastsVar.CastToComplex(a, "vector component"))));
                    }
                    else
                    {
                        if (!Functions.ContainsKey(funcName)) throw new NotSupportedException($"Функция '{funcName}' не найдена.");
                        var funcDef = Functions[funcName];
                        if (funcDef.ArgumentCount != -1 && funcDef.ArgumentCount != argCount) throw new ArgumentException($"Функция '{funcName}' ожидает {funcDef.ArgumentCount} аргументов, но получила {argCount}.");
                        evalStack.Push(funcDef.Delegate(args));
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Неизвестный токен в RPN: {token}");
                }
            }

            if (evalStack.Count > 1) throw new InvalidOperationException("Ошибка в синтаксисе выражения: в стеке осталось больше одного элемента.");
            return evalStack.Count == 0 ? null : evalStack.Pop();
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Применяет оператор к двум верхним элементам стека.
        /// </summary>
        private void ApplyOperator(string op, Stack<object> stack)
        {
            if (stack.Count < 2)
            {
                throw new InvalidOperationException($"Недостаточно операндов для оператора '{op}'.");
            }

            var op2 = stack.Pop();
            var op1 = stack.Pop();

            var normalizedOp1 = Normalize(op1);
            var normalizedOp2 = Normalize(op2);

            var key = (normalizedOp1.GetType(), normalizedOp2.GetType(), op);

            if (OperationsFunctions.TryGetValue(key, out var operationFunc))
            {
                object result = operationFunc(normalizedOp1, normalizedOp2);
                stack.Push(result);
            }
            else
            {
                // Если в словаре не нашлось подходящей операции, генерируем исключение
                throw new InvalidOperationException($"Оператор '{op}' не применим к типам {op1.GetType().Name} и {op2.GetType().Name}.");
            }
        }

        private object Normalize(object obj) => obj
        switch
        {
            double d => new Complex(d, 0),
            int i => new Complex(i, 0),
            Vector rv => new ComplexVector(rv.Select(c => new Complex(c, 0)).ToArray()),
            _ => obj
        };
        private bool IsValue(string token, ExecutionContext context) =>
            token == "i" || 
            (token.StartsWith("\"") && token.EndsWith("\"")) ||  // Строковый литерал
            double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || 
            context.Memory.ContainsKey(token);
        private bool IsValidVarName(string name) =>
            !string.IsNullOrWhiteSpace(name) && name != "i" && (char.IsLetter(name[0]) || name[0] == '_') && name.All(c => char.IsLetterOrDigit(c) || c == '_') && !Functions.ContainsKey(name);
        private bool IsSimpleAssignmentTarget(string expression) =>
            !expression.Split('=')[0].Any(c => "()[]<>!".Contains(c));

        #endregion
    }
}