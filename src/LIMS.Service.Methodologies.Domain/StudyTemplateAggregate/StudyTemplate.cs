using LIMS.Service.Methodologies.Domain.SeedWork;
using LIMS.Service.Methodologies.Domain.SeedWork.Result;
using LIMS.Service.Methodologies.Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.ValueObjects;
using LIMS.Service.Methodologies.Domain.UnitSnapshots;

namespace LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;

public sealed class StudyTemplate
    : SoftDeletableModel,
        IAggregateRoot
{
    private readonly List<CalculationRule> _calculationRules = [];

    private readonly List<InputParameter> _inputParameters = [];

    private readonly List<ResultDefinition> _resultDefinitions = [];

    private StudyTemplate()
    {
    }

    public StudyTemplateId? ParentId { get; private set; }

    public StudyTemplateId Id { get; private set; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Revision Revision { get; private set; } = null!;

    public Status Status { get; private set; } = Status.Draft;

    public IReadOnlyList<InputParameter> InputParameters => _inputParameters.AsReadOnly();

    public IReadOnlyList<ResultDefinition> ResultDefinitions => _resultDefinitions.AsReadOnly();

    public IReadOnlyList<CalculationRule> CalculationRules => _calculationRules.AsReadOnly();

    public bool CanCreateStudy => Status == Status.Active;

    internal void SetParentId(
        StudyTemplateId parentId)
    {
        ParentId = parentId;
    }

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

        return studyTemplate;
    }

    public Result<None, Exception> UpdatePartial(
        Name? name,
        Description? description)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException(
                "Cannot modify details of an Active or Archived template. Create a new revision.");
        }

        if (name is not null)
        {
            Name = name;
        }

        if (description is not null)
        {
            Description = description;
        }

        return new None();
    }

    public Result<InputParameter, Exception> AddInputParameter(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot add observation to an Active template.");
        }

        if (_inputParameters.Any(p => p.Name == name))
        {
            return new InvalidOperationException("Parameter name must be unique within the template.");
        }

        var parameter = new InputParameter(Id, name, description, aliasName, specification);

        _inputParameters.Add(parameter);
        return parameter;
    }

    public Result<CalculationRule, Exception> AddCalculationRule(
        Name name,
        FormulaExpression formula,
        Description description,
        ResultDefinitionId resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot add calculation rules to an Active template.");
        }

        if (_calculationRules.Any(p => p.Name == name))
        {
            return new InvalidOperationException("Calculation rule name must be unique within the template.");
        }

        if (_resultDefinitions.All(p => p.Id != resultDefinitionId))
        {
            throw new InvalidOperationException("Result definition not found in template.");
        }

        var rule = new CalculationRule(Id, name, formula, description, resultDefinitionId);
        _calculationRules.Add(rule);

        return rule;
    }

    public Result<None, Exception> RemoveCalculationRule(
        CalculationRuleId ruleId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot remove calculation rule from an Active or Archived template.");
        }

        var rule = _calculationRules.SingleOrDefault(r => r.Id == ruleId);
        if (rule == null)
        {
            return new InvalidOperationException("Calculation rule not found.");
        }

        rule.MarkAsDeleted();
        return new None();
    }

    public Result<None, Exception> RemoveInputParameter(
        InputParameterId observationId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot remove observation from an Active template.");
        }

        var parameter = _inputParameters.SingleOrDefault(p => p.Id == observationId);
        if (parameter == null)
        {
            return new InvalidOperationException("Parameter not found.");
        }

        parameter.MarkAsDeleted();
        return new None();
    }

    public Result<None, Exception> ChangeStatus(
        Status newStatus)
    {
        var result = Status.CanTransitionTo(newStatus, this);

        if (result.IsFailure)
        {
            return result.CastFailure<None>();
        }

        if (newStatus == Status.Active)
        {
            var validationResult = ValidateCalculationRulesForActivation();
            if (validationResult.IsFailure)
            {
                return validationResult;
            }
        }

        Status = newStatus;

        return new None();
    }

    private Result<None, Exception> ValidateCalculationRulesForActivation()
    {
        var activeRules = _calculationRules.ToList();

        var activeParameters = _inputParameters.ToList();

        foreach (var ruleValidation in activeRules.Select(rule => rule.ValidateVariables(activeParameters))
                     .Where(ruleValidation => ruleValidation.IsFailure))
        {
            return ruleValidation;
        }

        return new None();
    }

    public Result<ResultDefinition, Exception> AddResultDefinition(
        string resultInstance,
        UnitId unitId,
        Specification valueRange)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot add determination to an Active template.");
        }

        var existsResult = _resultDefinitions.Any(x => x.ResultInstance == resultInstance && x.UnitId == unitId);
        if (existsResult)
        {
            return new InvalidOperationException("Determination result instance already exists.");
        }

        var result = new ResultDefinition(Id, resultInstance, unitId, valueRange);
        _resultDefinitions.Add(result);

        return result;
    }

    public Result<None, Exception> RemoveResultDefinition(
        ResultDefinitionId resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot remove result definition from an Active or Archived template.");
        }

        var resultDef = _resultDefinitions.SingleOrDefault(r => r.Id == resultDefinitionId);
        if (resultDef == null)
        {
            return new InvalidOperationException("Determination result not found.");
        }

        var isUsedInCalculations = _calculationRules.Any(rule => rule.ResultDefinitionId == resultDefinitionId);

        if (isUsedInCalculations)
        {
            return new InvalidOperationException(
                "Cannot remove result definition because it is targeted by calculation rules. Remove or reassign the calculation rules first.");
        }

        resultDef.MarkAsDeleted();
        return new None();
    }

    public Result<None, Exception> UpdateInputParameter(
        InputParameterId parameterId,
        Name? name,
        Description? description,
        AliasName? aliasName,
        double? minValue,
        double? maxValue)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot modify input parameters in an Active or Archived template.");
        }

        var parameter = _inputParameters.FirstOrDefault(p => p.Id == parameterId);
        if (parameter is null)
        {
            return new InvalidOperationException("Input parameter not found.");
        }

        if (aliasName is not null && parameter.AliasName != aliasName)
        {
            if (_inputParameters.Any(p => p.AliasName == aliasName && p.Id != parameterId))
            {
                return new InvalidOperationException("Alias name must be unique within the template.");
            }
        }

        var min = minValue ?? parameter.Specification.MinValue;
        var max = maxValue ?? parameter.Specification.MaxValue;

        var specificationResult = Specification.Create(min, max);
        if (specificationResult.IsFailure)
        {
            return specificationResult.CastFailure<None>();
        }

        var specification = specificationResult.GetValue();

        parameter.Update(name, description, aliasName, specification);
        return new None();
    }

    public Result<None, Exception> UpdateResultDefinition(
        ResultDefinitionId resultDefinitionId,
        string? resultInstance,
        UnitId? unitId,
        double? minValue,
        double? maxValue)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot modify result definitions in an Active or Archived template.");
        }

        var resultDef = _resultDefinitions.FirstOrDefault(r => r.Id == resultDefinitionId);
        if (resultDef is null)
        {
            return new InvalidOperationException("Result definition not found.");
        }

        var min = minValue ?? resultDef.Specification.MinValue;
        var max = maxValue ?? resultDef.Specification.MaxValue;

        var specificationResult = Specification.Create(min, max);
        if (specificationResult.IsFailure)
        {
            return specificationResult.CastFailure<None>();
        }

        var specification = specificationResult.GetValue();

        resultDef.Update(resultInstance, unitId, specification);
        return new None();
    }

    public Result<None, Exception> UpdateCalculationRule(
        CalculationRuleId ruleId,
        Name? name,
        FormulaExpression? formulaExpression,
        Description? description,
        ResultDefinitionId? resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new InvalidOperationException("Cannot modify calculation rules in an Active or Archived template.");
        }

        var rule = _calculationRules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is null)
        {
            return new InvalidOperationException("Calculation rule not found.");
        }

        if (name is not null && rule.Name != name && _calculationRules.Any(r => r.Name == name && r.Id != ruleId))
        {
            return new InvalidOperationException("Calculation rule name must be unique within the template.");
        }

        if (resultDefinitionId is not null && rule.ResultDefinitionId != resultDefinitionId &&
            _resultDefinitions.All(r => r.Id != resultDefinitionId))
        {
            return new InvalidOperationException("Result definition not found in template.");
        }

        rule.Update(name, formulaExpression, description, resultDefinitionId);
        return new None();
    }

    public Result<None, Exception> Delete()
    {
        if (IsDeleted)
        {
            return new InvalidOperationException("Template is already deleted.");
        }

        if (Status != Status.Draft)
        {
            return new InvalidOperationException(
                $"Cannot delete template in '{Status.Name}' status. Only 'Draft' templates can be deleted. Use 'Archive' for Active templates.");
        }

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;

        foreach (var param in _inputParameters)
        {
            param.MarkAsDeleted();
        }

        foreach (var res in _resultDefinitions)
        {
            res.MarkAsDeleted();
        }

        foreach (var rule in _calculationRules)
        {
            rule.MarkAsDeleted();
        }

        return new None();
    }
}
