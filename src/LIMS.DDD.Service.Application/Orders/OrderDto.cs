using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

namespace LIMS.DDD.Service.Application.Orders;

public sealed record OrderDto(Guid Id, string Name, string? Description, string? Code, string Contractor, string Status)
{
    public static OrderDto FromDomain(
        Order order)
    {
        return new OrderDto(order.Id.Value, order.Name.Value, order.Description.Value, order.Code.Value,
            order.Contractor, order.OrderStatus.Name);
    }
}
