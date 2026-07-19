using LIMS.DDD.Service.Domain;
using Microsoft.EntityFrameworkCore;

namespace LIMS.DDD.Service.Persistence.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly DbContext _context;
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(
        DbContext context)
    {
        _context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity of type {typeof(TEntity).Name} with id {id} not found.");
        }

        return entity;
    }

    public TEntity Add(
        TEntity entity)
    {
        DbSet.Add(entity);
        return entity;
    }

    public async Task<TEntity> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        DbSet.Remove(entity);
        return entity;
    }

    public TEntity Update(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return entity;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
