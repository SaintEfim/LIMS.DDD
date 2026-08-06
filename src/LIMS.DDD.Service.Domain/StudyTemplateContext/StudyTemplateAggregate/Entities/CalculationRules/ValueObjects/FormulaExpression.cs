using LIMS.DDD.Service.Domain.SeedWork.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;

public sealed record FormulaExpression
{
    private const int MaxLength = 2000;

    private FormulaExpression(
        string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<FormulaExpression, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<FormulaExpression, Exception>.Failure(
                new ArgumentException("Formula expression cannot be empty.", nameof(value)));
        }

        if (value.Length > MaxLength)
        {
            return Result<FormulaExpression, Exception>.Failure(new ArgumentException(
                $"Formula expression length cannot exceed {MaxLength} characters. Current length: {value.Length}.",
                nameof(value)));
        }

        if (value.StartsWith("=") || value.StartsWith("+") || value.StartsWith("-"))
        {
            return Result<FormulaExpression, Exception>.Failure(
                new ArgumentException("Formula expression cannot start or end with '=', '+', '-' characters.",
                    nameof(value)));
        }

        var formula = new FormulaExpression(value.Trim());
        return Result<FormulaExpression, Exception>.Success(formula);
    }
}
