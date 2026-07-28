using LIMS.DDD.Service.Domain.SeedWork;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.Ids;
using LIMS.DDD.Service.Domain.StudyTemplateAggregate.ValueObjects;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public interface IStudyTemplateRepository : IRepository<StudyTemplate>
{
    Task<ICollection<StudyTemplate>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StudyTemplate?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    Task<StudyTemplate?> GetByIdForChangeAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    void Add(
        StudyTemplate studyTemplate);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAndRevisionAsync(
        Name name,
        Revision revision,
        CancellationToken cancellationToken = default);
}
