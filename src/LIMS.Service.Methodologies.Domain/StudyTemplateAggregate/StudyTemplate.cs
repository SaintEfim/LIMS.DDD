using Domain.SeedWork;
using Domain.SeedWork.Errors;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
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

    // for EF Core
    private StudyTemplate()
    {
    }

    public StudyTemplate(
        Name name,
        Description description,
        Revision revision)
    {
        Id = new StudyTemplateId(Guid.NewGuid());
        Name = name;
        Description = description;
        Revision = revision;
    }

    public StudyTemplateId? ParentId { get; private set; }

    public StudyTemplateId Id { get; }

    public Name Name { get; private set; } = null!;

    public Description Description { get; private set; } = null!;

    public Revision Revision { get; private set; } = null!;

    public Status Status { get; private set; } = Status.Draft;

    public IReadOnlyList<InputParameter> InputParameters => _inputParameters.AsReadOnly();

    public IReadOnlyList<ResultDefinition> ResultDefinitions => _resultDefinitions.AsReadOnly();

    public IReadOnlyList<CalculationRule> CalculationRules => _calculationRules.AsReadOnly();

    internal void SetParentId(
        StudyTemplateId parentId)
    {
        ParentId = parentId;
    }

    public Result<None, DomainError> UpdatePartial(
        Name? name,
        Description? description)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(StudyTemplate), Status.Name, "modify template details");
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

    public Result<InputParameter, DomainError> AddInputParameter(
        Name name,
        Description description,
        AliasName aliasName,
        Specification specification)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(InputParameter), Status.Name, "add input parameters to");
        }

        if (_inputParameters.Any(p => p.Name == name))
        {
            return new DuplicateEntityError("Input parameter", "name", name.Value);
        }

        var parameter = new InputParameter(Id, name, description, aliasName, specification);

        _inputParameters.Add(parameter);
        return parameter;
    }

    public Result<CalculationRule, DomainError> AddCalculationRule(
        Name name,
        FormulaExpression formula,
        Description description,
        ResultDefinitionId resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(CalculationRule), Status.Name, "add calculation rules to");
        }

        if (_calculationRules.Any(p => p.Name == name))
        {
            return new DuplicateEntityError("Calculation rule", "name", name.Value);
        }

        if (_resultDefinitions.All(p => p.Id != resultDefinitionId))
        {
            return new EntityNotFoundError("Result definition", resultDefinitionId.Value);
        }

        var rule = new CalculationRule(Id, name, formula, description, resultDefinitionId);
        _calculationRules.Add(rule);

        return rule;
    }

    public Result<None, DomainError> RemoveCalculationRule(
        CalculationRuleId ruleId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(CalculationRule), Status.Name, "remove calculation rules from");
        }

        var rule = _calculationRules.SingleOrDefault(r => r.Id == ruleId);
        if (rule == null)
        {
            return new EntityNotFoundError("Calculation rule", ruleId.Value);
        }

        rule.MarkAsDeleted();
        return new None();
    }

    public Result<None, DomainError> RemoveInputParameter(
        InputParameterId observationId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(InputParameter), Status.Name, "remove input parameters from");
        }

        var parameter = _inputParameters.SingleOrDefault(p => p.Id == observationId);
        if (parameter == null)
        {
            return new EntityNotFoundError("Input parameter", observationId.Value);
        }

        parameter.MarkAsDeleted();
        return new None();
    }

    public Result<None, DomainError> ChangeStatus(
        Status newStatus)
    {
        var result = Status.CanTransitionTo(newStatus, this);

        if (result.IsFailure)
        {
            return Result<None, DomainError>.Failure(result.GetError());
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

    private Result<None, DomainError> ValidateCalculationRulesForActivation()
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

    public Result<ResultDefinition, DomainError> AddResultDefinition(
        string resultInstance,
        UnitId unitId,
        Specification valueRange)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(ResultDefinition), Status.Name, "add result definitions to");
        }

        var existsResult = _resultDefinitions.Any(x => x.ResultInstance == resultInstance && x.UnitId == unitId);
        if (existsResult)
        {
            return new DuplicateEntityError("Result definition", "result instance + unit",
                $"{resultInstance} ({unitId.Value})");
        }

        var result = new ResultDefinition(Id, resultInstance, unitId, valueRange);
        _resultDefinitions.Add(result);

        return result;
    }

    public Result<None, DomainError> RemoveResultDefinition(
        ResultDefinitionId resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(ResultDefinition), Status.Name, "remove result definitions from");
        }

        var resultDef = _resultDefinitions.SingleOrDefault(r => r.Id == resultDefinitionId);
        if (resultDef == null)
        {
            return new EntityNotFoundError("Result definition", resultDefinitionId.Value);
        }

        var isUsedInCalculations = _calculationRules.Any(rule => rule.ResultDefinitionId == resultDefinitionId);

        if (isUsedInCalculations)
        {
            return new EntityInUseError("Result definition", "calculation rules");
        }

        resultDef.MarkAsDeleted();
        return new None();
    }

    public Result<None, DomainError> UpdateInputParameter(
        InputParameterId parameterId,
        Name? name,
        Description? description,
        AliasName? aliasName,
        double? minValue,
        double? maxValue)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(InputParameter), Status.Name, "update input parameters");
        }

        var parameter = _inputParameters.FirstOrDefault(p => p.Id == parameterId);
        if (parameter is null)
        {
            return new EntityNotFoundError("Input parameter", parameterId.Value);
        }

        if (aliasName is not null && parameter.AliasName != aliasName)
        {
            if (_inputParameters.Any(p => p.AliasName == aliasName && p.Id != parameterId))
            {
                return new DuplicateEntityError("Input parameter", "alias name", aliasName.Value);
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

    public Result<None, DomainError> UpdateResultDefinition(
        ResultDefinitionId resultDefinitionId,
        string? resultInstance,
        UnitId? unitId,
        double? minValue,
        double? maxValue)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(ResultDefinition), Status.Name, "update result definitions");
        }

        var resultDef = _resultDefinitions.FirstOrDefault(r => r.Id == resultDefinitionId);
        if (resultDef is null)
        {
            return new EntityNotFoundError("Result definition", resultDefinitionId.Value);
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

    public Result<None, DomainError> UpdateCalculationRule(
        CalculationRuleId ruleId,
        Name? name,
        FormulaExpression? formulaExpression,
        Description? description,
        ResultDefinitionId? resultDefinitionId)
    {
        if (!Status.CanEdit)
        {
            return new EntityNotEditableError(nameof(CalculationRule), Status.Name, "update calculation rules");
        }

        var rule = _calculationRules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is null)
        {
            return new EntityNotFoundError("Calculation rule", ruleId.Value);
        }

        if (name is not null && rule.Name != name && _calculationRules.Any(r => r.Name == name && r.Id != ruleId))
        {
            return new DuplicateEntityError("Calculation rule", "name", name.Value);
        }

        if (resultDefinitionId is not null && rule.ResultDefinitionId != resultDefinitionId &&
            _resultDefinitions.All(r => r.Id != resultDefinitionId))
        {
            return new EntityNotFoundError("Result definition", resultDefinitionId.Value);
        }

        rule.Update(name, formulaExpression, description, resultDefinitionId);
        return new None();
    }

    public Result<None, DomainError> Delete()
    {
        if (IsDeleted)
        {
            return new EntityAlreadyDeletedError("Study template", Id.Value);
        }

        if (Status != Status.Draft)
        {
            return Result<None, DomainError>.Failure(
                new InvalidStatusTransitionError(nameof(StudyTemplate), Status.Name, "Deleted"));
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
