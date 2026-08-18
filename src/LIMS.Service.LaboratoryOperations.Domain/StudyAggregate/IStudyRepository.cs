using LIMS.Service.LaboratoryOperations.Domain.SampleAggregate;
using LIMS.Service.LaboratoryOperations.Domain.SeedWork;

namespace LIMS.Service.LaboratoryOperations.Domain.StudyAggregate;

public interface IStudyRepository : IRepository<Study>
{
    Task<Study?> GetByIdForChangeAsync(
        StudyId id,
        CancellationToken cancellationToken = default);

    Task<Study?> GetByIdAsync(
        StudyId id,
        CancellationToken cancellationToken = default);

    Task<ICollection<Study>> GetBySampleIdAsync(
        SampleId sampleId,
        CancellationToken cancellationToken = default);

    void Add(
        Study study);
}
