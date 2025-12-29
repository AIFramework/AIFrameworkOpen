using System.Collections.Generic;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция 'while'.
/// </summary>
internal class WhileStatement : Statement
{
    public string Condition { get; }
    public List<Statement> Body { get; }

    public WhileStatement(string condition, List<Statement> body)
    {
        Condition = condition;
        Body = body;
    }

    public override void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default)
    {
        try
        {
            while (processor.IsTruthy(processor.AdvancedCalculator.Evaluate(Condition, context, cancellationToken)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    foreach (var statement in Body)
                    {
                        statement.Execute(processor, context, output, cancellationToken);
                    }
                }
                catch (ContinueException)
                {
                    // Continue - переходим к следующей итерации
                }
            }
        }
        catch (BreakException)
        {
            // Break - выходим из цикла
        }
    }
}