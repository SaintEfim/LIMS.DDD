using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateDeterminations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateDeterminations.Queries;

public sealed class StudyTemplateDeterminationQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateDeterminationDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid resultId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var result =
            studyTemplate?.Determinations.SingleOrDefault(r => r.Id == new StudyTemplateDeterminationId(resultId));

        return result != null ? StudyTemplateDeterminationDto.FromDomain(result) : null;
    }

    public async Task<ICollection<StudyTemplateDeterminationDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null) return [];

        return studyTemplate.Determinations
            .Select(StudyTemplateDeterminationDto.FromDomain)
            .ToList();
    }
}
