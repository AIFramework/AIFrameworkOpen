using System.Collections.Generic;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция-выражение (например, a = 5 + b). Умеет работать в "тихом" режиме.
/// </summary>
internal class ExpressionStatement : Statement
{
    public string Expression { get; }
    private readonly bool _isSilent;

    public ExpressionStatement(string expression, bool isSilent = false)
    {
        Expression = expression.Trim();
        _isSilent = isSilent;
    }

    public override void Execute(Processor processor, ExecutionContext context, List<string> output)
    {
        if (string.IsNullOrWhiteSpace(Expression)) return;

        if (_isSilent)
        {
            // Тихий режим: просто выполняем, ничего не выводим.
            processor.AdvancedCalculator.Evaluate(Expression, context);
        }
        else
        {
            // Громкий режим: выводим команду и результат.
            output.Add($">> {Expression}");
            var result = processor.AdvancedCalculator.Evaluate(Expression, context);
            if (result != null)
            {
                output.Add($"=> {Processor.FormatResult(result)}");
            }
        }
    }
}