using LIMS.DDD.Service.Domain.Seedwork;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result;
using StudyTemplateParameterId = LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter.StudyTemplateParameterId;
using StudyTemplateResultId = LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result.StudyTemplateResultId;

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
            Id = new StudyTemplateId(Guid.NewGuid()),
            Name = name,
            Description = description,
            Revision = revision
        };

        return studyTemplate;
    }

    public void UpdatePartial(
        Name? name,
        Description? description,
        Revision? revision)
    {
        if (Status == Status.Completed)
            throw new InvalidOperationException("Cannot modify a completed study template.");

        if (name is not null) Name = name.Value;
        if (description is not null) Description = description.Value;
        if (revision is not null) Revision = revision.Value;
    }

    public void ChangeStatus(
        Status newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
    }

    public StudyTemplateParameter AddParameter(
        Name name,
        Description description,
        AliasName aliasName,
        ValueRange valueRange)
    {
        if (_parameters.Any(p => p.Name == name))
        {
            throw new InvalidOperationException("Parameter name must be unique within the template.");
        }

        var parameter = StudyTemplateParameter.Create(Id, name, description, aliasName, valueRange);

        _parameters.Add(parameter);
        return parameter;
    }

    public void RemoveParameter(
        StudyTemplateParameterId parameterId)
    {
        var parameter = _parameters.FirstOrDefault(p => p.Id == parameterId);
        if (parameter == null)
        {
            throw new InvalidOperationException("Parameter not found.");
        }

        _parameters.Remove(parameter);
    }

    public StudyTemplateResult AddResult(
        string unit,
        ValueRange valueRange)
    {
        var result = StudyTemplateResult.Create(Id, unit, valueRange);
        _results.Add(result);
        return result;
    }

    public void RemoveResult(
        StudyTemplateResultId resultId)
    {
        var result = _results.FirstOrDefault(r => r.Id == resultId);
        if (result == null)
        {
            throw new InvalidOperationException("Result not found.");
        }

        _results.Remove(result);
    }
}
