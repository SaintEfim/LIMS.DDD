using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

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

    public Status Status { get; private set; } = Status.Draft;

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
        if (!Status.CanEdit)
            return Result<StudyTemplate, Exception>.Failure(
                new InvalidOperationException(
                    "Cannot modify details of an Active or Archived template. Create a new revision."));

        if (name is not null) Name = name;
        if (description is not null) Description = description;

        return Result<StudyTemplate, Exception>.Success(this);
    }

    public Result<InputParameter, Exception> AddInputParameter(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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
        var result = Status.CanTransitionTo(newStatus, this);

        if (result.IsFailure) return result;

        Status = newStatus;

        return Result<Exception>.Success();
    }

    public Result<ResultDefinition, Exception> AddResultDefinition(
        string resultInstance,
        string unit,
        Specification valueRange)
    {
        if (!Status.CanEdit)
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
        if (!Status.CanEdit)
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

    public Result<Exception> UpdateInputParameter(
        InputParameterId parameterId,
        Name? name,
        Description? description,
        AliasName? aliasName,
        Specification? specification)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify input parameters in an Active or Archived template."));

        var parameter = _inputParameters.FirstOrDefault(p => p.Id == parameterId);
        if (parameter is null)
            return Result<Exception>.Failure(new InvalidOperationException("Input parameter not found."));

        if (aliasName is not null && parameter.AliasName != aliasName)
        {
            if (_inputParameters.Any(p => p.AliasName == aliasName && p.Id != parameterId))
                return Result<Exception>.Failure(
                    new InvalidOperationException("Alias name must be unique within the template."));
        }

        parameter.Update(name, description, aliasName, specification);
        return Result<Exception>.Success();
    }

    public Result<Exception> UpdateResultDefinition(
        ResultDefinitionId resultDefinitionId,
        string? resultInstance,
        string? unit,
        Specification? specification)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify result definitions in an Active or Archived template."));

        var resultDef = _resultDefinitions.FirstOrDefault(r => r.Id == resultDefinitionId);
        if (resultDef is null)
            return Result<Exception>.Failure(new InvalidOperationException("Result definition not found."));

        resultDef.Update(resultInstance, unit, specification);
        return Result<Exception>.Success();
    }

    public Result<Exception> UpdateCalculationRule(
        CalculationRuleId ruleId,
        Name? name,
        FormulaExpression? formulaExpression,
        Description? description,
        ResultDefinitionId? resultDefinitionId)
    {
        if (!Status.CanEdit)
            return Result<Exception>.Failure(
                new InvalidOperationException("Cannot modify calculation rules in an Active or Archived template."));

        var rule = _calculationRules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is null)
            return Result<Exception>.Failure(new InvalidOperationException("Calculation rule not found."));

        if (name is not null && rule.Name != name)
        {
            if (_calculationRules.Any(r => r.Name == name && r.Id != ruleId))
                return Result<Exception>.Failure(
                    new InvalidOperationException("Calculation rule name must be unique within the template."));
        }

        if (resultDefinitionId is not null && rule.ResultDefinitionId != resultDefinitionId)
        {
            if (_resultDefinitions.All(r => r.Id != resultDefinitionId))
                return Result<Exception>.Failure(
                    new InvalidOperationException("Result definition not found in template."));
        }

        rule.Update(name, formulaExpression, description, resultDefinitionId);
        return Result<Exception>.Success();
    }
}
