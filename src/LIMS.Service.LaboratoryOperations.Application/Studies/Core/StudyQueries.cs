using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;
using LIMS.Service.LaboratoryOperations.Domain.StudyTemplateSnapshots;

namespace LIMS.Service.LaboratoryOperations.Application.Studies.Core;

public sealed class StudyQueries(IStudyRepository repository, IStudyTemplateSnapshotRepository snapshotRepository)
{
    public async Task<StudyDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var study = await repository.GetByIdAsync(new StudyId(id), cancellationToken);
        if (study is null)
        {
            return null;
        }

        var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
        if (snapshot is null)
        {
            throw new KeyNotFoundException("template not found");
        }

        return StudyDto.FromDomain(study, snapshot);
    }

    public async Task<ICollection<StudyDto>> GetAllBySampleIdAsync(
        Guid sampleId,
        CancellationToken cancellationToken = default)
    {
        var studies = await repository.GetBySampleIdAsync(new SampleId(sampleId), cancellationToken);
        if (studies.Count == 0)
        {
            return [];
        }

        var studyDtos = new List<StudyDto>();

        foreach (var study in studies)
        {
            var snapshot = await snapshotRepository.GetByIdAsync(study.StudyTemplateId, cancellationToken);
            if (snapshot is null)
            {
                throw new KeyNotFoundException("template not found");
            }

            studyDtos.Add(StudyDto.FromDomain(study, snapshot));
        }

        return studyDtos;
    }
}
