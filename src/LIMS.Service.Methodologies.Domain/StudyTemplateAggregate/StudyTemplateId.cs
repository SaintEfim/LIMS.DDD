using Domain.SeedWork.SeedWork;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;

public readonly record struct StudyTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
