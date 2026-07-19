namespace LIMS.DDD.Service.Domain.StudyTemplate;

public sealed class StudyTemplate
{
    public StudyTemplateId Id { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Revision Revision { get; private set; }
}
