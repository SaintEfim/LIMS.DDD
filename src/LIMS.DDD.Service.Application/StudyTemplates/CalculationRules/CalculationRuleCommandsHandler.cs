using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.SeedWork.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.CalculationRules.ValueObjects;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;

public sealed class CalculationRuleCommandsHandler(
    IStudyTemplateRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid, Exception>> CreateAsync(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<Guid>();
        }

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return nameResult.CastFailure<Guid>();
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return descResult.CastFailure<Guid>();
        }

        var formulaResult = FormulaExpression.Create(command.FormulaExpression);
        if (formulaResult.IsFailure)
        {
            return formulaResult.CastFailure<Guid>();
        }

        var addResult = templateResult.GetValue()
            .AddCalculationRule(nameResult.GetValue(), formulaResult.GetValue(), descResult.GetValue(),
                new ResultDefinitionId(command.ResultDefinitionId));
        if (addResult.IsFailure)
        {
            return addResult.CastFailure<Guid>();
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : Result<Guid, Exception>.Success(addResult.GetValue()
                .Id.Value);
    }

    public async Task<Result<None, Exception>> RemoveAsync(
        Guid studyTemplateId,
        Guid ruleId,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var removeResult = templateResult.GetValue()
            .RemoveCalculationRule(new CalculationRuleId(ruleId));
        if (removeResult.IsFailure)
        {
            return removeResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> CreateInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        AddCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var inputParameterId = new InputParameterId(command.InputParameterId);
        var calculationRuleId = new CalculationRuleId(ruleId);

        var addResult = templateResult.GetValue()
            .AddCalculationInput(calculationRuleId, inputParameterId);
        if (addResult.IsFailure)
        {
            return addResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> RemoveInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        RemoveCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        var variableAliasResult = AliasName.Create(command.VariableAlias);
        if (variableAliasResult.IsFailure)
        {
            return variableAliasResult.CastFailure<None>();
        }

        var calculationRuleId = new CalculationRuleId(ruleId);

        var removeResult = templateResult.GetValue()
            .RemoveCalculationInput(calculationRuleId, variableAliasResult.GetValue());
        if (removeResult.IsFailure)
        {
            return removeResult.CastFailure<None>();
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, Exception>> UpdateAsync(
        Guid studyTemplateId,
        Guid ruleId,
        UpdateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var templateResult = await GetTemplateForChangeAsync(studyTemplateId, cancellationToken);
        if (templateResult.IsFailure)
        {
            return templateResult.CastFailure<None>();
        }

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure)
            {
                return nameResult.CastFailure<None>();
            }

            name = nameResult.GetValue();
        }

        FormulaExpression? formula = null;
        if (command.FormulaExpression is not null)
        {
            var formulaResult = FormulaExpression.Create(command.FormulaExpression);
            if (formulaResult.IsFailure)
            {
                return formulaResult.CastFailure<None>();
            }

            formula = formulaResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return descResult.CastFailure<None>();
            }

            description = descResult.GetValue();
        }

        ResultDefinitionId? resultDefinitionId = command.ResultDefinitionId.HasValue
            ? new ResultDefinitionId(command.ResultDefinitionId.Value)
            : null;

        var updateResult = templateResult.GetValue()
            .UpdateCalculationRule(new CalculationRuleId(ruleId), name, formula, description, resultDefinitionId);
        if (updateResult.IsFailure)
        {
            return updateResult.CastFailure<None>();
        }

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

    private async Task<Result<None, Exception>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<None, Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<None, Exception>.Failure(new Exception($"Failed to save changes: {ex.Message}", ex));
        }
    }
}
