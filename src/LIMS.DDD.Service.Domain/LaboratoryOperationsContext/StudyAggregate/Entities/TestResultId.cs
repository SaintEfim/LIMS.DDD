using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Entities;

public readonly record struct TestResultId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
