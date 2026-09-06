namespace Library.Domain.SeedWork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot;
