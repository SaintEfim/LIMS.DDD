using Domain.SeedWork.SeedWork;
using Domain.SeedWork.SeedWork.ValueObjects;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;

public sealed class InputParameter : SoftDeletableModel
{
    internal InputParameter(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        Id = new InputParameterId(Guid.NewGuid());
        StudyTemplateId = studyTemplateId;
        Name = name;
        Description = description;
        AliasName = aliasName;
        Specification = specification;
    }

    // for EF Core
    private InputParameter()
    {
    }

    public InputParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public AliasName AliasName { get; private set; } = null!;

    public Specification Specification { get; private set; } = null!;

    internal void Update(
        Name? name,
        Description? description,
        AliasName? aliasName,
        Specification? specification)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (description is not null)
        {
            Description = description;
        }

        if (aliasName is not null)
        {
            AliasName = aliasName;
        }

        if (specification is not null)
        {
            Specification = specification;
        }
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
