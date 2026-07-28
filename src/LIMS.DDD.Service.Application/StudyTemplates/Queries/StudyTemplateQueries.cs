using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;

namespace LIMS.DDD.Service.Application.StudyTemplates.Queries;

public sealed class StudyTemplateQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(id), cancellationToken);

        return studyTemplate is null
            ? null
            : StudyTemplateDto.FromDomain(studyTemplate);
    }

    public async Task<ICollection<StudyTemplateDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplates = await repository.GetAllAsync(cancellationToken);

        return studyTemplates.Select(StudyTemplateDto.FromDomain).ToList();
    }
}
