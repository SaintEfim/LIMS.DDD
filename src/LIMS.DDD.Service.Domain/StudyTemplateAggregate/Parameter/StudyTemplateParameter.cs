using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter;

public sealed class StudyTemplateParameter
{
    private StudyTemplateParameter()
    {
    }

    private StudyTemplateParameter(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        ValueRange valueRange)
    {
        Id = new StudyTemplateParameterId(Guid.NewGuid());
        StudyTemplateId = studyTemplateId;
        Name = name;
        Description = description;
        AliasName = aliasName;
        ValueRange = valueRange;
    }

    internal static StudyTemplateParameter Create(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        ValueRange valueRange)
    {
        return new StudyTemplateParameter(
            studyTemplateId,
            name,
            description,
            aliasName,
            valueRange);
    }

    public StudyTemplateParameterId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public ValueRange ValueRange { get; private set; } = null!;
}
