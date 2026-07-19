using LIMS.DDD.Service.Domain.StudyTemplate.Parameter;
using LIMS.DDD.Service.Domain.StudyTemplate.Result;

namespace LIMS.DDD.Service.Domain.StudyTemplate;

public sealed class StudyTemplate
{
    private StudyTemplate()
    {
    }

    public StudyTemplateId Id { get; private set; }

    public Name Name { get; private set; }

    public Description? Description { get; private set; }

    public Revision Revision { get; private set; }

    public Status Status { get; private set; }

    public IReadOnlyList<StudyTemplateResult> Results => _results.AsReadOnly();

    private readonly List<StudyTemplateResult> _results = [];

    public IReadOnlyList<StudyTemplateParameter> Parameters => _parameters.AsReadOnly();

    private readonly List<StudyTemplateParameter> _parameters = [];

    public static StudyTemplate Create(
        Name name,
        Description description,
        Revision revision)
    {
        var studyTemplate = new StudyTemplate
        {
            Name = name,
            Description = description,
            Revision = revision
        };

        return studyTemplate;
    }

    public static StudyTemplate ChangeStatus(
        StudyTemplate studyTemplate,
        Status status)
    {
        if (studyTemplate.Status == status)
        {
            throw new InvalidOperationException("Cannot change status of study template.");
        }

        studyTemplate.Status = status;

        return studyTemplate;
    }
}
