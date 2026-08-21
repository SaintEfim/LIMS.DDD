using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(
        OrderId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Orders.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orderQuery = await context.Orders
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orderQuery;
    }

    public async Task<Order?> GetByIdForChangeAsync(
        OrderId id,
        CancellationToken cancellationToken = default)
    {
        return await context.Orders.FindAsync([id], cancellationToken);
    }

    public void Add(
        Order order)
    {
        context.Orders.Add(order);
    }
}
