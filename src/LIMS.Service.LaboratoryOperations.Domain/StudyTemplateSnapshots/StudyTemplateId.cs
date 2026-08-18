using LIMS.Service.LaboratoryOperations.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

public readonly record struct StudyTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
