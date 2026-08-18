namespace LIMS.Service.Methodologies.Domain.SeedWork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot;
