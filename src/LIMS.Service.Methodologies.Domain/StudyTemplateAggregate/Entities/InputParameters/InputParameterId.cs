using Domain.SeedWork.SeedWork;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;

public readonly record struct InputParameterId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
