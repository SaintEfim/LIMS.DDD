using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;

public sealed class StudyTemplateResultCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid> AddStudyTemplateResultAsync(
        Guid studyTemplateId,
        CreateStudyTemplateResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate =
            await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            throw new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found.");
        }

        var newResult = studyTemplate.AddResult(command.Unit, new ValueRange(command.MinValue, command.MaxValue));

        await repository.SaveChangesAsync(cancellationToken);

        return newResult.Id.Value;
    }

    public async Task<bool> RemoveStudyTemplateResultAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate =
            await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
        {
            throw new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found.");
        }

        try
        {
            studyTemplate.RemoveResult(new StudyTemplateResultId(resultId));
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
