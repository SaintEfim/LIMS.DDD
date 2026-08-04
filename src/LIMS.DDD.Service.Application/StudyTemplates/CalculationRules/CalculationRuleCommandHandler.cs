using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;

public sealed class CalculationRuleCommandHandler(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Guid, Exception>.Failure(templateResult.Error!);

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure) return Result<Guid, Exception>.Failure(nameResult.Error!);

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure) return Result<Guid, Exception>.Failure(descResult.Error!);

        var formulaResult = FormulaExpression.Create(command.FormulaExpression);
        if (formulaResult.IsFailure) return Result<Guid, Exception>.Failure(formulaResult.Error!);

        var addResult = templateResult.GetValue().AddCalculationRule(nameResult.GetValue(), formulaResult.GetValue(),
            descResult.GetValue(), new ResultDefinitionId(command.ResultDefinitionId));
        if (addResult.IsFailure) return Result<Guid, Exception>.Failure(addResult.Error!);

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? Result<Guid, Exception>.Failure(saveResult.Error!)
            : Result<Guid, Exception>.Success(addResult.GetValue().Id.Value);
    }

    public async Task<Result<Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid ruleId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var removeResult = templateResult.GetValue().RemoveCalculationRule(new CalculationRuleId(ruleId));
        if (removeResult.IsFailure) return Result<Exception>.Failure(removeResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> CreateInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        AddCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var inputParameterId = new InputParameterId(command.InputParameterId);
        var calculationRuleId = new CalculationRuleId(ruleId);

        var addResult = templateResult.GetValue().AddCalculationInput(calculationRuleId, inputParameterId);
        if (addResult.IsFailure) return Result<Exception>.Failure(addResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> RemoveInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        RemoveCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        var variableAliasResult = AliasName.Create(command.VariableAlias);
        if (variableAliasResult.IsFailure) return Result<Exception>.Failure(variableAliasResult.Error!);

        var calculationRuleId = new CalculationRuleId(ruleId);

        var removeResult = templateResult.GetValue().RemoveCalculationInput(calculationRuleId, variableAliasResult.GetValue());
        if (removeResult.IsFailure) return Result<Exception>.Failure(removeResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid ruleId,
        UpdateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure) return Result<Exception>.Failure(templateResult.Error!);

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure) return Result<Exception>.Failure(nameResult.Error!);
            name = nameResult.GetValue();
        }

        FormulaExpression? formula = null;
        if (command.FormulaExpression is not null)
        {
            var formulaResult = FormulaExpression.Create(command.FormulaExpression);
            if (formulaResult.IsFailure) return Result<Exception>.Failure(formulaResult.Error!);
            formula = formulaResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return Result<Exception>.Failure(descResult.Error!);
            description = descResult.GetValue();
        }

        ResultDefinitionId? resultDefinitionId = command.ResultDefinitionId.HasValue
            ? new ResultDefinitionId(command.ResultDefinitionId.Value)
            : null;

        var updateResult = templateResult.GetValue().UpdateCalculationRule(new CalculationRuleId(ruleId), name, formula,
            description, resultDefinitionId);
        if (updateResult.IsFailure) return Result<Exception>.Failure(updateResult.Error!);

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, Exception>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        return template is null
            ? Result<StudyTemplate, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."))
            : Result<StudyTemplate, Exception>.Success(template);
    }

    private async Task<Result<Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
