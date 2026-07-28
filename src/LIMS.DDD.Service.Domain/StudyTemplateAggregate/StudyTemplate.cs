using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ResultDefinitions;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public readonly record struct StudyTemplateId(Guid Value) : IValueObjectId
{
    public Guid Value { get; } = Value;
}

public sealed class StudyTemplate : IAggregateRoot
{
    private StudyTemplate()
    {
    }

    public StudyTemplateId? ParentId { get; private set; }

    internal void SetParentId(
        StudyTemplateId parentId)
    {
        ParentId = parentId;
    }

    public StudyTemplateId Id { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public Revision Revision { get; private set; }

    public Status Status { get; private set; }

    public IReadOnlyList<InputParameter> InputParameters => _inputParameters.AsReadOnly();

    private readonly List<InputParameter> _inputParameters = [];

    public IReadOnlyList<ResultDefinition> ResultDefinitions => _resultDefinitions.AsReadOnly();

    private readonly List<ResultDefinition> _resultDefinitions = [];

    public IReadOnlyList<CalculationRule> CalculationRules => _calculationRules.AsReadOnly();

    private readonly List<CalculationRule> _calculationRules = [];

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

    public Result<InputParameter, Exception> AddInputParameter(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (Status != Status.Draft)
            return Result<InputParameter, Exception>.Failure(
                new InvalidOperationException("Cannot add observation to an Active template."));

        if (_inputParameters.Any(p => p.Name == name))
            return Result<InputParameter, Exception>.Failure(
                new InvalidOperationException("Parameter name must be unique within the template."));

        var parameter = InputParameter.Create(Id, name, description, aliasName, specification);

        _inputParameters.Add(parameter);
        return Result<InputParameter, Exception>.Success(parameter);
    }

    public Result<CalculationRule, Exception> AddCalculationRule(
        Name name,
        FormulaExpression formula,
        Description description,
        ResultDefinitionId resultDefinitionId)
    {
        if (Status != Status.Draft)
            return Result<CalculationRule, Exception>.Failure(
                new InvalidOperationException("Cannot add calculation rules to an Active template."));

        if (_calculationRules.Any(p => p.Name == name))
            return Result<CalculationRule, Exception>.Failure(
                new InvalidOperationException("Calculation rule name must be unique within the template."));

        if (_resultDefinitions.All(p => p.Id != resultDefinitionId))
            throw new InvalidOperationException("Result definition not found in template.");

        var rule = CalculationRule.Create(Id, name, formula, description, resultDefinitionId);
        _calculationRules.Add(rule);

        return Result<CalculationRule, Exception>.Success(rule);
    }

    public Result<Exception> RemoveCalculationRule(
        CalculationRuleId ruleId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot add calculation rule to an Active template."));

        var result = _calculationRules.SingleOrDefault(r => r.Id == ruleId);
        if (result == null)
        {
            return Result<Exception>.Failure(new InvalidOperationException("Calculation rule not found."));
        }

        _calculationRules.Remove(result);
        return Result<Exception>.Success();
    }

    public Result<Exception> AddCalculationInput(
        CalculationRuleId ruleId,
        InputParameterId inputParameterId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify calculation rules in an Active template."));

        var rule = _calculationRules.SingleOrDefault(r => r.Id == ruleId);
        if (rule == null)
            return Result<Exception>.Failure(new InvalidOperationException("Calculation rule not found."));

        var parameter = _inputParameters.SingleOrDefault(p => p.Id == inputParameterId);

        return parameter is null
            ? Result<Exception>.Failure(new InvalidOperationException("InputParameter not found in template."))
            : rule.AddInput(parameter.AliasName, inputParameterId);
    }

    public Result<Exception> RemoveCalculationInput(
        CalculationRuleId ruleId,
        AliasName variableAlias)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify calculation rules in an Active template."));

        var rule = _calculationRules.SingleOrDefault(r => r.Id == ruleId);
        return rule == null
            ? Result<Exception>.Failure(new InvalidOperationException("Calculation rule not found."))
            : rule.RemoveInput(variableAlias);
    }

    public Result<Exception> RemoveInputParameter(
        InputParameterId observationId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot remove observation from an Active template."));

        var parameter = _inputParameters.SingleOrDefault(p => p.Id == observationId);
        if (parameter == null) return Result<Exception>.Failure(new InvalidOperationException("Parameter not found."));

        _inputParameters.Remove(parameter);
        return Result<Exception>.Success();
    }

    public Result<Exception> ChangeStatus(
        Status newStatus)
    {
        // Идемпотентность: если статус не меняется — успех
        if (Status == newStatus) return Result<Exception>.Success();

        switch (newStatus)
        {
            case Status.Active:
                if (Status != Status.Draft)
                    return Result<Exception>.Failure(
                        new InvalidOperationException("Only Draft templates can be activated."));

                if (_inputParameters.Count == 0)
                    return Result<Exception>.Failure(
                        new InvalidOperationException("Cannot activate a template without input parameters."));

                if (_resultDefinitions.Count == 0)
                    return Result<Exception>.Failure(
                        new InvalidOperationException("Cannot activate a template without result definitions."));

                if (_calculationRules.Count == 0)
                    return Result<Exception>.Failure(
                        new InvalidOperationException("Cannot activate a template without calculation rules."));
                break;

            case Status.Archived:
                if (Status != Status.Active && Status != Status.Draft)
                    return Result<Exception>.Failure(
                        new InvalidOperationException("Only Active or Draft templates can be archived."));
                break;

            case Status.Draft:
                if (Status == Status.Active)
                    return Result<Exception>.Failure(new InvalidOperationException(
                        "Active templates cannot be reverted to Draft. Create a new revision instead."));

                if (Status == Status.Archived)
                    return Result<Exception>.Failure(new InvalidOperationException(
                        "Archived templates cannot be reverted to Draft. Create a new revision instead."));
                break;

            default:
                return Result<Exception>.Failure(new ArgumentException($"Unknown status: {newStatus}"));
        }

        Status = newStatus;

        return Result<Exception>.Success();
    }

    public Result<ResultDefinition, Exception> AddResultDefinition(
        string resultInstance,
        string unit,
        Specification valueRange)
    {
        if (Status != Status.Draft)
            return Result<ResultDefinition, Exception>.Failure(
                new InvalidOperationException("Cannot add determination to an Active template."));

        var existsResult = _resultDefinitions.Any(x => x.ResultInstance == resultInstance && x.Unit == unit);
        if (existsResult)
        {
            return Result<ResultDefinition, Exception>.Failure(
                new InvalidOperationException("Determination result instance already exists."));
        }

        var result = ResultDefinition.Create(Id, resultInstance, unit, valueRange);
        _resultDefinitions.Add(result);

        return Result<ResultDefinition, Exception>.Success(result);
    }

    public Result<Exception> RemoveResultDefinition(
        ResultDefinitionId determinationId)
    {
        if (Status != Status.Draft)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot add determination to an Active template."));

        var result = _resultDefinitions.SingleOrDefault(r => r.Id == determinationId);
        if (result == null)
        {
            return Result<Exception>.Failure(new InvalidOperationException("Determination result not found."));
        }

        _resultDefinitions.Remove(result);
        return Result<Exception>.Success();
    }
}
