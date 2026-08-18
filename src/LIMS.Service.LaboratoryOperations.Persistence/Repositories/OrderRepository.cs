using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Service.LaboratoryOperations.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(
        OrderId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Order>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orderQuery = await _context.Orders
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orderQuery;
    }

    public async Task<Order?> GetByIdForChangeAsync(
        OrderId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders.FindAsync([id], cancellationToken);
    }

    public void Add(
        Order order)
    {
        _context.Orders.Add(order);
    }
}
