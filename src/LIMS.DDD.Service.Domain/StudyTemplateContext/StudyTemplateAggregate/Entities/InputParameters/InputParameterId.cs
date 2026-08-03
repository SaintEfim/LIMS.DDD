using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;

public readonly record struct InputParameterId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
