using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateParameters;

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

    public IReadOnlyList<StudyTemplateParameter> Parameters => _parameters.AsReadOnly();

    private readonly List<StudyTemplateParameter> _parameters = [];

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

    public Result<StudyTemplateParameter, Exception> AddParameter(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (Status != Status.Draft)
            return Result<StudyTemplateParameter, Exception>.Failure(
                new InvalidOperationException("Cannot add parameters to an Active template."));

        if (_parameters.Any(p => p.Name == name))
            return Result<StudyTemplateParameter, Exception>.Failure(
                new InvalidOperationException("Parameter name must be unique within the template."));

        var parameter = StudyTemplateParameter.Create(Id, name, description, aliasName, specification);

        _parameters.Add(parameter);
        return Result<StudyTemplateParameter, Exception>.Success(parameter);
    }

    public Result<Exception> RemoveParameter(
        StudyTemplateParameterId parameterId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot remove parameters from an Active template."));

        var parameter = _parameters.SingleOrDefault(p => p.Id == parameterId);
        if (parameter == null) return Result<Exception>.Failure(new InvalidOperationException("Parameter not found."));

        _parameters.Remove(parameter);
        return Result<Exception>.Success();
    }

    public Result<Exception> Approve()
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(new InvalidOperationException("Only Draft templates can be approved."));

        if (!_parameters.Any())
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
}
