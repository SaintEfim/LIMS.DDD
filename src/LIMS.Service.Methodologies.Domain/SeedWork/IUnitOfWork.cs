namespace LIMS.Service.Methodologies.Domain.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
