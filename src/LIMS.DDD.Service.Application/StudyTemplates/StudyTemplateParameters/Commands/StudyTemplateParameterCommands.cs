using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Parameter;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateParameters.Commands;

public sealed class StudyTemplateParameterCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid> AddStudyTemplateParameterAsync(
        Guid studyTemplateId,
        CreateStudyTemplateParameterCommand command,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate =
            await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
            throw new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found.");

        var newParameter = studyTemplate.AddParameter(new Name(command.Name), new Description(command.Description),
            new AliasName(command.AliasName), new ValueRange(command.MinValue, command.MaxValue));

        await repository.SaveChangesAsync(cancellationToken);

        return newParameter.Id.Value;
    }

    public async Task<bool> RemoveStudyTemplateParameterAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate =
            await repository.GetByIdForChangeAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null)
            throw new KeyNotFoundException($"StudyTemplate with id {studyTemplateId} not found.");

        try
        {
            studyTemplate.RemoveParameter(new StudyTemplateParameterId(parameterId));
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        await repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
