using LIMS.Service.Methodologies.Domain.SeedWork;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;

public readonly record struct ResultDefinitionId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}
