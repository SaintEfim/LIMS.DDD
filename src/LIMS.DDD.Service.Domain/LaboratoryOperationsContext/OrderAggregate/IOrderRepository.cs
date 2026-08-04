using LIMS.DDD.Service.Domain.SeedWork;

namespace LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdAsync(
        OrderId id,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdForChangeAsync(
        OrderId id,
        CancellationToken cancellationToken = default);

    void Add(
        Order order);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
