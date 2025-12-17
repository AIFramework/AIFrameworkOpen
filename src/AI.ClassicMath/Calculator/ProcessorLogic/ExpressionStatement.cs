using System.Collections.Generic;
using System.Threading;

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

    public override void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(Expression)) return;

        cancellationToken.ThrowIfCancellationRequested();
        
        if (_isSilent)
        {
            // Тихий режим: просто выполняем, ничего не выводим.
            processor.AdvancedCalculator.Evaluate(Expression, context, cancellationToken);
        }
        else
        {
            // Громкий режим: выводим команду и результат.
            output.Add($">> {Expression}");
            var result = processor.AdvancedCalculator.Evaluate(Expression, context, cancellationToken);
            if (result != null)
            {
                output.Add($"=> {Processor.FormatResult(result)}");
            }
        }
    }
}