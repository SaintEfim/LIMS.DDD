using LIMS.DDD.Service.Domain.StudyTemplateAggregate;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.StudyTemplateObservations;

namespace LIMS.DDD.Service.Application.StudyTemplates.StudyTemplateObservations.Queries;

public sealed class StudyTemplateObservationQueries(IStudyTemplateRepository repository)
{
    public async Task<StudyTemplateObservationDto?> GetByIdAsync(
        Guid studyTemplateId,
        Guid parameterId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        var parameter =
            studyTemplate?.Observations.SingleOrDefault(p => p.Id == new StudyTemplateObservationId(parameterId));

        return parameter != null ? StudyTemplateObservationDto.FromDomain(parameter) : null;
    }

    public async Task<ICollection<StudyTemplateObservationDto>> GetAllByTemplateIdAsync(
        Guid studyTemplateId,
        CancellationToken cancellationToken = default)
    {
        var studyTemplate = await repository.GetByIdAsync(new StudyTemplateId(studyTemplateId), cancellationToken);

        if (studyTemplate is null) return [];

        return studyTemplate.Observations
            .Select(StudyTemplateObservationDto.FromDomain)
            .ToList();
    }
}
