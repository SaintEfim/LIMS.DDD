namespace LIMS.Service.LaboratoryOperations.Domain.SeedWork;

public interface IRepository<TEntity>
    where TEntity : IAggregateRoot;
