using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates;

public sealed class StudyTemplateQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplate> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}
