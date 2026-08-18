using LIMS.Service.LaboratoryOperations.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

public readonly record struct ResultDefinitionId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
