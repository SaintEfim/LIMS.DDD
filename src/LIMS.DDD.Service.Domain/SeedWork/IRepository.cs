using LIMS.DDD.Service.Domain.StudyTemplateAggregate;

namespace LIMS.DDD.Service.Domain.Seedwork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot
{
    Task<ICollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        StudyTemplateId id,
        CancellationToken cancellationToken = default);

    void Add(
        TEntity studyTemplate);

    void Remove(
        TEntity studyTemplate);

    void Update(
        TEntity entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
