using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.StudyTemplateAggregate;

public interface IStudyTemplateRepository : IRepository<StudyTemplate>
{
    Task<bool> ExistsByNameAndRevisionAsync(Name name, Revision revision, CancellationToken cancellationToken = default);
}
