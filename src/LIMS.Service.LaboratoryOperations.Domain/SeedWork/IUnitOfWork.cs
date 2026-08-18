namespace LIMS.Service.LaboratoryOperations.Domain.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
