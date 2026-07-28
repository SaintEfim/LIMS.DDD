using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities;

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

    public InputParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public Specification Specification { get; private set; } = null!;
}
