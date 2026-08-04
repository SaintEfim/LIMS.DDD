using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;

public sealed class InputParameter : SoftDeletableModel
{
    private InputParameter()
    {
    }

    internal static InputParameter Create(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        var parameter = new InputParameter
        {
            Id = new InputParameterId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Name = name,
            Description = description,
            AliasName = aliasName,
            Specification = specification
        };

        return parameter;
    }

    internal void Update(
        Name? name,
        Description? description,
        AliasName? aliasName,
        Specification? specification)
    {
        if (name is not null) Name = name;
        if (description is not null) Description = description;
        if (aliasName is not null) AliasName = aliasName;
        if (specification is not null) Specification = specification;
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }

    public InputParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public Specification Specification { get; private set; } = null!;
}
