using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;

public readonly record struct StudyTemplateDeterminationId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
