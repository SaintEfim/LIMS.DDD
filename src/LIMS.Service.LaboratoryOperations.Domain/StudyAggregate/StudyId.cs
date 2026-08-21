using Domain.SeedWork.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;

public readonly record struct StudyId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
