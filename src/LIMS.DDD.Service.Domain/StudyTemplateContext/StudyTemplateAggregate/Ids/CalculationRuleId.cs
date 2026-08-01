using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;

public readonly record struct CalculationRuleId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
