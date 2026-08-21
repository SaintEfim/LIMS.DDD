using Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;

public readonly record struct CalculationRuleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
