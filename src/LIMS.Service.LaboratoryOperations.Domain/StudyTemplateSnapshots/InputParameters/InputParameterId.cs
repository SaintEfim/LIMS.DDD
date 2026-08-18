using LIMS.Service.LaboratoryOperations.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;

public readonly record struct InputParameterId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
