using System.Text.RegularExpressions;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;

namespace Domain.SeedWork.ValueObjects;

public sealed record FormulaExpression
{
    private const int MaxLength = 2000;

    // for EF Core
    private FormulaExpression()
    {
    }

    private FormulaExpression(
        string value)
    {
        Value = value;
    }

    public string Value { get; } = null!;

    private static Regex Formula()
    {
        return new Regex(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", RegexOptions.Compiled);
    }

    public static Result<FormulaExpression, DomainError> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ValidationError("Formula expression cannot be empty.");
        }

        if (value.Length > MaxLength)
        {
            return new ValidationError(
                $"Formula expression length cannot exceed {MaxLength} characters. Current length: {value.Length}.");
        }

        if (value.StartsWith("=") || value.StartsWith("+") || value.StartsWith("-"))
        {
            return new ValidationError("Formula expression cannot start with '=', '+', or '-' characters.");
        }

        var formula = new FormulaExpression(value.Trim());
        return formula;
    }

    public IReadOnlyCollection<string> ExtractVariables()
    {
        var matches = Formula()
            .Matches(Value);

        return matches.Select(m => m.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
