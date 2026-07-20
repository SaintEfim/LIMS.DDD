using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates;

public sealed class StudyTemplateCommands(IStudyTemplateRepository repository)
{
    public async Task<Guid?> CreateAsync(
        StudyTemplate studyTemplate,
        CancellationToken cancellationToken = default)
    {
        repository.Add(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);

        return studyTemplate.Id.Value;
    }

    public async Task UpdateAsync(
        StudyTemplate studyTemplate,
        CancellationToken cancellationToken = default)
    {
        repository.Update(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(id, cancellationToken) ??
                            throw new KeyNotFoundException($"StudyTemplate with id {id} not found.");

        repository.Remove(studyTemplate);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
