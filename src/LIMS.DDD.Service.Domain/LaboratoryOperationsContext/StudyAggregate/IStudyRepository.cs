using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate;
using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate;

public interface IStudyRepository : IRepository<Study>
{
    Task<Study?> GetByIdForChangeAsync(
        StudyId id,
        CancellationToken cancellationToken = default);

    Task<ICollection<Study>> GetBySampleIdAsync(
        SampleId sampleId,
        CancellationToken cancellationToken = default);

    void Add(
        Study study);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<ICollection<Study>> GetByTemplateIdAsync(
        StudyTemplateId templateId,
        CancellationToken cancellationToken);
}
