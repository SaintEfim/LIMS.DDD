using System.Text.RegularExpressions;
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

    public static Result<FormulaExpression, Exception> Create(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ArgumentException("Formula expression cannot be empty.", nameof(value));
        }

        if (value.Length > MaxLength)
        {
            return new ArgumentException(
                $"Formula expression length cannot exceed {MaxLength} characters. Current length: {value.Length}.",
                nameof(value));
        }

        if (value.StartsWith("=") || value.StartsWith("+") || value.StartsWith("-"))
        {
            return new ArgumentException("Formula expression cannot start or end with '=', '+', '-' characters.",
                nameof(value));
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
