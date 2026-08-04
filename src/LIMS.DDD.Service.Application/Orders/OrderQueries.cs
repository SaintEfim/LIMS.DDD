using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

namespace LIMS.DDD.Service.Application.Orders;

public sealed class OrderQueries(IOrderRepository repository)
{
    public async Task<OrderDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdAsync(new OrderId(id), cancellationToken);
        return order is null ? null : OrderDto.FromDomain(order);
    }
}
