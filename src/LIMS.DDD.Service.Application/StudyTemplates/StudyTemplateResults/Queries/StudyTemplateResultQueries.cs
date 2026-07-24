using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateResults;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateResults.Queries;

public sealed class StudyTemplateResultQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateResultDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var result = studyTemplate?.Results.SingleOrDefault(r => r.Id == new StudyTemplateResultId(resultId));

        return result != null ? StudyTemplateResultDto.FromDomain(result) : null;
    }

    public async Task<ICollection<StudyTemplateResultDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null) return [];

        return studyTemplate.Results
            .Select(StudyTemplateResultDto.FromDomain)
            .ToList();
    }
}
