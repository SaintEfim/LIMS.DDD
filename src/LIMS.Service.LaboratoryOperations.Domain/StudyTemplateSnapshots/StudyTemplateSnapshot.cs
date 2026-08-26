using Domain.SeedWork;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.CalculationRules;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.InputParameters;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots.ResultDefinitions;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

public sealed class StudyTemplateSnapshot
    : SoftDeletableModel,
        IAggregateRoot
{
    private readonly List<CalculationRuleSnapshot> _calculationRules = [];
    private readonly List<InputParameterSnapshot> _parameters = [];
    private readonly List<ResultDefinitionSnapshot> _results = [];

    private StudyTemplateSnapshot()
    {
    }

    public StudyTemplateSnapshot(
        StudyTemplateId id,
        Revision revision,
        Name name,
        Description description,
        IReadOnlyList<InputParameterSnapshot> parameters,
        IReadOnlyList<ResultDefinitionSnapshot> results,
        IReadOnlyList<CalculationRuleSnapshot> calculationRules)
    {
        Id = id;
        Revision = revision;
        Name = name;
        Description = description;
        _parameters.AddRange(parameters);
        _results.AddRange(results);
        _calculationRules.AddRange(calculationRules);
    }

    public StudyTemplateId Id { get; private set; }
    public Revision Revision { get; private set; } = null!;
    public Name Name { get; private set; } = null!;
    public Description Description { get; private set; } = null!;

    public IReadOnlyList<InputParameterSnapshot> Parameters => _parameters.AsReadOnly();
    public IReadOnlyList<ResultDefinitionSnapshot> Results => _results.AsReadOnly();
    public IReadOnlyList<CalculationRuleSnapshot> CalculationRules => _calculationRules.AsReadOnly();
}
