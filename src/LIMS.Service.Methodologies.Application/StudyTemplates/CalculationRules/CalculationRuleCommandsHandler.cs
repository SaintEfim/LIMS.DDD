using Application.SeedWork;
using Application.SeedWork.Errors;
using Domain.SeedWork;
using Domain.SeedWork.Result;
using Domain.SeedWork.ValueObjects;
using LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.CalculationRules;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Entities.ResultDefinitions;

namespace LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules;

public sealed class CalculationRuleCommandsHandler(IStudyTemplateRepository repository, IUnitOfWork unitOfWork)
    : ICommandsHandler
{
    public async Task<Result<Guid, ApplicationError>> CreateAsync(
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
            return new DomainRuleViolation(nameResult.GetError());
        }

        var descResult = Description.Create(command.Description);
        if (descResult.IsFailure)
        {
            return new DomainRuleViolation(descResult.GetError());
        }

        var formulaResult = FormulaExpression.Create(command.FormulaExpression);
        if (formulaResult.IsFailure)
        {
            return new DomainRuleViolation(formulaResult.GetError());
        }

        var addResult = templateResult.GetValue()
            .AddCalculationRule(nameResult.GetValue(), formulaResult.GetValue(), descResult.GetValue(),
                new ResultDefinitionId(command.ResultDefinitionId));
        if (addResult.IsFailure)
        {
            return new DomainRuleViolation(addResult.GetError());
        }

        var saveResult = await SaveChangesAsync(cancellationToken);
        return saveResult.IsFailure
            ? saveResult.CastFailure<Guid>()
            : addResult.GetValue()
                .Id.Value;
    }

    public async Task<Result<None, ApplicationError>> RemoveAsync(
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
            return new DomainRuleViolation(removeResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<None, ApplicationError>> UpdateAsync(
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
                return new DomainRuleViolation(nameResult.GetError());
            }

            name = nameResult.GetValue();
        }

        FormulaExpression? formula = null;
        if (command.FormulaExpression is not null)
        {
            var formulaResult = FormulaExpression.Create(command.FormulaExpression);
            if (formulaResult.IsFailure)
            {
                return new DomainRuleViolation(formulaResult.GetError());
            }

            formula = formulaResult.GetValue();
        }

        Description? description = null;
        if (command.Description is not null)
        {
            var descResult = Description.Create(command.Description);
            if (descResult.IsFailure)
            {
                return new DomainRuleViolation(descResult.GetError());
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
            return new DomainRuleViolation(updateResult.GetError());
        }

        return await SaveChangesAsync(cancellationToken);
    }

    private async Task<Result<StudyTemplate, ApplicationError>> GetTemplateForChangeAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var template = await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);
        if (template is null)
        {
            return new NotFoundError($"Study template with id '{studyTemplateId}' not found.");
        }

        return template;
    }

    private async Task<Result<None, ApplicationError>> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new None();
        }
        catch (Exception ex)
        {
            return new PersistenceError($"Failed to save changes: {ex.Message}");
        }
    }
}
