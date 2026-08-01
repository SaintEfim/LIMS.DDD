using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities;

public sealed class InputParameter
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
        if (name is not null) Name = name.Value;
        if (description is not null) Description = description.Value;
        if (aliasName is not null) AliasName = aliasName.Value;
        if (specification is not null) Specification = specification;
    }

    public InputParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public Specification Specification { get; private set; } = null!;
}
