using LIMS.Service.LaboratoryOperations.Domain.OrderAggregate;

namespace LIMS.Service.LaboratoryOperations.Application.Orders;

public sealed class OrderQueries(IOrderRepository repository)
{
    public async Task<OrderDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdAsync(new OrderId(id), cancellationToken);
        return order is null ? null : OrderDto.FromDomain(order);
    }

    public async Task<ICollection<OrderDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var studyTemplates = await repository.GetAllAsync(cancellationToken);

        return studyTemplates.Select(OrderDto.FromDomain)
            .ToList();
    }
}
