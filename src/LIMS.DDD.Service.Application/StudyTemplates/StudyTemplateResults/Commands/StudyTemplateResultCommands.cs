using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Result;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Commands;

public sealed class StudyTemplateResultCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid> AddStudyTemplateResultAsync(
        CreateStudyTemplateResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplateId = new StudyTemplateId(command.StudyTemplateId);
        var studyTemplate = await repository.GetByIdAsync(studyTemplateId, cancellationToken);

        if (studyTemplate is null)
        {
            throw new KeyNotFoundException($"StudyTemplate with id {studyTemplateId.Value} not found.");
        }

        var newResult = studyTemplate.AddResult(
            command.Unit,
            new ValueRange(command.MinValue, command.MaxValue));

        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return newResult.Id.Value;
    }

    public async Task<bool> RemoveStudyTemplateResultAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

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

        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
