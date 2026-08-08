namespace LIMS.DDD.Service.Domain.SeedWork;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
