using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Entities.InputParameters;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;

public sealed class CalculationRuleCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> AddCalculationRuleAsync(
        Guid studyTemplateId,
        CreateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        return await Name.Create(command.Name)
            .Bind(name => Description.Create(command.Description)
                .Map(description => (name, description)))
            .Bind(tuple => FormulaExpression.Create(command.FormulaExpression)
                .Map(formula => (tuple.name, tuple.description, formula)))
            .Bind(tuple => studyTemplate.AddCalculationRule(tuple.name, tuple.formula, tuple.description,
                new ResultDefinitionId(command.ResultDefinitionId)))
            .Bind(async result =>
            {
                try
                {
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(result.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(
                        new Exception($"Failed to save CalculationRule: {ex.Message}", ex));
                }
            });
    }

    public async Task<Result<Exception>> RemoveCalculationRuleAsync(
        Guid studyTemplateId,
        Guid ruleId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var removeResult = studyTemplate.RemoveCalculationRule(new CalculationRuleId(ruleId));

        if (removeResult.IsFailure)
        {
            return Result<Exception>.Failure(removeResult.Error!);
        }

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to remove CalculationRule: {ex.Message}", ex));
        }
    }

    public async Task<Result<Exception>> AddCalculationInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        AddCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var inputParameterId = new InputParameterId(command.InputParameterId);
        var calculationRuleId = new CalculationRuleId(ruleId);

        var resAddCalculationInput = studyTemplate.AddCalculationInput(calculationRuleId, inputParameterId);

        if (resAddCalculationInput.IsFailure) return Result<Exception>.Failure(resAddCalculationInput.Error!);

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to save CalculationInput: {ex.Message}", ex));
        }
    }

    public async Task<Result<Exception>> RemoveCalculationInputAsync(
        Guid studyTemplateId,
        Guid ruleId,
        RemoveCalculationInputCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var variableAlias = AliasName.Create(command.VariableAlias);
        if (variableAlias.IsFailure) return Result<Exception>.Failure(variableAlias.Error!);

        var calculationRuleId = new CalculationRuleId(ruleId);

        var resRemoveCalculationInput = studyTemplate.RemoveCalculationInput(calculationRuleId, variableAlias.Value);

        if (resRemoveCalculationInput.IsFailure) return Result<Exception>.Failure(resRemoveCalculationInput.Error!);

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to remove CalculationInput: {ex.Message}", ex));
        }
    }

    public async Task<Result<Exception>> UpdateCalculationRuleAsync(
        Guid studyTemplateId,
        Guid ruleId,
        UpdateCalculationRuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));

        Name? name = null;
        if (command.Name is not null)
        {
            var nameResult = Name.Create(command.Name);
            if (nameResult.IsFailure) return Result<Exception>.Failure(nameResult.Error!);
            name = nameResult.Value;
        }

        FormulaExpression? formula = null;
        if (command.FormulaExpression is not null)
        {
            var formulaResult = FormulaExpression.Create(command.FormulaExpression);
            if (formulaResult.IsFailure) return Result<Exception>.Failure(formulaResult.Error!);
            formula = formulaResult.Value;
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure) return Result<Exception>.Failure(descResult.Error!);
            description = descResult.Value;
        }

        ResultDefinitionId? resultDefinitionId = command.ResultDefinitionId.HasValue
            ? new ResultDefinitionId(command.ResultDefinitionId.Value)
            : null;

        var updateResult = studyTemplate.UpdateCalculationRule(new CalculationRuleId(ruleId), name, formula,
            description, resultDefinitionId);

        if (updateResult.IsFailure) return updateResult;

        try
        {
            await repository.SaveChangesAsync(cancellationToken);
            return Result<Exception>.Success();
        }
        catch (Exception ex)
        {
            return Result<Exception>.Failure(new Exception($"Failed to update CalculationRule: {ex.Message}", ex));
        }
    }
}
