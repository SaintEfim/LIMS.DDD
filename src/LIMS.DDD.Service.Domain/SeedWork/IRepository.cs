namespace LIMS.DDD.Service.Domain.SeedWork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot;
