using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter;

public sealed class StudyTemplateParameter
{
    private StudyTemplateParameter()
    {
    }

    internal static StudyTemplateParameter Create(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        ValueRange valueRange)
    {
        var parameter = new StudyTemplateParameter
        {
            Id = new StudyTemplateParameterId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Name = name,
            Description = description,
            AliasName = aliasName,
            ValueRange = valueRange
        };

        return parameter;
    }

    public StudyTemplateParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public ValueRange ValueRange { get; private set; } = null!;
}
