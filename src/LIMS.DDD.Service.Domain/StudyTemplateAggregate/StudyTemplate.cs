using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public sealed class StudyTemplate : IAggregateRoot
{
    private StudyTemplate()
    {
    }

    public StudyTemplateId Id { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Revision Revision { get; private set; }

    public Status Status { get; private set; }

    public IReadOnlyList<StudyTemplateObservation> Observations => _observation.AsReadOnly();

    private readonly List<StudyTemplateObservation> _observation = [];

    public IReadOnlyList<StudyTemplateDetermination> Determinations => _determinations.AsReadOnly();

    private readonly List<StudyTemplateDetermination> _determinations = [];

    public static Result<StudyTemplate, Exception> Create(
        Name name,
        Description description,
        Revision revision)
    {
        var studyTemplate = new StudyTemplate
        {
            Id = new StudyTemplateId(Guid.NewGuid()),
            Name = name,
            Description = description,
            Revision = revision
        };

        return Result<StudyTemplate, Exception>.Success(studyTemplate);
    }

    public Result<StudyTemplate, Exception> UpdatePartial(
        Name? name,
        Description? description)
    {
        if (Status != Status.Draft)
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException(
                    "Cannot modify details of an Active or Archived template. Create a new revision."));

        if (name is not null) Name = name.Value;
        if (description is not null) Description = description.Value;

        return Result<StudyTemplate, Exception>.Success(this);
    }

    public Result<StudyTemplateObservation, Exception> AddStudyTemplateObservation(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (Status != Status.Draft)
            return Result<StudyTemplateObservation, Exception>.Failure(
                new InvalidOperationException("Cannot add observation to an Active template."));

        if (_observation.Any(p => p.Name == name))
            return Result<StudyTemplateObservation, Exception>.Failure(
                new InvalidOperationException("Parameter name must be unique within the template."));

        var parameter = StudyTemplateObservation.Create(Id, name, description, aliasName, specification);

        _observation.Add(parameter);
        return Result<StudyTemplateObservation, Exception>.Success(parameter);
    }

    public Result<Exception> RemoveStudyTemplateObservation(
        StudyTemplateObservationId observationId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot remove observation from an Active template."));

        var parameter = _observation.SingleOrDefault(p => p.Id == observationId);
        if (parameter == null) return Result<Exception>.Failure(new InvalidOperationException("Parameter not found."));

        _observation.Remove(parameter);
        return Result<Exception>.Success();
    }

    public Result<Exception> Approve()
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(new InvalidOperationException("Only Draft templates can be approved."));

        if (!_observation.Any())
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot approve a template without parameters."));

        Status = Status.Active;

        return Result<Exception>.Success();
    }

    public Result<Exception> Archive()
    {
        if (Status == Status.Archived)
            return Result<Exception>.Failure(new InvalidOperationException("Template is already archived."));

        Status = Status.Archived;
        return Result<Exception>.Success();
    }

    public Result<Exception> EnsureCanBeDeleted()
    {
        if (Status != Status.Draft)
        {
            return Result<Exception>.Failure(new InvalidOperationException(
                $"Cannot delete a template with status '{Status}'. Only 'Draft' templates can be deleted. Use Archive instead."));
        }

        return Result<Exception>.Success();
    }

    public Result<StudyTemplateDetermination, Exception> AddStudyTemplateDetermination(
        string resultInstance,
        string unit,
        Specification valueRange)
    {
        if (Status != Status.Draft)
            return Result<StudyTemplateDetermination, Exception>.Failure(
                new InvalidOperationException("Cannot add determination to an Active template."));

        var existsResult = _determinations.Any(x => x.ResultInstance == resultInstance && x.Unit == unit);
        if (existsResult)
        {
            return Result<StudyTemplateDetermination, Exception>.Failure(
                new InvalidOperationException("Determination result instance already exists."));
        }

        var result = StudyTemplateDetermination.Create(Id, resultInstance, unit, valueRange);
        _determinations.Add(result);

        return Result<StudyTemplateDetermination, Exception>.Success(result);
    }

    public Result<Exception> RemoveStudyTemplateDetermination(
        StudyTemplateDeterminationId determinationId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot add determination to an Active template."));

        var result = _determinations.SingleOrDefault(r => r.Id == determinationId);
        if (result == null)
        {
            return Result<Exception>.Failure(new InvalidOperationException("Determination result not found."));
        }

        _determinations.Remove(result);
        return Result<Exception>.Success();
    }
}
