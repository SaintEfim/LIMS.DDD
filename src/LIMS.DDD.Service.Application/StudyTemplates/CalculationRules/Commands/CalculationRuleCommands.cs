using LIMS.DDD.Service.Domain;
using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.CalculationRules;

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
            .Bind(tuple => studyTemplate.AddCalculationRule(tuple.name,tuple.formula, tuple.description))
            .Bind(async result =>
            {
                try
                {
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(result.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(new Exception($"Failed to save CalculationRule: {ex.Message}", ex));
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
}
