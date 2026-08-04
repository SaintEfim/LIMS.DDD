using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.OrderAggregate;

namespace LIMS.DDD.Service.Application.Orders;

public sealed record OrderDto(
    Guid Id,
    string Name,
    string? Description,
    string? Code,
    string Contractor,
    string Status)
{
    public static OrderDto FromDomain(Order order)
    {
        return new OrderDto(
            Id: order.Id.Value,
            Name: order.Name.Value,
            Description: order.Description?.Value,
            Code: order.Code?.Value,
            Contractor: order.Contractor,
            Status: order.OrderStatus.Name);
    }
}
