using System.Collections.Generic;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Инструкция 'for'.
/// </summary>
internal class ForStatement : Statement
{
    public string Initializer { get; }
    public string Condition { get; }
    public string Increment { get; }
    public List<Statement> Body { get; }

    public ForStatement(string initializer, string condition, string increment, List<Statement> body)
    {
        Initializer = initializer;
        Condition = condition;
        Increment = increment;
        Body = body;
    }

    public override void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(Initializer)) processor.AdvancedCalculator.Evaluate(Initializer, context, cancellationToken);
        
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
                    // Continue - переходим к инкременту и следующей итерации
                }
                
                if (!string.IsNullOrWhiteSpace(Increment)) 
                    processor.AdvancedCalculator.Evaluate(Increment, context, cancellationToken);
            }
        }
        catch (BreakException)
        {
            // Break - выходим из цикла
        }
    }
}