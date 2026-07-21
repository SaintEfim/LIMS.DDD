using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Application.StudyTemplates.Queries;

public sealed class StudyTemplateQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        return StudyTemplateDto.FromDomain(studyTemplate);
    }

    public async Task<ICollection<StudyTemplateDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return (await repository.GetAllAsync(cancellationToken)).Select(StudyTemplateDto.FromDomain)
            .ToList();
    }
}
