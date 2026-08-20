namespace Domain.SeedWork.SeedWork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot;
