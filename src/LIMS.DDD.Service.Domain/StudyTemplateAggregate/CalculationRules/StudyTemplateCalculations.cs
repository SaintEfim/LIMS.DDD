using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

public readonly record struct CalculationRuleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed class StudyTemplateCalculations
{
    private StudyTemplateCalculations()
    {
    }

    internal static StudyTemplateCalculations Create(
        StudyTemplateId studyTemplateId,
        Name name,
        FormulaExpression formulaExpression,
        Description description)
    {
        return new StudyTemplateCalculations
        {
            Id = new CalculationRuleId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Name = name,
            FormulaExpression = formulaExpression,
            Description = description
        };
    }

    public CalculationRuleId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public FormulaExpression FormulaExpression { get; private set; }

    public Description Description { get; private set; }
}
