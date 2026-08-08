using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

namespace LIMS.DDD.Service.Application.Studies.Core;

public sealed class StudyQueries(IStudyRepository repository)
{
    public async Task<StudyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(id), cancellationToken);
        return study is not null ? StudyDto.FromDomain(study) : null;
    }

    public async Task<ICollection<StudyDto>> GetAllBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        var studies = await repository.GetBySampleIdAsync(new SampleId(sampleId), cancellationToken);
        return studies.Select(StudyDto.FromDomain)
            .ToList();
    }
}
