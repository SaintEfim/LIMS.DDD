namespace LIMS.DDD.Service.Domain;

public interface IRepository<TEntity>
{
    Task<ICollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TEntity> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    TEntity Add(
        TEntity entity);

    Task<TEntity> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    TEntity Update(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
