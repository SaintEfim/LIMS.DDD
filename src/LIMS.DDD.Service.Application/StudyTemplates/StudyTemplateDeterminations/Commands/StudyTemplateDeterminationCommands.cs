using LIMS.DDD.Service.Domain.SeedWork.Result;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Commands;

public sealed class StudyTemplateDeterminationCommands(IStudyTemplateRepository repository)
{
    public async Task<Result<Guid, Exception>> AddStudyTemplateDeterminationAsync(
        Guid studyTemplateId,
        CreateStudyTemplateDeterminationCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Guid, Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        return await studyTemplate.AddStudyTemplateDetermination(command.ResultInstance, command.Unit,
                new Specification(command.MinValue, command.MaxValue))
            .Bind(async result =>
            {
                try
                {
                    await repository.SaveChangesAsync(cancellationToken);
                    return Result<Guid, Exception>.Success(result.Id.Value);
                }
                catch (Exception ex)
                {
                    return Result<Guid, Exception>.Failure(new Exception($"Failed to save StudyTemplateDetermination: {ex.Message}", ex));
                }
            });
    }

    public async Task<Result<Exception>> RemoveStudyTemplateDeterminationAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdForChangeAsync(
            new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            return Result<Exception>.Failure(
                new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found."));
        }

        var removeResult = studyTemplate.RemoveStudyTemplateDetermination(new StudyTemplateDeterminationId(resultId));

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
            return Result<Exception>.Failure(new Exception($"Failed to remove StudyTemplateDetermination: {ex.Message}", ex));
        }
    }
}
