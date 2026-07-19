using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates;

public sealed class StudyTemplateQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplate?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(cancellationToken);
    }
}
