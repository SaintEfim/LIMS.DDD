namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

public sealed class StudyTemplateObservation
{
    private StudyTemplateObservation()
    {
    }

    internal static StudyTemplateObservation Create(
        StudyTemplateId studyTemplateId,
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        var parameter = new StudyTemplateObservation
        {
            Id = new StudyTemplateObservationId(Guid.NewGuid()),
            StudyTemplateId = studyTemplateId,
            Name = name,
            Description = description,
            AliasName = aliasName,
            Specification = specification
        };

        return parameter;
    }

    public StudyTemplateObservationId Id { get; private set; }

    public StudyTemplateId StudyTemplateId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public AliasName AliasName { get; private set; }

    public Specification Specification { get; private set; } = null!;
}
