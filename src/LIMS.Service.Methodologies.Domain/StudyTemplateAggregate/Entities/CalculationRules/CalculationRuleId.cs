using LIMS.Service.Methodologies.Domain.SeedWork;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.CalculationRules;

public readonly record struct CalculationRuleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
