using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

public readonly record struct StudyTemplateObservationId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
